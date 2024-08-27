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

namespace GadrocsWorkshop.Helios.Windows.Controls
{
    using GadrocsWorkshop.Helios;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using TextDecorations = GadrocsWorkshop.Helios.TextDecorations;

    /// <summary>
    /// Interaction logic for TextFormatButton.xaml
    /// </summary>
    public partial class TextFormatButton : UserControl
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public TextFormatButton()
        {
            InitializeComponent();
        }

        #region Properties

        public TextFormat TextFormat
        {
            get { return (TextFormat)GetValue(TextFormatProperty); }
            set { SetValue(TextFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextFormatProperty =
            DependencyProperty.Register("TextFormat", typeof(TextFormat), typeof(TextFormatButton), new UIPropertyMetadata(new TextFormat()));

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FontChooserDialog dialog = new FontChooserDialog();
                dialog.SelectedFamily = TextFormat.FontFamily;
                dialog.SelectedTypeface = new Typeface(TextFormat.FontFamily, TextFormat.FontStyle, TextFormat.FontWeight, FontStretches.Normal);
                dialog.SelectedSize = TextFormat.FontSize;

                if (TextFormat.Decorations.HasFlag(TextDecorations.Underline))
                {
                    dialog.IsUnderline = true;
                }
                if (TextFormat.Decorations.HasFlag(TextDecorations.Strikethrough))
                {
                    dialog.IsStrikethrough = true;
                }
                if (TextFormat.Decorations.HasFlag(TextDecorations.Baseline))
                {
                    dialog.IsBaseline = true;
                }
                if (TextFormat.Decorations.HasFlag(TextDecorations.OverLine))
                {
                    dialog.IsOverLine = true;
                }
                

                dialog.Owner = Window.GetWindow(this);
                bool? results = dialog.ShowDialog();
                if (results != null && results == true)
                {
                    ConfigManager.UndoManager.StartBatch();
                    TextFormat.FontFamily = dialog.SelectedTypeface.FontFamily;
                    TextFormat.FontStyle = dialog.SelectedTypeface.Style;
                    TextFormat.FontWeight = dialog.SelectedTypeface.Weight;
                    TextFormat.FontStretch = dialog.SelectedTypeface.Stretch;
                    TextFormat.FontSize = dialog.SelectedSize;
                    TextFormat.ConfiguredFontSize = dialog.SelectedSize;

                    TextDecorations newDecorations = 0;
                    if (dialog.IsUnderline)
                    {
                        newDecorations |= TextDecorations.Underline;
                    }
                    if (dialog.IsStrikethrough)
                    {
                        newDecorations |= TextDecorations.Strikethrough;
                    }
                    if (dialog.IsBaseline)
                    {
                        newDecorations |= TextDecorations.Baseline;
                    }
                    if (dialog.IsOverLine)
                    {
                        newDecorations |= TextDecorations.OverLine;
                    }
                    TextFormat.Decorations = newDecorations;

                    ConfigManager.UndoManager.CloseBatch();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error opening text format editor.");
            }
        }
    }
}
