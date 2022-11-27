//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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
    using GadrocsWorkshop.Helios.ProfileEditor.UndoEvents;
    using GadrocsWorkshop.Helios.ProfileEditor.ViewModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    public enum BindingPanelType
    {
        Input,
        Output
    }

    /// <summary>
    /// Interaction logic for InputBindingsPanel.xaml
    /// </summary>
    public partial class BindingsPanel : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private HeliosProfile _actionTriggerProfile = null;
        private ProfileExplorerTreeItem _actionsList = null;
        private ProfileExplorerTreeItem _triggerList = null;

        static BindingsPanel()
        {
            Type ownerType = typeof(BindingsPanel);

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_CanExecute));
        }

        public BindingsPanel()
        {
            DataContext = this;
            InitializeComponent();

            ProfileExplorerItems = new ProfileExplorerTreeItemCollection();
            LoadBindings();
            LoadSources();
        }

        #region Properties

        public BindingPanelType BindingType
        {
            get { return (BindingPanelType)GetValue(BindingTypeProperty); }
            set { SetValue(BindingTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindingType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingTypeProperty =
            DependencyProperty.Register("BindingType", typeof(BindingPanelType), typeof(BindingsPanel), new PropertyMetadata(BindingPanelType.Input, OnBindingTypeChanged));


        public ProfileExplorerTreeItemCollection ProfileExplorerItems
        {
            get { return (ProfileExplorerTreeItemCollection)GetValue(ProfileExplorerItemsProperty); }
            set { SetValue(ProfileExplorerItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProfileExplorerItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProfileExplorerItemsProperty =
            DependencyProperty.Register("ProfileExplorerItems", typeof(ProfileExplorerTreeItemCollection), typeof(BindingsPanel), new PropertyMetadata(null));

        public ProfileExplorerTreeItemCollection BindingItems
        {
            get { return (ProfileExplorerTreeItemCollection)GetValue(BindingItemsProperty); }
            set { SetValue(BindingItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BindingItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingItemsProperty =
            DependencyProperty.Register("BindingItems", typeof(ProfileExplorerTreeItemCollection), typeof(BindingsPanel), new PropertyMetadata(null));

        public HeliosObject BindingObject
        {
            get { return (HeliosObject)GetValue(BindingObjectProperty); }
            set { SetValue(BindingObjectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Profile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BindingObjectProperty =
            DependencyProperty.Register("BindingObject", typeof(HeliosObject), typeof(BindingsPanel), new PropertyMetadata(null, OnBindingFocusChanged));

        public HeliosBinding SelectedBinding
        {
            get { return (HeliosBinding)GetValue(SelectedBindingProperty); }
            set { SetValue(SelectedBindingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedBinding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedBindingProperty =
            DependencyProperty.Register("SelectedBinding", typeof(HeliosBinding), typeof(BindingsPanel), new PropertyMetadata(null));

        public StaticValueEditor ValueEditor
        {
            get { return (StaticValueEditor)GetValue(ValueEditorProperty); }
            set { SetValue(ValueEditorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueEditor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueEditorProperty =
            DependencyProperty.Register("ValueEditor", typeof(StaticValueEditor), typeof(BindingsPanel), new PropertyMetadata(null));

        public bool BMSFalconPathExists
        {
            get => ConfigManager.BMSFalconPath.Length > 0;
        }

        #endregion

        private static void OnBindingFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            BindingsPanel p = d as BindingsPanel;
            p.LoadBindings();
            p.LoadSources();
        }

        private static void OnBindingTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            BindingsPanel p = d as BindingsPanel;
            p.LoadBindings();
            p.LoadSources();
        }

        public void Clear()
        {
            ClearBindings();
            ProfileExplorerItems = null;
            ClearSources();
        }

        private void LoadBindings()
        {
            if (BindingObject != null)
            {
                ClearBindings();
                BindingItems = new ProfileExplorerTreeItemCollection();

                if (BindingType == BindingPanelType.Input)
                {
                    ProfileExplorerTreeItem inputBindings = new ProfileExplorerTreeItem(BindingObject, ProfileExplorerTreeItemType.Action | ProfileExplorerTreeItemType.Binding);
                    if (inputBindings.HasChildren)
                    {
                        foreach (ProfileExplorerTreeItem item in inputBindings.Children)
                        {
                            BindingItems.Add(item);
                        }
                    }
                }
                else if (BindingType == BindingPanelType.Output)
                {
                    ProfileExplorerTreeItem outputBindings = new ProfileExplorerTreeItem(BindingObject, ProfileExplorerTreeItemType.Trigger | ProfileExplorerTreeItemType.Binding);
                    if (outputBindings.HasChildren)
                    {
                        foreach (ProfileExplorerTreeItem item in outputBindings.Children)
                        {
                            BindingItems.Add(item);
                        }
                    }
                }
            }
            else
            {
                BindingItems = null;
            }
        }

        private void ClearBindings()
        {
            if (BindingItems != null)
            {
                BindingItems.Disconnect();
                BindingItems = null;
            }
        }

        private void LoadSources()
        {
            ProfileExplorerTreeItemType triggerTypes = ProfileExplorerTreeItemType.Interface | ProfileExplorerTreeItemType.Monitor | ProfileExplorerTreeItemType.Panel | ProfileExplorerTreeItemType.Visual | ProfileExplorerTreeItemType.Trigger;
            ProfileExplorerTreeItemType actionTypes = ProfileExplorerTreeItemType.Interface | ProfileExplorerTreeItemType.Monitor | ProfileExplorerTreeItemType.Panel | ProfileExplorerTreeItemType.Visual | ProfileExplorerTreeItemType.Action;

            if (BindingObject != null)
            {
                if (BindingObject.Profile != _actionTriggerProfile)
                {
                    ClearSources();
                    HeliosProfile newProfile = BindingObject.Profile;
                    if (newProfile != null)
                    {
                        Logger.Debug("loading actions for bindings");
                        _actionsList = new ProfileExplorerTreeItem(newProfile, actionTypes);
                        Logger.Debug("loading triggers for bindings");
                        _triggerList = new ProfileExplorerTreeItem(newProfile, triggerTypes);
                    }
                    _actionTriggerProfile = newProfile;
                }
            }

            if (BindingType == BindingPanelType.Input)
            {
                if (_triggerList == null)
                {
                    ProfileExplorerItems = null;
                }
                else
                {
                    ProfileExplorerItems = _triggerList.Children;
                }
            }
            else if (BindingType == BindingPanelType.Output)
            {
                if (_actionsList == null)
                {
                    ProfileExplorerItems = null;
                }
                else
                {
                    ProfileExplorerItems = _actionsList.Children;
                }
            }
        }

        private void ClearSources()
        {
            if (_actionsList != null)
            {
                _actionsList.Disconnect();
                _actionsList = null;
            }

            if (_triggerList != null)
            {
                _triggerList.Disconnect();
                _triggerList = null;
            }
        }

        private void UpdateValueEditor()
        {
            StaticValueEditor editor = null;

            if (BindingObject != null && SelectedBinding != null && SelectedBinding.ValueSource != BindingValueSources.TriggerValue)
            {
                if (SelectedBinding.Action.ValueEditorType == null || SelectedBinding.ValueSource == BindingValueSources.LuaScript)
                {
                    editor = new TextStaticEditor();
                }
                else
                {
                    editor = (StaticValueEditor)Activator.CreateInstance(SelectedBinding.Action.ValueEditorType);
                }

                Binding bind = new Binding("SelectedBinding.Value");
                editor.Profile = BindingObject.Profile;
                editor.StaticValue = SelectedBinding.Value;

                bind.Source = this;
                bind.Mode = BindingMode.TwoWay;
                bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                editor.SetBinding(StaticValueEditor.StaticValueProperty, bind);
            }

            ValueEditor = editor;
        }

        private void BindingsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ProfileExplorerTreeItem selectedItem = e.NewValue as ProfileExplorerTreeItem;
            if (selectedItem != null && selectedItem.ItemType.HasFlag(ProfileExplorerTreeItemType.Binding))
            {
                SelectedBinding = (HeliosBinding)selectedItem.ContextItem;
                UpdateValueEditor();
            }
            else
            {
                SelectedBinding = null;
            }
        }

        private void ValueSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateValueEditor();
        }

        #region Command Handlers

        private static void Delete_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            BindingsPanel editor = target as BindingsPanel;
            HeliosBinding binding = e.Parameter as HeliosBinding;
            if (editor != null)
            {
                e.Handled = true;
                e.CanExecute = binding != null || editor.SelectedBinding != null;
            }
        }

        private static void Delete_Executed(object target, ExecutedRoutedEventArgs e)
        {
            BindingsPanel editor = target as BindingsPanel;
            HeliosBinding binding = e.Parameter as HeliosBinding;
            if (editor != null)
            {
                BindingDeleteUndoEvent undo;
                if (binding != null)
                {
                    undo = new BindingDeleteUndoEvent(binding);
                }
                else
                {
                    undo = new BindingDeleteUndoEvent(editor.SelectedBinding);
                }
                undo.Do();
                ConfigManager.UndoManager.AddUndoItem(undo);
                e.Handled = true;
            }
        }

        #endregion

    }
}
