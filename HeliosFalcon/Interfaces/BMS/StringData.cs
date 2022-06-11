//  Copyright 2021 Todd Kennedy
//  Copyright 2020 Helios Contributors
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
using System.Collections.Generic;
using System.Text;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon.BMS
{
    [Serializable]
    public class StringData
    {
        public const uint STRINGDATA_AREA_SIZE_MAX = 1024 * 1024;

        public int VersionNum;  // Version of the StringData shared memory area - only indicates changes to the StringIdentifier enum
        public uint NoOfStrings;       // How many strings do we have in the area?
        public uint dataSize;          // the overall size of the StringData/FalconSharedMemoryAreaString shared memory area
        public IEnumerable<StringStruct> data = new List<StringStruct>();
        private List<string> _navPoints = new List<string>();


        private static StringData GetStringData(byte[] data)
        {
            if (data == null) return null;
            int offset = 0;
            var toReturn = new StringData();
            toReturn.VersionNum = BitConverter.ToInt32(data, offset);
            offset += sizeof(int);
            toReturn.NoOfStrings = BitConverter.ToUInt32(data, offset);
            offset += sizeof(uint);
            toReturn.dataSize = BitConverter.ToUInt32(data, offset);
            offset += sizeof(uint);
            for (var i = 0; i < toReturn.NoOfStrings; i++)
            {
                if (offset >= data.Length - sizeof(uint)) break;
                var sStruct = new StringStruct();
                sStruct.strId = BitConverter.ToUInt32(data, offset);
                offset += sizeof(uint);
                sStruct.strLength = BitConverter.ToUInt32(data, offset);
                offset += sizeof(uint);
                sStruct.strData = new byte[sStruct.strLength];
                Array.Copy(data, offset, sStruct.strData, 0, Math.Min(sStruct.strLength, data.Length - offset));
                offset += (int)sStruct.strLength + 1;
                (toReturn.data as IList<StringStruct>).Add(sStruct);
            }
            return toReturn;
        }

        public string GetValueForStrId(byte[] data, StringIdentifier stringIdentifier)
        {
            string rtnValue = "";
            foreach(var item in GetStringData(data).data)
            {
                if ((StringIdentifier)item.strId == stringIdentifier)
                {
                    rtnValue = item.value;
                    break;
                }
            }
            return rtnValue;
        }

        public List<string> GetNavPoints(byte[] data)
        {
            StringData stringData = GetStringData(data);
            foreach (var item in stringData.data)
            {
                if ((StringIdentifier)item.strId == StringIdentifier.NavPoint)
                {
                    _navPoints.Add(item.value.Replace(";", ""));
                }
            }
            return _navPoints.Count > 0 ? _navPoints : null;
        }
    }

    [Serializable]
    public struct StringStruct
    {
        public uint strId;
        public uint strLength;     // The length of the string in "strData", stored *without* termination!
        public byte[] strData;
        public string value { get { return strData != null && strData.Length > 0 ? Encoding.Default.GetString(strData, 0, (int)strLength) : string.Empty; } }
        public override string ToString()
        {
            var identifier = Enum.GetName(typeof(StringIdentifier), strId);
            if (string.IsNullOrWhiteSpace(identifier))
            {
                identifier = strId.ToString();
            }
            return $"{identifier}={value}";
        }
    }
}
