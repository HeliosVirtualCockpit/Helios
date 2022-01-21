using GadrocsWorkshop.Helios.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GadrocsWorkshop.Helios.Interfaces.HeliosMacroBoard
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MacroBoardButtonPropertiesDialog : DialogWindow
    {
        private MacroBoardButton _targetButton;
        private MacroBoardButton _pendingButton;

        public MacroBoardButton TargetButton
        {
            get => _targetButton;
            set
            {
                _targetButton = value;
                _pendingButton.Row = _targetButton.Row;
                _pendingButton.Column = _targetButton.Column;
                _pendingButton.Text = _targetButton.Text;
                _pendingButton.BackgroundImageUri = _targetButton.BackgroundImageUri;
            }
        }

        public MacroBoardButton PendingButton
        {
            get => _pendingButton;
        }

        public MacroBoardButtonPropertiesDialog(MacroBoardButton targetButton)
        {
            _pendingButton = new MacroBoardButton(targetButton.Row, targetButton.Column, targetButton.Text, targetButton.BackgroundImageUri);
            TargetButton = targetButton;
            InitializeComponent();
        }

        private void BackgroundImageBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();

            openDialog.Multiselect = false;
            openDialog.Filter = "Image Files (*.png;*.bmp;*.jpg;*.jpeg;*.gif)|*.png;*.bmp;*.jpg;*.jpeg;*.gif";
            openDialog.InitialDirectory = ConfigManager.ImagePath;

            if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PendingButton.BackgroundImageUri = openDialog.FileName;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            TargetButton.BackgroundImageUri = PendingButton.BackgroundImageUri;
            TargetButton.Text = PendingButton.Text;
            TargetButton.BackgroundImageEnabled = PendingButton.BackgroundImageEnabled;

            DialogResult = true;

            Close();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            TargetButton.BackgroundImageUri = PendingButton.BackgroundImageUri;
            TargetButton.Text = PendingButton.Text;
            TargetButton.BackgroundImageEnabled = PendingButton.BackgroundImageEnabled;
        }
    }
}