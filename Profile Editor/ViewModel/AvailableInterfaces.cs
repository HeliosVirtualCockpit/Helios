// Copyright 2021 Ammo Goettsch
// 
// Profile Editor is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Profile Editor is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;

namespace GadrocsWorkshop.Helios.ProfileEditor.ViewModel
{
    internal class AvailableInterfaces : DependencyObject, IAvailableInterfaces, IDisposable
    {
        /// <summary>
        /// factories that may be working on our behalf to find new interface instances, so we shut them down when
        /// we are done
        /// </summary>
        private readonly HashSet<IHeliosInterfaceFactoryAsync> _asyncFactories =
            new HashSet<IHeliosInterfaceFactoryAsync>();

        public AvailableInterfaces()
        {
            // create fresh collection, instead of relying on dependency property initializer
            Items = new ObservableCollection<Item>();
        }

        /// <summary>
        /// enumerate the initial synchronous set of interfaces and start asynchronously scanning for more
        /// until Dispose() is called
        ///
        /// </summary>
        /// <exception cref="Exception">if interface factories throw anything</exception>
        /// <param name="profile"></param>
        public void Start(HeliosProfile profile)
        {
            if (profile == null)
            {
                throw new Exception("available interface list instantiated without a profile; UI logic error");
            }

            List<Item> synchronousItems = new List<Item>();
            foreach (HeliosInterfaceDescriptor descriptor in ConfigManager.ModuleManager.InterfaceDescriptors)
            {
                ConfigManager.LogManager.LogInfo("Checking for available instances of " + descriptor.Name +
                                                 " interface.");
                try
                {
                    if (descriptor.Factory is IHeliosInterfaceFactoryAsync async)
                    {
                        // don't wait for all instances now, let the factory tell them to us as it discovers,
                        // potentially on another thread
                        async.StartDiscoveringInterfaces(this, profile);
                        _asyncFactories.Add(async);
                        continue;
                    }

                    foreach (HeliosInterface newInterface in descriptor.GetNewInstances(profile))
                    {
                        if (!descriptor.Deprecated)
                        {
                            ConfigManager.LogManager.LogInfo("Adding " + newInterface.Name + " Type: " +
                                                             descriptor.InterfaceType.BaseType.Name +
                                                             " to add interface list.");
                            synchronousItems.Add(new InterfaceItem(newInterface));
                        } else
                        {
                            ConfigManager.LogManager.LogInfo("Suppressing " + newInterface.Name + " Type: " +
                                                              descriptor.InterfaceType.BaseType.Name +
                                                              " from add interface list because it has been flagged as deprecated.");
                        }
                    }
                }
                catch (Exception e)
                {
                    ConfigManager.LogManager.LogError(
                        "Error trying to get available instances for " + descriptor.Name + " interface.", e);
                    throw;
                }
            }

            synchronousItems.Sort(SortItemsByName);
            Items = new ObservableCollection<Item>(synchronousItems);
        }

        private static void SelectedInterfaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((AvailableInterfaces)d).CanAdd = args.NewValue != null;
        }

        private static int SortItemsByName(Item left, Item right) =>
            string.Compare(left.Name, right.Name, StringComparison.InvariantCulture);

        private void DoReceiveAvailableInstance(IHeliosInterfaceFactoryAsync factory, string displayName,
            object context)
        {
            Item newItem = new DeferredItem(displayName, factory, context);

            // REVISIT: if we had LINQ binary search, we would use it here
            // select index of first item greater in sort order than the new item, or 0
            int position = Items
                .Select((item, index) => (SortItemsByName(item, newItem) > 0)
                    ? index
                    : -1)
                .FirstOrDefault(index => index > -1);
            Items.Insert(position, newItem);
        }

        #region IAvailableInterfaces

        public void ReceiveAvailableInstance(IHeliosInterfaceFactoryAsync factory, string displayName, object context)
        {
            Application.Current?.Dispatcher.BeginInvoke(
                new Action<IHeliosInterfaceFactoryAsync, string, object>(DoReceiveAvailableInstance),
                DispatcherPriority.Input,
                factory, displayName, context);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            foreach (IHeliosInterfaceFactoryAsync asyncFactory in _asyncFactories)
            {
                asyncFactory.StopDiscoveringInterfaces();
            }

            _asyncFactories.Clear();
        }

        #endregion

        #region Properties

        /// <summary>
        /// the list of interfaces that can be added, may change as we scan for more
        /// </summary>
        public ObservableCollection<Item> Items
        {
            get => (ObservableCollection<Item>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }


        /// <summary>
        /// the currently selected interface object from the list or null
        /// </summary>
        public Item SelectedInterface
        {
            get => (Item)GetValue(SelectedInterfaceProperty);
            set => SetValue(SelectedInterfaceProperty, value);
        }

        /// <summary>
        /// true if the add action should be enabled
        /// </summary>
        public bool CanAdd
        {
            get => (bool)GetValue(CanAddProperty);
            set => SetValue(CanAddProperty, value);
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<Item>),
                typeof(AvailableInterfaces), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedInterfaceProperty =
            DependencyProperty.Register("SelectedInterface", typeof(Item), typeof(AvailableInterfaces),
                new PropertyMetadata(null, SelectedInterfaceChanged));

        public static readonly DependencyProperty CanAddProperty =
            DependencyProperty.Register("CanAdd", typeof(bool), typeof(AvailableInterfaces),
                new PropertyMetadata(false));

        #endregion

        /// <summary>
        /// an item that still needs to be created if we select it
        /// </summary>
        public class DeferredItem : Item
        {
            private readonly IHeliosInterfaceFactoryAsync _factory;
            private readonly object _context;

            public DeferredItem(string name, IHeliosInterfaceFactoryAsync factory, object context)
                : base(name)
            {
                _factory = factory;
                _context = context;
            }

            #region Overrides

            public override HeliosInterface HeliosInterface => _factory.CreateInstance(_context);

            #endregion
        }

        /// <summary>
        /// an item that already has an interface instance
        /// </summary>
        public class InterfaceItem : Item
        {
            public InterfaceItem(HeliosInterface heliosInterface)
                : base(heliosInterface.Name)
            {
                HeliosInterface = heliosInterface;
            }

            #region Overrides

            public override HeliosInterface HeliosInterface { get; }

            #endregion
        }

        /// <summary>
        /// items to choose in the list of available interfaces
        /// </summary>
        public abstract class Item
        {
            protected Item(string name)
            {
                Name = name;
            }

            #region Properties

            public string Name { get; }
            public abstract HeliosInterface HeliosInterface { get; }

            #endregion
        }
    }
}