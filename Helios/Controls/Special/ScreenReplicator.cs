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

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Xml;
using GadrocsWorkshop.Helios.ComponentModel;

namespace GadrocsWorkshop.Helios.Controls.Special
{
    [HeliosControl("Helios.Base.ScreenReplicator", "Screen Replicator", "Special Controls", typeof(ScreenReplicatorRenderer))]
    public class ScreenReplicator : HeliosVisual
    {
        private Int32Rect _captureLocation = new Int32Rect(0, 0, 300, 300);
        private int _replicationsPerSecond = 2;
        private bool _replicateOnProfileStart = false;
        private bool _isRunning = false;
        private bool _isReplicating = false;
        private bool _blankOnStop = true;
        private readonly Size _defaultSize = new Size(300, 300);

        private int _millisecondsPerReplication = 500;
        private int _lastReplication;

        public ScreenReplicator() : this("Screen Shot Extractor", new Size(300,300)) { }
        public ScreenReplicator(string name, Size size)
            : base(name, size)
        {
            HeliosAction startReplicating = new HeliosAction(this, "", "", "start replication", "Start replicating the screen.");
            startReplicating.Execute += StartReplicating_Execute;
            Actions.Add(startReplicating);

            HeliosAction stopReplicating = new HeliosAction(this, "", "", "stop replication", "Stops replicating the screen.");
            stopReplicating.Execute += StopReplicating_Execute;
            Actions.Add(stopReplicating);
        }

        void StartReplicating_Execute(object action, HeliosActionEventArgs e)
        {
            _isReplicating = true;
        }

        void StopReplicating_Execute(object action, HeliosActionEventArgs e)
        {
            _isReplicating = false;
            OnDisplayUpdate();
        }

        #region Properties

