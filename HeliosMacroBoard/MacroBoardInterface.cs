// Copyright 2014 Craig Courtney
// Copyright 2022 Helios Contributors
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml;
using GadrocsWorkshop.Helios.Collections;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using OpenMacroBoard.SDK;

namespace GadrocsWorkshop.Helios.Interfaces.HeliosMacroBoard
{
    public abstract class MacroBoardInterface : HeliosInterface, IExtendedDescription
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private byte _brightness = 50;

        private IMacroBoard _board;

        private bool _isDeckConnected;

        // currently registered Helios triggers for this object
        private readonly NoResetObservablecollection<IBindingTrigger> _pressedTriggers =
            new NoResetObservablecollection<IBindingTrigger>();

        // currently registered Helios triggers for this object
        private readonly NoResetObservablecollection<IBindingTrigger> _releasedTriggers =
            new NoResetObservablecollection<IBindingTrigger>();

        protected MacroBoardInterface(MacroBoardModel model, string displayName, int rowCount, int columnCount) :
            base(displayName)
        {
            Model = model;
            RowCount = rowCount;
            Description = displayName;
            ColumnCount = columnCount;

            CreateDefaultButtons();
        }

        private void RegisterButtonTriggers()
        {
            foreach (MacroBoardButton button in DeckButtons)
            {
                HeliosTrigger pressedTrigger = new HeliosTrigger(this, $"Row {button.Row}",
                    $"button at row {button.Row} column {button.Column}", "pressed", "Fired when a button is pressed.",
                    "Always returns true.", BindingValueUnits.Boolean);
                HeliosTrigger releasedTrigger = new HeliosTrigger(this, $"Row {button.Row}",
                    $"button at row {button.Row} column {button.Column}", "released",
                    "Fired when a button is released.", "Always returns false.", BindingValueUnits.Boolean);

                _pressedTriggers.Add(pressedTrigger);
                _releasedTriggers.Add(releasedTrigger);

                Triggers.Add(pressedTrigger);
                Triggers.Add(releasedTrigger);
            }
        }

        private void UnregisterButtonTriggers()
        {
            Triggers.Remove(_pressedTriggers);
            Triggers.Remove(_releasedTriggers);
            _pressedTriggers.Clear();
            _releasedTriggers.Clear();
        }

        private void OpenBoard()
        {
            if (null != _board)
            {
                Debug.Assert(null == _board, "logic error");
                return;
            }

            _board = OpenDevice();
            _board.KeyStateChanged += Board_KeyStateChanged;
        }

        private void CloseBoard()
        {
            if (null == _board)
            {
                return;
            }

            _board.KeyStateChanged -= Board_KeyStateChanged;
            _board.ClearKeys();
            _board.Dispose();
        }

