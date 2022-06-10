using GadrocsWorkshop.Helios.Collections;
using GadrocsWorkshop.Helios.ComponentModel;
using GadrocsWorkshop.Helios.Interfaces.Capabilities;
using OpenMacroBoard.SDK;
using StreamDeckSharp;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Application = System.Windows.Application;
using FlowDirection = System.Windows.FlowDirection;
using KeyEventArgs = OpenMacroBoard.SDK.KeyEventArgs;

namespace GadrocsWorkshop.Helios.Interfaces.HeliosMacroBoard
{
    public enum MacroBoardModel
    {
        StreamDeck,
        // ReSharper disable once InconsistentNaming
        StreamDeckXL,
        Virtual
    }

    [Serializable]
    public class MacroBoardButton : INotifyPropertyChanged
    {
        private int _row;
        private int _column;
        private bool _isPressed;
        private string _buttonText;
        private string _backgroundImageUri;
        private bool _backgroundImageEnabled;
        private BitmapImage _bitmapImage;
        private RenderTargetBitmap _buttonImage;

        public event PropertyChangedEventHandler PropertyChanged;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MacroBoardButton(int row, int column, string text, string backgroundImageUri)
        {
            _row = row;
            _column = column;
            _buttonText = text;
            _isPressed = false;
            _backgroundImageUri = backgroundImageUri;
            _backgroundImageEnabled = true;
            _bitmapImage = null;
            UpdateImage();
        }

        public MacroBoardButton(int row, int column) : this(row, column, "", "")
        {
        }

        public MacroBoardButton() : this(0, 0)
        {
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateImage()
        {
            RenderTargetBitmap targetImage = new RenderTargetBitmap(72, 72, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual visual = new DrawingVisual();

            using (DrawingContext r = visual.RenderOpen())
            {
                r.DrawRectangle(new SolidColorBrush(Colors.Black), new Pen(new SolidColorBrush(Colors.Black), 2), new Rect(0, 0, 72, 72));
                if (BackgroundImageEnabled && _bitmapImage == null)
                {
                    try
                    {
                        _bitmapImage = new BitmapImage();
                        _bitmapImage.BeginInit();
                        _bitmapImage.UriSource = new Uri(BackgroundImageUri, UriKind.Absolute);
                        _bitmapImage.EndInit();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Image rendering failed");
                    }
                }

                if (BackgroundImageEnabled)
                {
                    r.DrawImage(_bitmapImage, new Rect(0, 0, 72, 72));
                }

                FormattedText formattedText = new FormattedText(
                    _buttonText,
                    CultureInfo.GetCultureInfo("en-us"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    18,
                    Brushes.White, VisualTreeHelper.GetDpi(visual).PixelsPerDip);

                formattedText.MaxTextHeight = 64;
                formattedText.MaxTextWidth = 64;

                formattedText.TextAlignment = TextAlignment.Center;

                r.DrawText(formattedText, new Point(4, 36 - formattedText.Height / 2));
            }

            targetImage.Render(visual);
            targetImage.Freeze();
            _buttonImage = targetImage;
            NotifyPropertyChanged(nameof(ButtonImage));
        }

        public int Row
        {
            get => _row;
            set => _row = value;
        }

        public int Column
        {
            get => _column;
            set => _column = value;
        }

        public string Text
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                NotifyPropertyChanged(nameof(Text));
                UpdateImage();
            }
        }

        public string BackgroundImageUri
        {
            get => _backgroundImageUri;
            set
            {
                _backgroundImageUri = value;

                if (_backgroundImageUri == "")
                {
                    BackgroundImageEnabled = false;
                }
                _bitmapImage = null;

                NotifyPropertyChanged(nameof(BackgroundImageUri));
                UpdateImage();
            }
        }

        public bool BackgroundImageEnabled
        {
            get => _backgroundImageEnabled;
            set
            {
                _backgroundImageEnabled = value;
                NotifyPropertyChanged(nameof(BackgroundImageEnabled));
                UpdateImage();
            }
        }

        public RenderTargetBitmap ButtonImage
        {
            get
            {
                if (_buttonImage == null)
                {
                    UpdateImage();
                }
                return _buttonImage;
            }
        }

        public bool IsPressed
        {
            get => _isPressed;
            set
            {
                _isPressed = value;
                NotifyPropertyChanged(nameof(IsPressed));
            }
        }
    }

    [HeliosInterface("Helios.Base.StreamDeckXLInterface", "Stream Deck XL", typeof(MacroBoardInterfaceEditor),
                typeof(UniqueHeliosInterfaceFactory), AutoAdd = false)]
    public class StreamDeckXLInterface : MacroBoardInterface
    {

        public StreamDeckXLInterface() : base(MacroBoardModel.StreamDeckXL, "Stream Deck XL", 4, 8)
        {

        }
        protected override IMacroBoard OpenDevice()
        {
            // REVISIT this needs to target a specific device so that we can support muliple devices
            return StreamDeck.OpenDevice();
        }
    }

