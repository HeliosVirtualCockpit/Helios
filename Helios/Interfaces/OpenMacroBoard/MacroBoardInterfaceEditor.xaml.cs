using System;
using System.Collections.Generic;
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

namespace GadrocsWorkshop.Helios.Interfaces.OpenMacroBoard
{

    using GadrocsWorkshop.Helios.Windows.Controls;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Text.RegularExpressions;

    [ValueConversion(typeof(MacroBoardModel), typeof(string))]
    public class MacroBoardModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var model = (MacroBoardModel)value;
            switch (model)
            {
                case MacroBoardModel.StreamDeck:
                    {
                        return "Stream Deck";
                    }
                case MacroBoardModel.StreamDeckXL:
                    {
                        return "Stream Deck XL";
                    }
                default:
                    {
                        return "";
                    }
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = (string)value;
            switch (strValue)
            {
                case "Stream Deck":
                    {
                        return MacroBoardModel.StreamDeck;
                    }
                case "Stream Deck XL":
                    {
                        return MacroBoardModel.StreamDeckXL;
                    }
                default:
                    return null;

            }
        }
    }

    [ValueConversion(typeof(IEnumerable<MacroBoardModel>), typeof(IEnumerable<string>))]
    public class MacroBoardModelValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var models = (IEnumerable<MacroBoardModel>)value;
            return models.Select((item) => new MacroBoardModelConverter().Convert(item, typeof(string), parameter, culture));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Interaction logic for SteamDeckInterfaceEditor.xaml
    /// </summary>
    public partial class MacroBoardInterfaceEditor : HeliosInterfaceEditor
    {

        private void ValidateBrightnessTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex r = new Regex("^[0-9]*$");

            if (!r.IsMatch(e.Text)) 
            {
                e.Handled = true;
            }

            if (int.Parse(e.Text) > 100)
            {
                e.Handled = true;
            }
        }
        public MacroBoardInterfaceEditor()
        {
            InitializeComponent();
        }
    }
}