        private void CreateDefaultButtons()
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    MacroBoardButton newConfig = new MacroBoardButton(row, col, "TEST",
                        "pack://application:,,,/Helios;component/Images/Buttons/tactile-dark-square.png");
                    newConfig.PropertyChanged += MacroBoardButton_PropertyChanged;
                    DeckButtons.Add(newConfig);
                }
            }
        }

        private void SendButtonImages()
        {
            _board.ClearKeys();
            foreach (MacroBoardButton button in DeckButtons)
            {
                SendButtonImage(button.Row, button.Column, button.ButtonImage);
            }
        }

        private void ClearButtons()
        {
            foreach (MacroBoardButton button in DeckButtons)
            {
                button.PropertyChanged -= MacroBoardButton_PropertyChanged;
            }

            DeckButtons.Clear();
        }

        private void SendButtonImage(int row, int column, RenderTargetBitmap img)
        {
            int buttonId = row * ColumnCount + column;

            // Convert buttonConfig.ButtonImage to a Bitmap
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder bEnc = new BmpBitmapEncoder();
                bEnc.Frames.Add(BitmapFrame.Create(img));
                bEnc.Save(outStream);
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream))
                using (System.Drawing.Bitmap topLeft = bitmap.Clone(new System.Drawing.Rectangle(0, 0, 72, 72),
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    _board.SetKeyBitmap(buttonId, KeyBitmap.Create.FromBitmap(topLeft));
                }
            }
        }

        #region Hooks

        protected abstract IMacroBoard OpenDevice();

        #endregion

        #region Event Handlers

        internal void Board_ConnectionStateChanged(object sender, ConnectionEventArgs e)
        {
            IsBoardConnected = e.NewConnectionState;
        }

        internal void Board_KeyStateChanged(object sender, KeyEventArgs e)
        {
            MacroBoardButton changedButton = DeckButtons[e.Key];
            changedButton.IsPressed = e.IsDown;
            Logger.Debug("Button {Index} is {State}", e.Key, e.IsDown);

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
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    trigger?.FireTrigger(new BindingValue(e.IsDown))));
            }
            else
            {
                trigger?.FireTrigger(new BindingValue(e.IsDown));
            }
        }

        internal void MacroBoardButton_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MacroBoardButton buttonConfig = (MacroBoardButton)sender;
            if (e.PropertyName == nameof(MacroBoardButton.ButtonImage))
            {
                SendButtonImage(buttonConfig.Row, buttonConfig.Column, buttonConfig.ButtonImage);
            }
        }

        private void MacroBoardInterface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Model")
            {
                UnregisterButtonTriggers();
                ClearButtons();
                CreateDefaultButtons();
                SendButtonImages();
                RegisterButtonTriggers();
            }

            if (e.PropertyName == "Brightness" && IsBoardConnected)
            {
                _board?.SetBrightness(Brightness);
            }
        }

        #endregion

        #region Overrides

        protected override void AttachToProfileOnMainThread()
        {
            OpenBoard();
            SendButtonImages();
            RegisterButtonTriggers();
            PropertyChanged += MacroBoardInterface_PropertyChanged;
        }

        protected override void DetachFromProfileOnMainThread(HeliosProfile oldProfile)
        {
            PropertyChanged -= MacroBoardInterface_PropertyChanged;
            UnregisterButtonTriggers();
            ClearButtons();
            CloseBoard();
        }

        public override void ReadXml(XmlReader reader)
        {
            if (reader.Name == "Model")
            {
                // NOTE: ignoring parse error, because it currently does not matter
                Model = Enum.TryParse(reader.ReadElementString("Model"), true, out MacroBoardModel model)
                    ? model
                    : MacroBoardModel.StreamDeck;
            }

            Brightness = byte.Parse(reader.ReadElementString("Brightness"), CultureInfo.InvariantCulture);

            ClearButtons();
            reader.ReadStartElement("Buttons");
            int buttonCount = int.Parse(reader.ReadElementString("ButtonCount"));
            for (int i = 0; i < buttonCount; i++)
            {
                reader.ReadStartElement("Button");
                int row = int.Parse(reader.ReadElementString("Row"));
                int column = int.Parse(reader.ReadElementString("Column"));
                // survive hacked up profiles which don't populate all the buttons
                int position = row * ColumnCount + column;
                if (position < buttonCount)
                {
                    MacroBoardButton button = new MacroBoardButton(row, column, reader.ReadElementString("Text"),
                        reader.ReadElementString("BackgroundImageUri"))
                    {
                        BackgroundImageEnabled = bool.Parse(reader.ReadElementString("BackgroundImageEnabled"))
                    };
                    DeckButtons.Add(button);
                }
                else
                {
                    Logger.Warn(
                        "Macro Board interface discarded button at row {Row}, column {Column} that does not match the declared macro board size",
                        row, column);
                }

                reader.ReadEndElement();
            }

            reader.ReadEndElement();
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Model", Model.ToString());
            writer.WriteElementString("Brightness", Brightness.ToString(CultureInfo.InvariantCulture));

            writer.WriteStartElement("Buttons");
            writer.WriteElementString("ButtonCount", (RowCount * ColumnCount).ToString());
            foreach (MacroBoardButton button in DeckButtons)
            {
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

        #endregion

        #region IExtendedDescription

        public string Description { get; }

        public string RemovalNarrative => "Removes the Macro Board interface..";

        #endregion

        #region Properties

        public ObservableCollection<MacroBoardButton> DeckButtons { get; set; } =
            new ObservableCollection<MacroBoardButton>();

        public MacroBoardModel Model { get; private set; }

        public byte Brightness
        {
            get => _brightness;
            set
            {
                byte oldValue = _brightness;
                _brightness = value;
                OnPropertyChanged("Brightness", oldValue, value, true);
            }
        }

        public int RowCount { get; }

        public int ColumnCount { get; }

        public bool IsBoardConnected
        {
            get => _isDeckConnected;
            private set
            {
                bool oldValue = _isDeckConnected;
                _isDeckConnected = value;
                OnPropertyChanged("IsBoardConnected", oldValue, value, false);
            }
        }

        #endregion
    }
}