    [HeliosInterface("Helios.Base.StreamDeckInterface", "Stream Deck", typeof(MacroBoardInterfaceEditor),
        typeof(UniqueHeliosInterfaceFactory), AutoAdd = false)]
    public class StreamDeckInterface : MacroBoardInterface
    {

        public StreamDeckInterface() : base(MacroBoardModel.StreamDeck, "Stream Deck", 3, 5)
        {

        }
        protected override IMacroBoard OpenDevice()
        {
            // REVISIT this needs to target a specific device so that we can support muliple devices
            return StreamDeck.OpenDevice();
        }
    }

    [HeliosInterface("Helios.Base.VirtualMacroBoardInterface", "HeliosMacroBoard Virtual Board",
        typeof(MacroBoardInterfaceEditor),
        typeof(UniqueHeliosInterfaceFactory), AutoAdd = false)]
    public class VirtualMacroboardInterface : MacroBoardInterface
    {
        public VirtualMacroboardInterface() : base(MacroBoardModel.Virtual, "HeliosMacroBoard Virtual Board", 4, 8)
        {

        }

        protected override IMacroBoard OpenDevice()
        {
            return OpenMacroBoard.VirtualBoard.BoardFactory.SpawnVirtualBoard(new GridKeyPositionCollection(8, 4, 72, 25));
        }
    }

    public abstract class MacroBoardInterface : HeliosInterface, IExtendedDescription
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private byte _brightness = 50;

        public ObservableCollection<MacroBoardButton> DeckButtons { get; set; } = new ObservableCollection<MacroBoardButton>();

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

        private IMacroBoard _board;

        private bool _isDeckConnected;

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

        public string Description { get; }

        public string RemovalNarrative => "Removes the Macro Board interface..";

        // currently registered Helios triggers for this object
        private readonly NoResetObservablecollection<IBindingTrigger> _pressedTriggers = new NoResetObservablecollection<IBindingTrigger>();

        // currently registered Helios triggers for this object
        private readonly NoResetObservablecollection<IBindingTrigger> _releasedTriggers = new NoResetObservablecollection<IBindingTrigger>();

        protected MacroBoardInterface(MacroBoardModel model, string displayName, int rowCount, int columnCount) : base(displayName)
        {
            Model = model;
            RowCount = rowCount;
            Description = displayName;
            ColumnCount = columnCount;
        }

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

        protected abstract IMacroBoard OpenDevice();

        private void RegisterButtonTriggers()
        {
            foreach (MacroBoardButton button in DeckButtons)
            {
                HeliosTrigger pressedTrigger = new HeliosTrigger(this, $"Row {button.Row}", $"button at row {button.Row} column {button.Column}", "pressed", "Fired when a button is pressed.");
                HeliosTrigger releasedTrigger = new HeliosTrigger(this, $"Row {button.Row}", $"button at row {button.Row} column {button.Column}", "released", "Fired when a button is released.");

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
                    MacroBoardButton newConfig = new MacroBoardButton(row, col, "TEST", "pack://application:,,,/Helios;component/Images/Buttons/tactile-dark-square.png");
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

        public override void ReadXml(XmlReader reader)
        {
            if (reader.Name == "Model")
            {
                // NOTE: ignoring parse error, because it currently does not matter
                Model = Enum.TryParse(reader.ReadElementString("Model"), true, out MacroBoardModel model) ? model : MacroBoardModel.StreamDeck;
            }
            Brightness = byte.Parse(reader.ReadElementString("Brightness"), CultureInfo.InvariantCulture);

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
                    MacroBoardButton button = new MacroBoardButton(row, column, reader.ReadElementString("Text"), reader.ReadElementString("BackgroundImageUri"))
                    {
                        BackgroundImageEnabled = bool.Parse(reader.ReadElementString("BackgroundImageEnabled"))
                    };
                    DeckButtons.Add(button);
                }
                else
                {
                    Logger.Warn("Macro Board interface discarded button at row {Row}, column {Column} that does not match the declared macro board size", row, column);
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

        private void MacroBoardInterface_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Model")
            {
                Triggers.Remove(_pressedTriggers);
                Triggers.Remove(_releasedTriggers);

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

        private void SendButtonImage(int row, int column, RenderTargetBitmap img)
        {
            int buttonId = row * ColumnCount + column;

            //Convert buttonConfig.ButtonImage to a Bitmap
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder bEnc = new BmpBitmapEncoder();
                bEnc.Frames.Add(BitmapFrame.Create(img));
                bEnc.Save(outStream);
                using (System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream))
                using (System.Drawing.Bitmap topLeft = bitmap.Clone(new System.Drawing.Rectangle(0, 0, 72, 72), System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    _board.SetKeyBitmap(buttonId, KeyBitmap.Create.FromBitmap(topLeft));
                }
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
                Application.Current.Dispatcher.BeginInvoke(new Action(() => trigger?.FireTrigger(new BindingValue(e.IsDown))));
            }
            else
            {
                trigger?.FireTrigger(new BindingValue(e.IsDown));
            }
        }

        internal void Board_ConnectionStateChanged(object sender, ConnectionEventArgs e)
        {
            IsBoardConnected = e.NewConnectionState;
        }
    }
}