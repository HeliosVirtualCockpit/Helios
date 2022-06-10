using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GadrocsWorkshop.Helios.Interfaces.HeliosMacroBoard
{
    [ValueConversion(typeof(bool), typeof(Brush))]
    internal class ButtonStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool buttonPressed = (bool)value;
            if (buttonPressed)
            {
                return Brushes.Red;
            }
            else
            {
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Interaction logic for MacroBoardButtonEditor.xaml
    /// </summary>
    public partial class MacroBoardLayoutEditor : UserControl
    {

        public MacroBoardLayoutEditor()
        {
            InitializeComponent();
        }

        public void ButtonPropertiesMenuItem_Click(object source, RoutedEventArgs e)
        {
            MacroBoardButton targetButton = ((Control)source).DataContext as MacroBoardButton;
            MacroBoardButtonPropertiesDialog buttonPropertiesDialog = new MacroBoardButtonPropertiesDialog(targetButton);
            buttonPropertiesDialog.TargetButton = targetButton;

            buttonPropertiesDialog.ShowDialog();
        }

        public void ButtonPreview_MouseUp(object source, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                MacroBoardButton targetButton = ((Control)source).DataContext as MacroBoardButton;
                MacroBoardButtonPropertiesDialog buttonPropertiesDialog = new MacroBoardButtonPropertiesDialog(targetButton);
                buttonPropertiesDialog.TargetButton = targetButton;

                buttonPropertiesDialog.ShowDialog();
            }
        }
    }
}