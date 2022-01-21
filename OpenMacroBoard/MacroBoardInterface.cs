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

using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using System;
using System.Globalization;
using System.Xml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using GadrocsWorkshop.Helios.Collections;
using System.Windows.Media.Imaging;

using StreamDeckSharp;
using OpenMacroBoard.SDK;
using System.Windows.Media;
using System.Windows;
using System.IO;
using OpenMacroBoard.VirtualBoard;

namespace GadrocsWorkshop.Helios.Interfaces.OpenMacroBoard
{
    public enum MacroBoardModel
    {
        StreamDeck,
        StreamDeckXL
    }

    public class StreamDeckXLInterface : MacroBoardInterface
    {

        public StreamDeckXLInterface() : base("StreamDeckXLInterface", "Stream Deck XL", 4, 8)
        {

        }
        protected override IMacroBoard OpenDevice()
        {
            return StreamDeck.OpenDevice();
        }
    }

    [HeliosInterface("Helios.Base.VirtualMacroBoardInterface", "OpenMacroBoard Virtual Board", typeof(MacroBoardInterfaceEditor),
            typeof(UniqueHeliosInterfaceFactory), AutoAdd = false)]
    public class VirtualMacroboardInterface : MacroBoardInterface
    {
        public VirtualMacroboardInterface() : base("VirtualMacroboardInterface", "OpenMacroBoard Virtual Board", 4, 8)
        {

        }
        protected override IMacroBoard OpenDevice()
        {
            return BoardFactory.SpawnVirtualBoard(new GridKeyPositionCollection(8, 4, 72, 25));
        }
    }

    [HeliosInterface("Helios.Base.StreamDeckXLInterface", "Stream Deck XL", typeof(MacroBoardInterfaceEditor),
               typeof(UniqueHeliosInterfaceFactory), AutoAdd = false)]
    public abstract class MacroBoardInterface : HeliosInterface, IExtendedDescription
    {
        private ObservableCollection<MacroBoardButton> _buttons;

        private MacroBoardModel _model;
        private string _description;
        private int _rowCount;
        private int _columnCount;
        private byte _brightness;

        public ObservableCollection<MacroBoardButton> DeckButtons
        {
            get => _buttons;
            set => _buttons = value;
        }

        public MacroBoardModel Model
        {
            get => _model;
        }

        public byte Brightness
        {
            get => _brightness;
            set
            {
                var oldValue = _brightness;
                _brightness = value;
                OnPropertyChanged("Brightness", oldValue, value, true);
            }
        }

        private IMacroBoard _board;

        private bool _isDeckConnected;

        public int RowCount
        {
            get => _rowCount;
        }

        public int ColumnCount
        {
            get => _columnCount;
        }

        public bool IsBoardConnected
        {
            get => _isDeckConnected;
            private set
            {
                var oldValue = _isDeckConnected;
                _isDeckConnected = value;
                OnPropertyChanged("IsBoardConnected", oldValue, value, false);
            }
        }

        public string Description
        {
            get => _description;
        }

        public string RemovalNarrative => "Removes the Macro Board interface..";

        private NoResetObservablecollection<IBindingTrigger> _pressedTriggers;
        private NoResetObservablecollection<IBindingTrigger> _releasedTriggers;

        public MacroBoardInterface(string deviceName, string displayName, int rowCount, int columnCount) : base(displayName)
        {
            Brightness = 50;
            _rowCount = rowCount;
            _description = displayName;
            _columnCount = columnCount;
        }

        protected override void AttachToProfileOnMainThread()
        {
            _board = OpenDevice();

            DeckButtons = new ObservableCollection<MacroBoardButton>();
            InitializeTriggers();
            InitializeButtons();
            PropertyChanged += MacroBoardInterface_PropertyChanged;
            _board.KeyStateChanged += Board_KeyStateChanged;
        }

        protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
        {
            _board.KeyStateChanged -= Board_KeyStateChanged;
            PropertyChanged -= MacroBoardInterface_PropertyChanged;
            foreach (MacroBoardButton button in DeckButtons)
            {
                button.PropertyChanged -= StreamDeckInterface_ButtonImageChanged;
            }

            DeckButtons.Clear();
            Triggers.Remove(_pressedTriggers);
            Triggers.Remove(_releasedTriggers);
            _board.ClearKeys();
            _board.Dispose();
        }   

        protected abstract IMacroBoard OpenDevice();

