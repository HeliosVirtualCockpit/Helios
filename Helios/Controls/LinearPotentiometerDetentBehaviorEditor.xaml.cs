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
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for LinearPotentiometerDetentBehaviorEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.LinearPotentiometerDetentsAnimated", "Behavior")]
    public partial class LinearPotentiometerDetentBehaviorEditor : HeliosPropertyEditor
    {
        private double _oldValue;
        private string _oldMinMaxValue;

        public LinearPotentiometerDetentBehaviorEditor()
        {
            InitializeComponent();
        }
        private void Value_Focus(object sender, RoutedEventArgs e)
        {
            LinearPotentiometerDetentsAnimated linearPotentiometer = Control as LinearPotentiometerDetentsAnimated;
            FrameworkElement senderControl = sender as FrameworkElement;
            HeliosTextBox heliosTextBoxControl = e.Source as HeliosTextBox;
            switch (e.RoutedEvent.Name)
            {
                case "GotFocus":
                    _oldMinMaxValue = heliosTextBoxControl.Text;
                    break;
                case "LostFocus":
                    if (heliosTextBoxControl.Text != _oldMinMaxValue) {
                        if(linearPotentiometer != null)
                        {
                            //linearPotentiometer.RemoveDetent(Double.Parse(heliosTextBoxControl.Text, CultureInfo.InvariantCulture));
                            linearPotentiometer.DetentPositions.Sort();
                            DetentPositionList.Items.Refresh();
                        }
                    }
                    break;
            }
        }
        private void Position_Focus(object sender, RoutedEventArgs e)
        {
            LinearPotentiometerDetentsAnimated linearPotentiometer = Control as LinearPotentiometerDetentsAnimated;
            FrameworkElement senderControl = sender as FrameworkElement;

            if (senderControl != null && senderControl.GetType().Name == "HeliosTextBox" && linearPotentiometer != null)
            {
                HeliosTextBox htb = senderControl as HeliosTextBox;
                switch (e.RoutedEvent.Name)
                {
                    case "GotFocus":
                        _oldValue = Double.Parse(htb.Text, CultureInfo.InvariantCulture);
                        senderControl.Tag = _oldValue;

                        int index = linearPotentiometer.DetentPositions.IndexOf((double)senderControl.Tag);
                        linearPotentiometer.CurrentPosition = index + 1;

                        break;
                    case "LostFocus":
                        double newFieldValue = Double.Parse(htb.Text, CultureInfo.InvariantCulture);
                        if (newFieldValue != _oldValue && newFieldValue < linearPotentiometer.MaxValue && newFieldValue > linearPotentiometer.MinValue)
                        {
                            senderControl.Tag = newFieldValue;
                            linearPotentiometer.DetentPositions[linearPotentiometer.DetentPositions.IndexOf(_oldValue)] = newFieldValue;
                            linearPotentiometer.DetentPositions.Sort();
                            DetentPositionList.Items.Refresh();
                        }
                        break;
                }
            }
        }

        private void Add_Position_Click(object sender, RoutedEventArgs e)
        {
            LinearPotentiometerDetentsAnimated linearPotentiometer = Control as LinearPotentiometerDetentsAnimated;
            if (linearPotentiometer != null)
            {
                if (!linearPotentiometer.DetentPositions.Contains(-999d))
                {
                    linearPotentiometer.DetentPositions.Add(-999d);
                    ConfigManager.UndoManager.AddUndoItem(new DetentAddPositionUndoEvent(linearPotentiometer, -999d));
                    linearPotentiometer.DetentPositions.Sort();
                    linearPotentiometer.Triggers.Add(new HeliosTrigger(linearPotentiometer, "", $"detent { linearPotentiometer.DetentPositions.Count }", "holding", "Fires when potentiometer stopped at detent position"));
                    linearPotentiometer.Triggers.Add(new HeliosTrigger(linearPotentiometer, "", $"detent { linearPotentiometer.DetentPositions.Count }", "released", "Fires when potentiometer released from detent position"));
                    DetentPositionList.Items.Refresh();
                }
            }
        }

        private void DeletePosition_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            LinearPotentiometerDetentsAnimated linearPotentiometer = Control as LinearPotentiometerDetentsAnimated;
            if (linearPotentiometer != null)
            {
                e.CanExecute = (linearPotentiometer.DetentPositions.Count > 2);
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void DeletePosition_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LinearPotentiometerDetentsAnimated linearPotentiometer = Control as LinearPotentiometerDetentsAnimated;
            if (linearPotentiometer != null && linearPotentiometer.DetentPositions.Contains((double)DetentPositionList.SelectedItem))
            {
                double removePosition = (double)DetentPositionList.SelectedItem;
                linearPotentiometer.DetentPositions.Remove((double)DetentPositionList.SelectedItem);
                ConfigManager.UndoManager.AddUndoItem(new DetentDeletePositionUndoEvent(linearPotentiometer, removePosition));
                DetentPositionList.Items.Refresh();
            }        
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LinearPotentiometerDetentsAnimated linearPotentiometer = Control as LinearPotentiometerDetentsAnimated;
            if (linearPotentiometer != null)
            {
                linearPotentiometer.CurrentPosition = DetentPositionList.SelectedIndex + 1;
            }
        }

        private void Delete_Position_Click(object sender, RoutedEventArgs e)
        {
            LinearPotentiometerDetentsAnimated linearPotentiometer = Control as LinearPotentiometerDetentsAnimated;
            FrameworkElement senderControl = sender as FrameworkElement;
            if (senderControl != null && linearPotentiometer != null)
            {
                double position = (double)senderControl.Tag;
                linearPotentiometer.DetentPositions.Remove(position);
                linearPotentiometer.Triggers.RemoveAt(linearPotentiometer.Triggers.Count - 1);
                linearPotentiometer.Triggers.RemoveAt(linearPotentiometer.Triggers.Count - 1);
                DetentPositionList.Items.Refresh();
            }
        }
    }
}