        public int CaptureTop
        {
            get
            {
                return _captureLocation.Y;
            }
            set
            {
                if (!_captureLocation.Y.Equals(value))
                {
                    int oldValue = _captureLocation.Y;
                    _captureLocation.Y = value;
                    OnPropertyChanged("CaptureTop", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        public int CaptureLeft
        {
            get
            {
                return _captureLocation.X;
            }
            set
            {
                if (!_captureLocation.X.Equals(value))
                {
                    int oldValue = _captureLocation.X;
                    _captureLocation.X = value;
                    OnPropertyChanged("CaptureLeft", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        public int CaptureWidth
        {
            get
            {
                return _captureLocation.Width;
            }
            set
            {
                if (!_captureLocation.Width.Equals(value))
                {
                    int oldValue = _captureLocation.Width;
                    _captureLocation.Width = value;
                    OnPropertyChanged("CaptureWidth", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        public int CaptureHeight
        {
            get
            {
                return _captureLocation.Height;
            }
            set
            {
                if (!_captureLocation.Height.Equals(value))
                {
                    int oldValue = _captureLocation.Height;
                    _captureLocation.Height = value;
                    OnPropertyChanged("CaptureHeight", oldValue, value, true);
                    OnDisplayUpdate();
                }
            }
        }

        internal Int32Rect CaptureRectangle
        {
            get
            {
                return _captureLocation;
            }
        }

        public int ReplicationsPerSecond
        {
            get
            {
                return _replicationsPerSecond;
            }
            set
            {
                if (!_replicationsPerSecond.Equals(value))
                {
                    int oldValue = _replicationsPerSecond;
                    _replicationsPerSecond = value;
                    _millisecondsPerReplication = Math.Max(1000 / _replicationsPerSecond, 100);
                    OnPropertyChanged("ReplicationsPerSecond", oldValue, value, true);
                }
            }
        }

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            private set
            {
                if (!_isRunning.Equals(value))
                {
                    bool oldValue = _isRunning;
                    _isRunning = value;
                    OnPropertyChanged("IsRunning", oldValue, value, false);
                }
            }
        }

        public bool IsReplicating
        {
            get
            {
                return _isReplicating;
            }
            set
            {
                if (!_isReplicating.Equals(value))
                {
                    bool oldValue = _isReplicating;
                    _isReplicating = value;
                    OnPropertyChanged("IsReplicating", oldValue, value, false);
                }
            }
        }

        public bool ReplicatesOnProfileStart
        {
            get
            {
                return _replicateOnProfileStart;
            }
            set
            {
                if (!_replicateOnProfileStart.Equals(value))
                {
                    bool oldValue = _replicateOnProfileStart;
                    _replicateOnProfileStart = value;
                    OnPropertyChanged("ReplicatesOnProfileStart", oldValue, value, true);
                }
            }
        }

        public bool BlankOnStop
        {
            get
            {
                return _blankOnStop;
            }
            set
            {
                if (!_blankOnStop.Equals(value))
                {
                    bool oldValue = _blankOnStop;
                    _blankOnStop = value;
                    OnPropertyChanged("BlankOnStop", oldValue, value, true);
                }
            }
        }

        #endregion

        protected override void OnProfileChanged(HeliosProfile oldProfile)
        {
            base.OnProfileChanged(oldProfile);

            if (oldProfile != null)
            {
                oldProfile.ProfileStarted -= new EventHandler(Profile_ProfileStarted);
                oldProfile.ProfileTick -= new EventHandler(Profile_ProfileTick);
                oldProfile.ProfileStopped -= new EventHandler(Profile_ProfileStopped);
            }

            if (Profile != null)
            {
                Profile.ProfileStarted += new EventHandler(Profile_ProfileStarted);
                Profile.ProfileTick += new EventHandler(Profile_ProfileTick);
                Profile.ProfileStopped += new EventHandler(Profile_ProfileStopped);
            }
        }

        void Profile_ProfileStopped(object sender, EventArgs e)
        {
            IsReplicating = false;
            IsRunning = false;
        }

        void Profile_ProfileTick(object sender, EventArgs e)
        {
            int currentTime = Environment.TickCount;

            if (currentTime - _lastReplication > _millisecondsPerReplication)
            {
                OnDisplayUpdate();
                _lastReplication = currentTime;
            }
        }

        void Profile_ProfileStarted(object sender, EventArgs e)
        {
            IsRunning = true;
            IsReplicating = ReplicatesOnProfileStart;
        }

        public override void MouseDown(System.Windows.Point location)
        {
            // No-Op
        }

        public override void MouseDrag(System.Windows.Point location)
        {
            // No-Op
        }

        public override void MouseUp(System.Windows.Point location)
        {
            // No-Op
        }

        public override void WriteXml(XmlWriter writer)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            base.WriteXml(writer);
            writer.WriteElementString("CaptureTop", CaptureTop.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("CaptureLeft", CaptureLeft.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("CaptureWidth", CaptureWidth.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("CaptureHeight", CaptureHeight.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("UpdateFrequency", ReplicationsPerSecond.ToString(CultureInfo.InvariantCulture));
            writer.WriteElementString("ReplicateOnStart", boolConverter.ConvertToInvariantString(ReplicatesOnProfileStart));
            writer.WriteElementString("BlankOnStop", boolConverter.ConvertToInvariantString(BlankOnStop));
        }

        public override void ReadXml(XmlReader reader)
        {
            TypeConverter boolConverter = TypeDescriptor.GetConverter(typeof(bool));

            base.ReadXml(reader);
            CaptureTop = int.Parse(reader.ReadElementString("CaptureTop"), CultureInfo.InvariantCulture);
            CaptureLeft = int.Parse(reader.ReadElementString("CaptureLeft"), CultureInfo.InvariantCulture);
            CaptureWidth = int.Parse(reader.ReadElementString("CaptureWidth"), CultureInfo.InvariantCulture);
            CaptureHeight = int.Parse(reader.ReadElementString("CaptureHeight"), CultureInfo.InvariantCulture);
            ReplicationsPerSecond = int.Parse(reader.ReadElementString("UpdateFrequency"), CultureInfo.InvariantCulture);
            ReplicatesOnProfileStart = (bool)boolConverter.ConvertFromInvariantString(reader.ReadElementString("ReplicateOnStart"));
            BlankOnStop = (bool)boolConverter.ConvertFromInvariantString(reader.ReadElementString("BlankOnStop"));
        }
    }
}
