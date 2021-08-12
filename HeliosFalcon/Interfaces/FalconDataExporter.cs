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

using System.Collections.Generic;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon
{
    abstract class FalconDataExporter
    {
        private FalconInterface _falconInterface;
        private Dictionary<string, HeliosValue> _values = new Dictionary<string, HeliosValue>();

        protected FalconDataExporter(FalconInterface falconInterface)
        {
            _falconInterface = falconInterface;
        }

        #region Properties

        protected FalconInterface FalconInterface
        {
            get { return _falconInterface; }
        }

        internal abstract RadarContact[] RadarContacts { get; }
        internal abstract string[] RwrInfo { get; }
        internal abstract List<string> NavPoints { get; }
        internal abstract bool StringDataUpdated { get; }

        #endregion

        internal void RemoveExportData(FalconInterface falcon)
        {
            foreach (HeliosValue value in _values.Values)
            {
                falcon.Triggers.Remove(value);
                falcon.Values.Remove(value);
            }
            _values.Clear();
        }

        internal abstract void InitData();
		internal abstract void PollUserInterfaceData();
		internal abstract void PollFlightData();
        internal abstract void CloseData();

        public static float AngleDelta(float Ang1, float Ang2)
        {
            Ang1 = Ang1 % 360;
            Ang2 = Ang2 % 360;
            if (Ang1 == Ang2)
            {
                return 0.0f; //No angle to compute
            }
            else
            {
                float fDif = (Ang2 - Ang1);//There is an angle to compute
                if (fDif >= 180.0f)
                {
                    fDif = fDif - 180.0f; //correct the half
                    fDif = 180.0f - fDif; //invert the half
                    fDif = 0 - fDif; //reverse direction
                    return fDif;
                }
                else
                {
                    if (fDif <= -180.0f)
                    {
                        fDif = fDif + 180.0f; //correct the half
                        fDif = 180.0f + fDif;
                        return fDif;
                    }
                }
                return fDif;
            }
        }

        protected HeliosValue AddValue(string device, string name, string description, string valueDescription, BindingValueUnit unit, string correctedDeviceName = null)
        {
            HeliosValue value;
            if (correctedDeviceName == null)
            {
                value = new HeliosValue(FalconInterface, BindingValue.Empty, device, name, description, valueDescription, unit);
            }
            else
            {
                value = new HeliosValueWithCorrectedDeviceName(FalconInterface, BindingValue.Empty, device, name, description, valueDescription, unit, correctedDeviceName);
            }
            FalconInterface.Triggers.Add(value);
            FalconInterface.Values.Add(value);
            _values.Add(device + "." + name, value);
            return value;
        }

        protected void SetValue(string device, string name, BindingValue value)
        {
            string key = device + "." + name;
            if (_values.ContainsKey(key))
            {
                _values[key].SetValue(value, false);
            }
        }

        public BindingValue GetValue(string device, string name)
        {
            string key = device + "." + name;
            if (_values.ContainsKey(key))
            {
                return _values[key].Value;
            }
            return BindingValue.Empty;
        }
    }
}