        private void InitializeTriggers()
        {
            _pressedTriggers = new NoResetObservablecollection<IBindingTrigger>();
            _releasedTriggers = new NoResetObservablecollection<IBindingTrigger>();

            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    var pressedTrigger = new HeliosTrigger(this, $"Row {row}", $"button at row {row} column {col}", "pressed", "Fired when a button is pressed.");
                    var releasedTrigger = new HeliosTrigger(this, $"Row {row}", $"button at row {row} column {col}", "released", "Fired when a button is released.");
                    
                    _pressedTriggers.Add(pressedTrigger);
                    _releasedTriggers.Add(releasedTrigger);

                    Triggers.Add(pressedTrigger);
                    Triggers.Add(releasedTrigger);
                }
            }

        }

        private void InitializeButtons()
        {
            DeckButtons.Clear();
            _board.ClearKeys();

            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    var newConfig = new MacroBoardButton(row, col, "TEST", "pack://application:,,,/Helios;component/Images/Buttons/tactile-dark-square.png");
                    newConfig.PropertyChanged += StreamDeckInterface_ButtonImageChanged;
                    DeckButtons.Add(newConfig);
                    SendUpdatedButtonImage(newConfig.Row, newConfig.Column, newConfig.ButtonImage);
                }
            }
        }

        public override void ReadXml(XmlReader reader)
        {
            Brightness = byte.Parse(reader.ReadElementString("Brightness"), CultureInfo.InvariantCulture);

            reader.ReadStartElement("Buttons");
            int buttonCount = int.Parse(reader.ReadElementString("ButtonCount"));
            for (int i = 0; i < buttonCount; i++)
            {
                reader.ReadStartElement("Button");
                var row = int.Parse(reader.ReadElementString("Row"));
                var column = int.Parse(reader.ReadElementString("Column"));
                DeckButtons[i].Text = reader.ReadElementString("Text");
                DeckButtons[i].BackgroundImageUri = reader.ReadElementString("BackgroundImageUri");
                DeckButtons[i].BackgroundImageEnabled = bool.Parse(reader.ReadElementString("BackgroundImageEnabled"));
                reader.ReadEndElement();
            }
            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            //writer.WriteElementString("Model", Model.ToString());
            writer.WriteElementString("Brightness", Brightness.ToString(CultureInfo.InvariantCulture));

            writer.WriteStartElement("Buttons");
            writer.WriteElementString("ButtonCount", (RowCount * ColumnCount).ToString());
            foreach (MacroBoardButton button in DeckButtons) {
                writer.WriteStartElement("Button");
                writer.WriteElementString("Row", button.Row.ToString());
                writer.WriteElementString("Column", button.Column.ToString());
                writer.WriteElementString("Text", button.Text);
                writer.WriteElementString("BackgroundImageUri", button.BackgroundImageUri);
                writer.WriteElementString("BackgroundImageEnabled", button.BackgroundImageEnabled.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

        }

        private void MacroBoardInterface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Model")
            {
                Triggers.Remove(_pressedTriggers);
                Triggers.Remove(_releasedTriggers);

                InitializeTriggers();
                InitializeButtons();
            }

            if (e.PropertyName == "Brightness" && IsBoardConnected)
            {
                _board?.SetBrightness(Brightness);
            }
        }

        private void SendUpdatedButtonImage(int row, int column, RenderTargetBitmap img)
        {
            var buttonId = row * ColumnCount + column;

            System.Drawing.Bitmap bitmap;

            //Convert buttonConfig.ButtonImage to a Bitmap
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder bEnc = new BmpBitmapEncoder();
                bEnc.Frames.Add(BitmapFrame.Create(img));

                bEnc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
                _board.SetKeyBitmap(buttonId, KeyBitmap.Create.FromBitmap(bitmap.Clone(new System.Drawing.Rectangle(0, 0, 72, 72),
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb)));
            }

        }

        internal void StreamDeckInterface_ButtonImageChanged(object sender, PropertyChangedEventArgs e)
        {
            var buttonConfig = sender as MacroBoardButton;

            if (e.PropertyName == "ButtonImage")
            {
                SendUpdatedButtonImage(buttonConfig.Row, buttonConfig.Column, buttonConfig.ButtonImage);
            }

        }

        internal void Board_KeyStateChanged(object sender, KeyEventArgs e)
        {
            MacroBoardButton changedButton = _buttons[e.Key];
            changedButton.IsPressed = e.IsDown;
            Console.WriteLine($"Button is pressed: {e.IsDown}");

            HeliosTrigger trigger;

            if (e.IsDown)
            {
                trigger = _pressedTriggers[e.Key] as HeliosTrigger;
            } 
            else
            {
                trigger = _releasedTriggers[e.Key] as HeliosTrigger;
            }

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() => trigger.FireTrigger(new BindingValue(e.IsDown))));
            }
            else
            {
                trigger.FireTrigger(new BindingValue(e.IsDown));
            }
        }

        internal void Board_ConnectionStateChanged(object sender, ConnectionEventArgs e)
        {
            IsBoardConnected = e.NewConnectionState;
        }
    }


}
