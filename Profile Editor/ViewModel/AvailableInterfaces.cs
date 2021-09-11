﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;

namespace GadrocsWorkshop.Helios.ProfileEditor.ViewModel
{
    internal class AvailableInterfaces : DependencyObject, IAvailableInterfaces, IDisposable
    {
        /// <summary>
        /// items to choose in the list of available interfaces
        /// </summary>
        public abstract class Item
        {
            protected Item(string name)
            {
                Name = name;
            }

            public string Name { get; }
            public abstract HeliosInterface HeliosInterface { get; }
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

            public override HeliosInterface HeliosInterface { get; }
        }

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

            public override HeliosInterface HeliosInterface => _factory.CreateInstance(_context);
        }

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
                        ConfigManager.LogManager.LogInfo("Adding " + newInterface.Name + " Type: " +
                                                         descriptor.InterfaceType.BaseType.Name +
                                                         " to add interface list.");
                        Items.Add(new InterfaceItem(newInterface));
                    }
                }
                catch (Exception e)
                {
                    ConfigManager.LogManager.LogError(
                        "Error trying to get available instances for " + descriptor.Name + " interface.", e);
                    throw;
                }
            }
        }

        /// <summary>
        /// the list of interfaces that can be added, may change as we scan for more
        /// </summary>
        public ObservableCollection<Item> Items
        {
            get => (ObservableCollection<Item>) GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        } 
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<Item>),
                typeof(AvailableInterfaces), new PropertyMetadata(null));

        /// <summary>
        /// the currently selected interface object from the list or null
        /// </summary>
        public Item SelectedInterface
        {
            get => (Item) GetValue(SelectedInterfaceProperty);
            set => SetValue(SelectedInterfaceProperty, value);
        }

        public static readonly DependencyProperty SelectedInterfaceProperty =
            DependencyProperty.Register("SelectedInterface", typeof(Item), typeof(AvailableInterfaces),
                new PropertyMetadata(null, SelectedInterfaceChanged));

        /// <summary>
        /// true if the add action should be enabled
        /// </summary>
        public bool CanAdd
        {
            get => (bool) GetValue(CanAddProperty);
            set => SetValue(CanAddProperty, value);
        }

        public static readonly DependencyProperty CanAddProperty =
            DependencyProperty.Register("CanAdd", typeof(bool), typeof(AvailableInterfaces),
                new PropertyMetadata(false));

        /// <summary>
        /// factories that may be working on our behalf to find new interface instances, so we shut them down when
        /// we are done
        /// </summary>
        private HashSet<IHeliosInterfaceFactoryAsync> _asyncFactories = new HashSet<IHeliosInterfaceFactoryAsync>();

        private static void SelectedInterfaceChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ((AvailableInterfaces) d).CanAdd = args.NewValue != null;
        }

        public void ReceiveAvailableInstance(IHeliosInterfaceFactoryAsync factory, string displayName, object context)
        {
            Application.Current?.Dispatcher.BeginInvoke(
                new Action<IHeliosInterfaceFactoryAsync, string, object>(DoReceiveAvailableInstance),
                DispatcherPriority.Input,
                factory, displayName, context);
        }

        private void DoReceiveAvailableInstance(IHeliosInterfaceFactoryAsync factory, string displayName,
            object context)
        {
            Items.Add(new DeferredItem(displayName, factory, context));
        }

        public void Dispose()
        {
            foreach (IHeliosInterfaceFactoryAsync asyncFactory in _asyncFactories)
            {
                asyncFactory.StopDiscoveringInterfaces();
            }
            _asyncFactories.Clear();
        }
    }
}