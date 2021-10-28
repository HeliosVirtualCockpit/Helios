//  Copyright 2014 Craig Courtney
//  Copyright 2020 Ammo Goettsch
//  Copyright 2021 Helios Contributors
//
//  Helios is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Helios is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace GadrocsWorkshop.Helios.ProfileEditor
{
    using GadrocsWorkshop.Helios;
    using GadrocsWorkshop.Helios.ProfileEditor.UndoEvents;
    using GadrocsWorkshop.Helios.ProfileEditor.ViewModel;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class ItemDeleteEventArgs : EventArgs
    {
        public ItemDeleteEventArgs(HeliosObject item)
        {
            DeletedItem = item;
        }

        public HeliosObject DeletedItem { get; }
    }

    /// <summary>
    /// Interaction logic for ProjectExplorerPanel.xaml
    /// </summary>
    public partial class ProfileExplorerPanel : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public ProfileExplorerPanel()
        {
            DataContext = this;
            ProfileExplorerItems = new ProfileExplorerTreeItemCollection();
            InitializeComponent();
            ProfileExplorerTree.SelectedItemChanged += ProfileExplorerTree_SelectedItemChanged;
        }

        public event EventHandler<ItemDeleteEventArgs> ItemDeleting;

        #region Properties

        public ProfileExplorerTreeItemCollection ProfileExplorerItems
        {
            get { return (ProfileExplorerTreeItemCollection)GetValue(ProfileExplorerItemsProperty); }
            set { SetValue(ProfileExplorerItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProfileExplorerItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProfileExplorerItemsProperty =
            DependencyProperty.Register("ProfileExplorerItems", typeof(ProfileExplorerTreeItemCollection), typeof(ProfileExplorerPanel), new PropertyMetadata(null));

        public HeliosProfile Profile
        {
            get { return (HeliosProfile)GetValue(ProfileProperty); }
            set { SetValue(ProfileProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Profile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProfileProperty =
            DependencyProperty.Register("Profile", typeof(HeliosProfile), typeof(ProfileExplorerPanel), new PropertyMetadata(null, new PropertyChangedCallback(OnItemReload)));

        #endregion

        private void LoadItems()
        {
            Logger.Debug("loading items for profile explorer");

            ProfileExplorerItems.Disconnect();
            ProfileExplorerItems.Clear();
            if (Profile == null)
            {
                return;
            }

            ProfileExplorerTreeItemType types = ProfileExplorerTreeItemType.Interface | ProfileExplorerTreeItemType.Monitor | ProfileExplorerTreeItemType.Panel;
            ProfileExplorerTreeItem item = new ProfileExplorerTreeItem(Profile, types);
            item.ExpandAll();
            ProfileExplorerItems = item.Children;

            ButtonBranchExpand.IsEnabled = false;
            ButtonBranchCollapse.IsEnabled = false;
            ButtonItemOpen.IsEnabled = false;
        }

        private static void OnItemReload(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            ProfileExplorerPanel p = d as ProfileExplorerPanel;
            p.LoadItems();
        }

        private void ProfileExplorerTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (ProfileExplorerTree.SelectedItem is ProfileExplorerTreeItem item)
            {
                ButtonBranchExpand.IsEnabled = item.HasChildren;
                ButtonBranchCollapse.IsEnabled = item.HasChildren;
                ButtonItemOpen.IsEnabled = item.ItemType.HasFlag(ProfileExplorerTreeItemType.Panel) ||
                                             item.ItemType.HasFlag(ProfileExplorerTreeItemType.Monitor) ||
                                             item.ItemType.HasFlag(ProfileExplorerTreeItemType.Interface);
            }
        }

        private void ButtonBranchExpand_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileExplorerTree.SelectedItem is ProfileExplorerTreeItem item)
            {
                item.ExpandAll();
            }
        }

        private void ButtonBranchCollapse_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileExplorerTree.SelectedItem is ProfileExplorerTreeItem item)
            {
                item.CollapseAll();
            }
        }

        private void ButtonItemOpen_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileExplorerTree.SelectedItem is ProfileExplorerTreeItem item &&
                (item.ItemType.HasFlag(ProfileExplorerTreeItemType.Panel) ||
                 item.ItemType.HasFlag(ProfileExplorerTreeItemType.Monitor) ||
                 item.ItemType.HasFlag(ProfileExplorerTreeItemType.Interface)))
            {
                ProfileEditorCommands.OpenProfileItem.Execute(item.ContextItem, this);
            }
        }

        private void TreeView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProfileExplorerTree.SelectedItem is ProfileExplorerTreeItem item &&
                (item.ItemType.HasFlag(ProfileExplorerTreeItemType.Panel) ||
                 item.ItemType.HasFlag(ProfileExplorerTreeItemType.Monitor) ||
                 item.ItemType.HasFlag(ProfileExplorerTreeItemType.Interface)))
            {
                ProfileEditorCommands.OpenProfileItem.Execute(item.ContextItem, this);
            }
            e.Handled = true;
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ProfileExplorerTreeItem item = ProfileExplorerTree.SelectedItem as ProfileExplorerTreeItem;
            e.CanExecute = (item != null &&
                            (item.ItemType.HasFlag(ProfileExplorerTreeItemType.Panel) ||
                            item.ItemType.HasFlag(ProfileExplorerTreeItemType.Visual) ||
                            item.ItemType.HasFlag(ProfileExplorerTreeItemType.Interface)));
        }

        private void Delete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(ProfileExplorerTree.SelectedItem is ProfileExplorerTreeItem item))
            {
                return;
            }

            if (item.ItemType.HasFlag(ProfileExplorerTreeItemType.Panel) ||
                item.ItemType.HasFlag(ProfileExplorerTreeItemType.Visual))
            {
                HeliosVisual visual = item.ContextItem as HeliosVisual;
                if (!(visual?.Parent is HeliosVisualContainer container))
                {
                    return;
                }

                ConfigManager.UndoManager.AddUndoItem(new ControlDeleteUndoEvent(container, new List<HeliosVisual> { visual }, new List<int> { container.Children.IndexOf(visual) } ));
                OnDeleting(visual);
                container.Children.Remove(visual);
            }
            else if (item.ItemType.HasFlag(ProfileExplorerTreeItemType.Interface))
            {
                if (item.ContextItem is HeliosInterface deletedInterface)
                {
                    DeleteInterface(deletedInterface);
                }
            }
        }

        public void DeleteInterface(HeliosInterface deletedInterface)
        {
            if (MessageBox.Show("Are you sure you want to remove the " + deletedInterface.Name +
                " interface from the profile?  This will remove all bindings associated with this interface.",
                "Remove Interface", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No,
                MessageBoxOptions.None) != MessageBoxResult.Yes)
            {
                return;
            }

            // also must delete all child interfaces.  we do it in such an order that children get deleted before their parents
            Stack<HeliosInterface> descendants = new Stack<HeliosInterface>();

            // brute force find them all
            descendants.Push(deletedInterface);
            FindAllDescendants(descendants, Profile, deletedInterface);

            ConfigManager.UndoManager.AddUndoItem(new InterfaceDeleteUndoEvent(Profile, descendants));
            foreach (HeliosInterface heliosInterface in descendants)
            {
                OnDeleting(heliosInterface);
                Profile.Interfaces.Remove(heliosInterface);
            }
        }

        private static void FindAllDescendants(Stack<HeliosInterface> descendants, HeliosProfile profile, HeliosInterface parent)
        {
            foreach(HeliosInterface heliosInterface in profile.Interfaces)
            {
                if (heliosInterface.ParentInterface != parent)
                {
                    continue;
                }

                // check for infinite loop in Debug
                Debug.Assert(!descendants.Contains(heliosInterface));
                descendants.Push(heliosInterface);

                // recurse
                FindAllDescendants(descendants, profile, heliosInterface);
            }
        }

        public void OnDeleting(HeliosObject item)
        {
            ItemDeleting?.Invoke(this, new ItemDeleteEventArgs(item));
        }
    }
}
