//  Copyright 2014 Craig Courtney
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


namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for RotarySwitchBehaviorEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.RotarySwitch", "Behavior")]
    public partial class RotarySwitchBehaviorEditor : HeliosPropertyEditor
    {
        public RotarySwitchBehaviorEditor()
        {
            InitializeComponent();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == ControlProperty)
            {
                RotarySwitch rotary = Control as RotarySwitch;
                if (rotary != null && rotary.DefaultPosition > 0)
                {
                    DefaultPositionCombo.SelectedIndex = rotary.DefaultPosition - 1;
                }
            }
            base.OnPropertyChanged(e);
        }

        private void Position_GotFocus(object sender, RoutedEventArgs e)
        {
            RotarySwitch rotary = Control as RotarySwitch;
            FrameworkElement senderControl = sender as FrameworkElement;
            if (senderControl != null && rotary != null)
            {
                int index = rotary.Positions.IndexOf((RotarySwitchPosition)senderControl.Tag);
                rotary.CurrentPosition = index+1;
            }
        }

        private void Add_Position_Click(object sender, RoutedEventArgs e)
        {
            RotarySwitch rotary = Control as RotarySwitch;
            if (rotary != null)
            {
                RotarySwitchPosition position = new RotarySwitchPosition(rotary, rotary.Positions.Count + 1, rotary.Positions.Count.ToString(CultureInfo.InvariantCulture), 0d);              
                rotary.Positions.Add(position);
                ConfigManager.UndoManager.AddUndoItem(new RotarySwitchAddPositionUndoEvent(rotary, position));
            }
        }

        private void DeletePosition_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            RotarySwitch rotary = Control as RotarySwitch;
            if (rotary != null)
            {
                e.CanExecute = (rotary.Positions.Count > 2);
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void DeletePosition_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RotarySwitch rotary = Control as RotarySwitch;
            if (rotary != null && rotary.Positions.Contains((RotarySwitchPosition)PositionList.SelectedItem))
            {
                RotarySwitchPosition removedPosition = (RotarySwitchPosition)PositionList.SelectedItem;
                int index = rotary.Positions.IndexOf(removedPosition);
                rotary.Positions.Remove(removedPosition);
                ConfigManager.UndoManager.AddUndoItem(new RotarySwitchDeletePositionUndoEvent(rotary, removedPosition, index));
            }        
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RotarySwitch rotary = Control as RotarySwitch;
            if (rotary != null)
            {
                rotary.CurrentPosition = PositionList.SelectedIndex + 1;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RotarySwitch rotary = Control as RotarySwitch;
            if (rotary != null)
            {
                rotary.DefaultPosition = DefaultPositionCombo.SelectedIndex + 1;
            }
        }

        private void Delete_Position_Click(object sender, RoutedEventArgs e)
        {
            RotarySwitch rotary = Control as RotarySwitch;
            FrameworkElement senderControl = sender as FrameworkElement;
            if (senderControl != null && rotary != null)
            {
                RotarySwitchPosition position = senderControl.Tag as RotarySwitchPosition;
                if (position != null && rotary.Positions.Contains(position))
                {
                    rotary.Positions.Remove(position);
                }
            }
        }
    }
}
