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

using System.Linq;

namespace GadrocsWorkshop.Helios
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Xml;

    [JsonObject]
    public class CalibrationPointCollectionDouble : NotificationObject, ICollection<CalibrationPointDouble>, INotifyCollectionChanged
    {
        [JsonProperty("points")]
        private LinkedList<CalibrationPointDouble> _points = new LinkedList<CalibrationPointDouble>();

        private int _precision = 1;

        private double _outputLimitMax = double.MaxValue;
        private double _outputLimitMin = double.MinValue;

        public event EventHandler CalibrationChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;



        public CalibrationPointCollectionDouble()
        {
            Add(new CalibrationPointDouble(0, 0));
            Add(new CalibrationPointDouble(100, 1));
        }

        public CalibrationPointCollectionDouble(double minimumInput, double minimumOuput, double maximumInput, double maximumOutput, int precision = 1)
        {
            Precision = precision;
            Add(new CalibrationPointDouble(minimumInput, minimumOuput));
            Add(new CalibrationPointDouble(maximumInput, maximumOutput));
        }

        public double Interpolate(double input)
        {
            double retValue = 0.0d;

            if (_points.Count >= 2)
            {
                LinkedListNode<CalibrationPointDouble> valuePoint = _points.First;
                while (valuePoint != null && valuePoint.Value.Value < input)
                {
                    valuePoint = valuePoint.Next;
                }

                if (valuePoint == null)
                {
                    retValue = _points.Last.Value.Multiplier;
                }
                else if (valuePoint.Value.Value == input || valuePoint.Previous == null)
                {
                    retValue = valuePoint.Value.Multiplier;
                }
                else
                {
                    double valueRange = valuePoint.Value.Value - valuePoint.Previous.Value.Value;
                    double modifierRange = valuePoint.Value.Multiplier - valuePoint.Previous.Value.Multiplier;
                    double offsetValue = input - valuePoint.Previous.Value.Value;
                    double offsetMultiplier = offsetValue / valueRange;
                    double modifier = modifierRange * offsetMultiplier;

                    retValue = valuePoint.Previous.Value.Multiplier + modifier;
                }

                retValue = Math.Round(retValue, _precision);
            }

            return retValue;
        }

        public double OutputLimitMin
        {
            get { return _outputLimitMin; }
            set
            {
                if (_outputLimitMin != value)
                {
                    _outputLimitMin = value;
                    if (MinimumOutputValue < _outputLimitMin)
                    {
                        MinimumOutputValue = _outputLimitMin;
                    }
                }
            }
        }
        public bool ShouldSerializeOutputLimitMin()
        {
            return OutputLimitMin != double.MinValue;
        }

        public double OutputLimitMax
        {
            get { return _outputLimitMax; }
            set
            {
                if (_outputLimitMax != value)
                {
                    _outputLimitMin = value;
                    if (MaximumOutputValue > _outputLimitMax)
                    {
                        MaximumOutputValue = _outputLimitMax;
                    }
                }
            }
        }
        public bool ShouldSerializeOutputLimitMax()
        {
            return OutputLimitMax != double.MaxValue;
        }

        [JsonProperty("precision")]
        public int Precision
        {
            get { return _precision; }
            set
            {
                _precision = value;
                OnCalibrationChanged();
            }
        }

        [JsonIgnore]
        public double MaximumInputValue
        {
            get
            {
                return _points.Last.Value.Value;
            }
            set
            {
                double oldValue = _points.Last.Value.Value;
                double lastValue = value;
                LinkedListNode<CalibrationPointDouble> node = _points.Last.Previous;
                while (node != null && node.Value.Value > lastValue)
                {
                    lastValue -= 0.1f;
                    node.Value.Value = lastValue;
                    node = node.Previous;
                }
                _points.Last.Value.Value = value;
                OnCalibrationChanged();
                OnPropertyChanged("MaximumInputValue", oldValue, value, false);
            }
        }

        [JsonIgnore]
        public double MaximumOutputValue
        {
            get
            {
                return _points.Last.Value.Multiplier;
            }
            set
            {
                double oldValue = _points.Last.Value.Multiplier;
                double lastValue = value;
                //LinkedListNode<CalibrationPointDouble> node = _points.Last.Previous;
                //while (node != null && node.Value.Multiplier > lastValue)
                //{
                //    lastValue -= 0.1f;
                //    node.Value.Multiplier = lastValue;
                //    node = node.Previous;
                //}
                _points.Last.Value.Multiplier = value;
                OnCalibrationChanged();
                OnPropertyChanged("MaximumOutputValue", oldValue, value, false);
            }
        }

        [JsonIgnore]
        public double MinimumInputValue
        {
            get
            {
                return _points.First.Value.Value;
            }
            set
            {
                double oldValue = _points.First.Value.Value;
                double lastValue = value;
                //LinkedListNode<CalibrationPointDouble> node = _points.First.Next;
                //while (node != null && node.Value.Value < lastValue)
                //{
                //    lastValue += 0.1f;
                //    node.Value.Value = lastValue;
                //    node = node.Next;
                //}
                _points.First.Value.Value = value;
                OnCalibrationChanged();
                OnPropertyChanged("MinimumInputValue", oldValue, value, false);
            }
        }

        [JsonIgnore]
        public double MinimumOutputValue
        {
            get
            {
                return _points.First.Value.Multiplier;
            }
            set
            {
                double oldValue = _points.First.Value.Multiplier;
                double lastValue = value;
                LinkedListNode<CalibrationPointDouble> node = _points.First.Next;
                while (node != null && node.Value.Value < lastValue)
                {
                    lastValue += 1;
                    node.Value.Multiplier = lastValue;
                    node = node.Next;
                }
                _points.First.Value.Multiplier = value;
                OnCalibrationChanged();
                OnPropertyChanged("MinimumOutputValue", oldValue, value, false);
            }
        }

        #region ICollection<KeyPoint> Members

        public void Add(CalibrationPointDouble item)
        {
            if (item.Multiplier < OutputLimitMin) item.Multiplier = OutputLimitMin;
            if (item.Multiplier > OutputLimitMax) item.Multiplier = OutputLimitMax;

            LinkedListNode<CalibrationPointDouble> addBefore = _points.First;
            while (addBefore != null && addBefore.Value.Value < item.Value)
            {
                addBefore = addBefore.Next;
            }

            if (addBefore == null)
            {
                _points.AddLast(item);
            }
            else
            {
                _points.AddBefore(addBefore, item);
            }

            item.PropertyChanged += Item_PropertyChanged;

            OnCalibrationChanged();
            OnCollectionChanged();
        }

        public void AddPointAfter(CalibrationPointDouble item)
        {
            LinkedListNode<CalibrationPointDouble> addAfter = _points.Find(item);
            if (addAfter?.Next == null)
            {
                return;
            }

            CalibrationPointDouble point = new CalibrationPointDouble(item.Value + ((addAfter.Next.Value.Value - item.Value) / 2), item.Multiplier + ((addAfter.Next.Value.Multiplier - item.Multiplier) / 2));
            Add(point);
        }

        void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (MinimumOutputValue < _outputLimitMin)
            {
                MinimumOutputValue = _outputLimitMin;
            }
            if (MaximumOutputValue > _outputLimitMax)
            {
                MaximumOutputValue = _outputLimitMax;
            }
            OnCalibrationChanged();
        }

        public void Clear()
        {
            CalibrationPointDouble first = _points.First.Value;
            CalibrationPointDouble last = _points.Last.Value;
            _points.Clear();
            _points.AddFirst(first);
            _points.AddLast(last);
            OnCalibrationChanged();
        }

        public bool Contains(CalibrationPointDouble item)
        {
            return _points.Contains(item);
        }

        public void CopyTo(CalibrationPointDouble[] array, int arrayIndex)
        {
            _points.CopyTo(array, arrayIndex);
        }

        [JsonIgnore]
        public int Count
        {
            get { return _points.Count; }
        }

        [JsonIgnore]
        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(CalibrationPointDouble item)
        {
            if (item != _points.First.Value && item != _points.Last.Value)
            {
                bool retValue = _points.Remove(item);
                item.PropertyChanged -= Item_PropertyChanged;
                OnCalibrationChanged();
                OnCollectionChanged();
                return retValue;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<KeyPoint> Members

        public IEnumerator<CalibrationPointDouble> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        #endregion

        public void OnCalibrationChanged()
        {
            EventHandler handler = CalibrationChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Write(XmlWriter writer)
        {
            writer.WriteElementString("PointCount", _points.Count.ToString(CultureInfo.InvariantCulture));
            writer.WriteStartElement("CalibrationPoints");
            LinkedListNode<CalibrationPointDouble> point = _points.First;
            while (point != null)
            {
                writer.WriteStartElement("Point");
                writer.WriteElementString("Input", point.Value.Value.ToString(CultureInfo.InvariantCulture));
                writer.WriteElementString("Output", point.Value.Multiplier.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
                point = point.Next;
            }
            writer.WriteEndElement();
        }

        public void Read(XmlReader reader)
        {
            int count = int.Parse(reader.ReadElementString("PointCount"), CultureInfo.InvariantCulture);
            reader.ReadStartElement("CalibrationPoints");

            CalibrationPointDouble minPoint = ReadPoint(reader);
            List<CalibrationPointDouble> points = new List<CalibrationPointDouble>();

            for (int i = 2; i < count; i++)
            {
                points.Add(ReadPoint(reader));
            }
            CalibrationPointDouble maxPoint = ReadPoint(reader);

            MinimumInputValue = minPoint.Value;
            MinimumOutputValue = minPoint.Multiplier;
            MaximumInputValue = maxPoint.Value;
            MaximumOutputValue = maxPoint.Multiplier;

            foreach (CalibrationPointDouble midPoint in points)
            {
                Add(midPoint);
            }

            reader.ReadEndElement();
        }

        internal void Sort()
        {
            List<CalibrationPointDouble> sorted = this.OrderBy(item => item.Value).ToList();
            _points.Clear();
            sorted.ForEach(item => _points.AddLast(item));
            OnCalibrationChanged();
            OnCollectionChanged();
        }

        private CalibrationPointDouble ReadPoint(XmlReader reader)
        {
            reader.ReadStartElement("Point");
            double value = double.Parse(reader.ReadElementString("Input"), CultureInfo.InvariantCulture);
            double output = double.Parse(reader.ReadElementString("Output"), CultureInfo.InvariantCulture);
            reader.ReadEndElement();
            return new CalibrationPointDouble(value, output);
        }
    }
}
