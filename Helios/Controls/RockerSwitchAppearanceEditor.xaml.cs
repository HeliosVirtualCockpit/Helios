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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for RockerSwitchAppearanceEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.RockerSwitch", "Appearance")]
    public partial class RockerSwitchAppearanceEditor : HeliosPropertyEditor
    {
        public RockerSwitchAppearanceEditor()
        {
            InitializeComponent();
        }
        private void LeftPaddingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (!System.Windows.Input.Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && slider != null && slider.IsFocused)
            {
                PushButton indicator = Control as PushButton;
                if (indicator != null)
                {
                    indicator.TextFormat.PaddingRight = indicator.TextFormat.PaddingLeft;
                }
            }
        }

        private void RightPaddingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (!System.Windows.Input.Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && slider != null && slider.IsFocused)
            {
                PushButton indicator = Control as PushButton;
                if (indicator != null)
                {
                    indicator.TextFormat.PaddingLeft = indicator.TextFormat.PaddingRight;
                }
            }
        }

        private void TopPaddingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (!System.Windows.Input.Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && slider != null && slider.IsFocused)
            {
                PushButton indicator = Control as PushButton;
                if (indicator != null)
                {
                    indicator.TextFormat.PaddingBottom = indicator.TextFormat.PaddingTop;
                }
            }
        }

        private void BottomPaddingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (!System.Windows.Input.Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && slider != null && slider.IsFocused)
            {
                PushButton indicator = Control as PushButton;
                if (indicator != null)
                {
                    indicator.TextFormat.PaddingTop = indicator.TextFormat.PaddingBottom;
                }
            }
        }
    }
}
