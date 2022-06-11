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

namespace GadrocsWorkshop.Helios.Controls
{
    using GadrocsWorkshop.Helios.ComponentModel;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for DualIndicatorPushButtonAppearanceEditor.xaml
    /// </summary>
    [HeliosPropertyEditor("Helios.Base.DualIndicatorPushButton", "Appearance")]
    public partial class DualIndicatorPushButtonAppearanceEditor : HeliosPropertyEditor
    {
        public DualIndicatorPushButtonAppearanceEditor()
        {
            InitializeComponent();
        }

        private void TurnIndicatorOn(object sender, RoutedEventArgs e)
        {
            var caller = sender as Control;
            string tag = caller.Tag.ToString();
            DualIndicatorPushButton indicator = Control as DualIndicatorPushButton;
            if (tag.Length >= 10 && indicator != null)
            {
                if (tag.Substring(0,7) == "Primary")
                {
                    indicator.Indicator = true;
                } else if(tag.Substring(0,10) == "Additional")
                {
                    indicator.AdditionalIndicator = true;
                }
                else { }
            }
        }

        private void TurnIndicatorOff(object sender, RoutedEventArgs e)
        {
            var caller = sender as Control;
            string tag = caller.Tag.ToString();

            DualIndicatorPushButton indicator = Control as DualIndicatorPushButton;
            if (tag.Length >= 10 && indicator != null)
            {
                if (tag.Substring(0,7
                    ) == "Primary")
                {
                    indicator.Indicator = false;
                }
                else if (tag.Substring(0,10) == "Additional")
                {
                    indicator.AdditionalIndicator = false;
                }
                else { }
            }
        }

        private void LeftPaddingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (!System.Windows.Input.Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && slider != null && slider.IsFocused)
            {
                DualIndicatorPushButton indicator = Control as DualIndicatorPushButton;
                if (indicator != null && slider.Tag.ToString() == "Primary")
                {
                    indicator.TextFormat.PaddingRight = indicator.TextFormat.PaddingLeft;
                }
                else if (indicator != null && slider.Tag.ToString() == "Additional")
                {
                    indicator.AdditionalTextFormat.PaddingRight = indicator.AdditionalTextFormat.PaddingLeft;
                }
                else { }
            }
        }

        private void RightPaddingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (!System.Windows.Input.Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && slider != null && slider.IsFocused)
            {
                DualIndicatorPushButton indicator = Control as DualIndicatorPushButton;
                if (indicator != null && slider.Tag.ToString() == "Primary")
                {
                    indicator.TextFormat.PaddingLeft = indicator.TextFormat.PaddingRight;
                }
                else if (indicator != null && slider.Tag.ToString() == "Additional")
                {
                    indicator.AdditionalTextFormat.PaddingLeft = indicator.AdditionalTextFormat.PaddingRight;
                }
                else { }
            }
        }

        private void TopPaddingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (!System.Windows.Input.Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && slider != null && slider.IsFocused)
            {
                DualIndicatorPushButton indicator = Control as DualIndicatorPushButton;
                if (indicator != null && slider.Tag.ToString() == "Primary")
                {
                    indicator.TextFormat.PaddingBottom = indicator.TextFormat.PaddingTop;
                }
                else if (indicator != null && slider.Tag.ToString() == "Additional")
                {
                    indicator.AdditionalTextFormat.PaddingBottom = indicator.AdditionalTextFormat.PaddingTop;
                }
                else { }
            }
        }

        private void BottomPaddingChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (!System.Windows.Input.Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) && slider != null && slider.IsFocused)
            {
                DualIndicatorPushButton indicator = Control as DualIndicatorPushButton;
                if (indicator != null && slider.Tag.ToString() == "Primary")
                {
                    indicator.TextFormat.PaddingTop = indicator.TextFormat.PaddingBottom;
                }
                else if (indicator != null && slider.Tag.ToString() == "Additional")
                {
                    indicator.AdditionalTextFormat.PaddingTop = indicator.AdditionalTextFormat.PaddingBottom;
                }
                else { }
            }
        }
    }
}
