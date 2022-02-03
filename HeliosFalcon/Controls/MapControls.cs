//  Copyright 2014 Craig Courtney
//  Copyright 2022 Helios Contributors
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

namespace GadrocsWorkshop.Helios.Controls
{
	using System.Windows;
	using System.Xml;
	using System;
	using System.Globalization;


	public class MapControls : Gauges.BaseGauge
	{
		public string[,] _mapBaseImages = new string[,]
		{	{ "101", "{HeliosFalcon}/Images/Maps/Aegean/Map.jpg", "1", "Aegean" },
			{ "102", "{HeliosFalcon}/Images/Maps/Balkans/Map.jpg", "1", "Balkans, BFB 1.2.1" },
			{ "103", "{HeliosFalcon}/Images/Maps/CentralEurope/Map.jpg", "1", "CentralEurope, Central Europe, CET" },
			{ "104", "{HeliosFalcon}/Images/Maps/EMF/Map.jpg", "2", "EMF, EMFL 35.0.6a" },
			{ "105", "{HeliosFalcon}/Images/Maps/Iberia/Map.jpg", "2", "Iberia, POH" },
			{ "106", "{HeliosFalcon}/Images/Maps/Ikaros/Map.jpg", "1", "Ikaros" },
			{ "107", "{HeliosFalcon}/Images/Maps/Israel/Map.jpg", "1", "Israel, bfs2.2.4sp" },
			{ "108", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO, Korea TvT, RedFlag, Red Flag" },
			{ "109", "{HeliosFalcon}/Images/Maps/Kuwait/Map.jpg", "1", "Kuwait" },
			{ "110", "{HeliosFalcon}/Images/Maps/Libya/Map.jpg", "2", "Libya" },
			{ "111", "{HeliosFalcon}/Images/Maps/Nevada/Map.jpg", "1", "Nevada" },
			{ "112", "{HeliosFalcon}/Images/Maps/Nordic/Map.jpg", "1", "Nordic, NTO, NTO Polar Vortex, CWC, NordicCWC" },
			{ "113", "{HeliosFalcon}/Images/Maps/Panama/Map.jpg", "1", "Panama" },
			{ "114", "{HeliosFalcon}/Images/Maps/Taiwan/Map.jpg", "1", "Taiwan" },
			{ "115", "", "1", "" },
			{ "116", "", "1", "" },
			{ "117", "", "1", "" },
			{ "118", "", "1", "" },
			{ "119", "", "1", "" },
			{ "120", "", "1", "" },
			{ "121", "", "1", "" },
			{ "122", "", "1", "" },
			{ "123", "", "1", "" },
			{ "124", "", "1", "" } };

		public string[,] _mapUserImages = new string[,]
		{	{ "201", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO" },
			{ "202", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO" },
			{ "203", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO" },
			{ "204", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO" },
			{ "205", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO" },
			{ "206", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO" },
			{ "207", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO" },
			{ "208", "{HeliosFalcon}/Images/Maps/Korea/Map.jpg", "1", "Korea KTO" }, };


		public MapControls(string baseName, Size controlSize)
			: base(baseName, controlSize) {}


		#region Properties

		public interface ITargetData
		{
			double TargetBearing { get; set; }
			double TargetDistance { get; set; }
			double TargetPosition_X { get; set; }
			double TargetPosition_Y { get; set; }
			double TargetHorizontalValue { get; set; }
			double TargetVerticalValue { get; set; }
			double MapTargetHorizontalValue { get; set; }
			double MapTargetVerticalValue { get; set; }
			double CourseBearing { get; set; }
			double CourseDistance { get; set; }
		}

		public string UserMapImage_201
		{
			get
			{
				return _mapUserImages[0, 1];
			}
			set
			{
				if ((_mapUserImages[0, 1] == null && value != null)
					|| (_mapUserImages[0, 1] != null && !_mapUserImages[0, 1].Equals(value)))
				{
					_mapUserImages[0, 1] = value;
				}
			}
		}

		public string UserMapName_201
		{
			get
			{
				return _mapUserImages[0, 3];
			}
			set
			{
				if ((_mapUserImages[0, 3] == null && value != null)
					|| (_mapUserImages[0, 3] != null && !_mapUserImages[0, 3].Equals(value)))
				{
					_mapUserImages[0, 3] = value;
				}
			}
		}

		public bool UserMapSize_201
		{
			get
			{
				return Convert.ToBoolean(_mapUserImages[0, 2] == "2" ? "True" : "False");
			}
			set
			{
				string newValue = (value ? "2" : "1");

				if (_mapUserImages[0, 2] != newValue)
				{
					_mapUserImages[0, 2] = newValue;
				}
			}
		}

		public string UserMapImage_202
		{
			get
			{
				return _mapUserImages[1, 1];
			}
			set
			{
				if ((_mapUserImages[1, 1] == null && value != null)
					|| (_mapUserImages[1, 1] != null && !_mapUserImages[1, 1].Equals(value)))
				{
					_mapUserImages[1, 1] = value;
				}
			}
		}

		public string UserMapName_202
		{
			get
			{
				return _mapUserImages[1, 3];
			}
			set
			{
				if ((_mapUserImages[1, 3] == null && value != null)
					|| (_mapUserImages[1, 3] != null && !_mapUserImages[1, 3].Equals(value)))
				{
					_mapUserImages[1, 3] = value;
				}
			}
		}

		public bool UserMapSize_202
		{
			get
			{
				return Convert.ToBoolean(_mapUserImages[1, 2] == "2" ? "True" : "False");
			}
			set
			{
				string newValue = (value ? "2" : "1");

				if (_mapUserImages[1, 2] != newValue)
				{
					_mapUserImages[1, 2] = newValue;
				}
			}
		}

		public string UserMapImage_203
		{
			get
			{
				return _mapUserImages[2, 1];
			}
			set
			{
				if ((_mapUserImages[2, 1] == null && value != null)
					|| (_mapUserImages[2, 1] != null && !_mapUserImages[2, 1].Equals(value)))
				{
					_mapUserImages[2, 1] = value;
				}
			}
		}

		public string UserMapName_203
		{
			get
			{
				return _mapUserImages[2, 3];
			}
			set
			{
				if ((_mapUserImages[2, 3] == null && value != null)
					|| (_mapUserImages[2, 3] != null && !_mapUserImages[2, 3].Equals(value)))
				{
					_mapUserImages[2, 3] = value;
				}
			}
		}

		public bool UserMapSize_203
		{
			get
			{
				return Convert.ToBoolean(_mapUserImages[2, 2] == "2" ? "True" : "False");
			}
			set
			{
				string newValue = (value ? "2" : "1");

				if (_mapUserImages[2, 2] != newValue)
				{
					_mapUserImages[2, 2] = newValue;
				}
			}
		}

		public string UserMapImage_204
		{
			get
			{
				return _mapUserImages[3, 1];
			}
			set
			{
				if ((_mapUserImages[3, 1] == null && value != null)
					|| (_mapUserImages[3, 1] != null && !_mapUserImages[3, 1].Equals(value)))
				{
					_mapUserImages[3, 1] = value;
				}
			}
		}

		public string UserMapName_204
		{
			get
			{
				return _mapUserImages[3, 3];
			}
			set
			{
				if ((_mapUserImages[3, 3] == null && value != null)
					|| (_mapUserImages[3, 3] != null && !_mapUserImages[3, 3].Equals(value)))
				{
					_mapUserImages[3, 3] = value;
				}
			}
		}

		public bool UserMapSize_204
		{
			get
			{
				return Convert.ToBoolean(_mapUserImages[3, 2] == "2" ? "True" : "False");
			}
			set
			{
				string newValue = (value ? "2" : "1");

				if (_mapUserImages[3, 2] != newValue)
				{
					_mapUserImages[3, 2] = newValue;
				}
			}
		}

		public string UserMapImage_205
		{
			get
			{
				return _mapUserImages[4, 1];
			}
			set
			{
				if ((_mapUserImages[4, 1] == null && value != null)
					|| (_mapUserImages[4, 1] != null && !_mapUserImages[4, 1].Equals(value)))
				{
					_mapUserImages[4, 1] = value;
				}
			}
		}

		public string UserMapName_205
		{
			get
			{
				return _mapUserImages[4, 3];
			}
			set
			{
				if ((_mapUserImages[4, 3] == null && value != null)
					|| (_mapUserImages[4, 3] != null && !_mapUserImages[4, 3].Equals(value)))
				{
					_mapUserImages[4, 3] = value;
				}
			}
		}

		public bool UserMapSize_205
		{
			get
			{
				return Convert.ToBoolean(_mapUserImages[4, 2] == "2" ? "True" : "False");
			}
			set
			{
				string newValue = (value ? "2" : "1");

				if (_mapUserImages[4, 2] != newValue)
				{
					_mapUserImages[4, 2] = newValue;
				}
			}
		}

		public string UserMapImage_206
		{
			get
			{
				return _mapUserImages[5, 1];
			}
			set
			{
				if ((_mapUserImages[5, 1] == null && value != null)
					|| (_mapUserImages[5, 1] != null && !_mapUserImages[5, 1].Equals(value)))
				{
					_mapUserImages[5, 1] = value;
				}
			}
		}

		public string UserMapName_206
		{
			get
			{
				return _mapUserImages[5, 3];
			}
			set
			{
				if ((_mapUserImages[5, 3] == null && value != null)
					|| (_mapUserImages[5, 3] != null && !_mapUserImages[5, 3].Equals(value)))
				{
					_mapUserImages[5, 3] = value;
				}
			}
		}

		public bool UserMapSize_206
		{
			get
			{
				return Convert.ToBoolean(_mapUserImages[5, 2] == "2" ? "True" : "False");
			}
			set
			{
				string newValue = (value ? "2" : "1");

				if (_mapUserImages[5, 2] != newValue)
				{
					_mapUserImages[5, 2] = newValue;
				}
			}
		}

		public string UserMapImage_207
		{
			get
			{
				return _mapUserImages[6, 1];
			}
			set
			{
				if ((_mapUserImages[6, 1] == null && value != null)
					|| (_mapUserImages[6, 1] != null && !_mapUserImages[6, 1].Equals(value)))
				{
					_mapUserImages[6, 1] = value;
				}
			}
		}

		public string UserMapName_207
		{
			get
			{
				return _mapUserImages[6, 3];
			}
			set
			{
				if ((_mapUserImages[6, 3] == null && value != null)
					|| (_mapUserImages[6, 3] != null && !_mapUserImages[6, 3].Equals(value)))
				{
					_mapUserImages[6, 3] = value;
				}
			}
		}

		public bool UserMapSize_207
		{
			get
			{
				return Convert.ToBoolean(_mapUserImages[6, 2] == "2" ? "True" : "False");
			}
			set
			{
				string newValue = (value ? "2" : "1");

				if (_mapUserImages[6, 2] != newValue)
				{
					_mapUserImages[6, 2] = newValue;
				}
			}
		}

		public string UserMapImage_208
		{
			get
			{
				return _mapUserImages[7, 1];
			}
			set
			{
				if ((_mapUserImages[7, 1] == null && value != null)
					|| (_mapUserImages[7, 1] != null && !_mapUserImages[7, 1].Equals(value)))
				{
					_mapUserImages[7, 1] = value;
				}
			}
		}

		public string UserMapName_208
		{
			get
			{
				return _mapUserImages[7, 3];
			}
			set
			{
				if ((_mapUserImages[7, 3] == null && value != null)
					|| (_mapUserImages[7, 3] != null && !_mapUserImages[7, 3].Equals(value)))
				{
					_mapUserImages[7, 3] = value;
				}
			}
		}

		public bool UserMapSize_208
		{
			get
			{
				return Convert.ToBoolean(_mapUserImages[7, 2] == "2" ? "True" : "False");
			}
			set
			{
				string newValue = (value ? "2" : "1");

				if (_mapUserImages[7, 2] != newValue)
				{
					_mapUserImages[7, 2] = newValue;
				}
			}
		}

		public string BaseMapName_101
		{
			get
			{
				return _mapBaseImages[0, 3];
			}
			set
			{
				if ((_mapBaseImages[0, 3] == null && value != null)
					|| (_mapBaseImages[0, 3] != null && !_mapBaseImages[0, 3].Equals(value)))
				{
					_mapBaseImages[0, 3] = value;
				}
			}
		}

		public string BaseMapName_102
		{
			get
			{
				return _mapBaseImages[1, 3];
			}
			set
			{
				if ((_mapBaseImages[1, 3] == null && value != null)
					|| (_mapBaseImages[1, 3] != null && !_mapBaseImages[1, 3].Equals(value)))
				{
					_mapBaseImages[1, 3] = value;
				}
			}
		}

		public string BaseMapName_103
		{
			get
			{
				return _mapBaseImages[2, 3];
			}
			set
			{
				if ((_mapBaseImages[2, 3] == null && value != null)
					|| (_mapBaseImages[2, 3] != null && !_mapBaseImages[2, 3].Equals(value)))
				{
					_mapBaseImages[2, 3] = value;
				}
			}
		}

		public string BaseMapName_104
		{
			get
			{
				return _mapBaseImages[3, 3];
			}
			set
			{
				if ((_mapBaseImages[3, 3] == null && value != null)
					|| (_mapBaseImages[3, 3] != null && !_mapBaseImages[3, 3].Equals(value)))
				{
					_mapBaseImages[3, 3] = value;
				}
			}
		}

		public string BaseMapName_105
		{
			get
			{
				return _mapBaseImages[4, 3];
			}
			set
			{
				if ((_mapBaseImages[4, 3] == null && value != null)
					|| (_mapBaseImages[4, 3] != null && !_mapBaseImages[4, 3].Equals(value)))
				{
					_mapBaseImages[4, 3] = value;
				}
			}
		}

		public string BaseMapName_106
		{
			get
			{
				return _mapBaseImages[5, 3];
			}
			set
			{
				if ((_mapBaseImages[5, 3] == null && value != null)
					|| (_mapBaseImages[5, 3] != null && !_mapBaseImages[5, 3].Equals(value)))
				{
					_mapBaseImages[5, 3] = value;
				}
			}
		}

		public string BaseMapName_107
		{
			get
			{
				return _mapBaseImages[6, 3];
			}
			set
			{
				if ((_mapBaseImages[6, 3] == null && value != null)
					|| (_mapBaseImages[6, 3] != null && !_mapBaseImages[6, 3].Equals(value)))
				{
					_mapBaseImages[6, 3] = value;
				}
			}
		}

		public string BaseMapName_108
		{
			get
			{
				return _mapBaseImages[7, 3];
			}
			set
			{
				if ((_mapBaseImages[7, 3] == null && value != null)
					|| (_mapBaseImages[7, 3] != null && !_mapBaseImages[7, 3].Equals(value)))
				{
					_mapBaseImages[7, 3] = value;
				}
			}
		}

		public string BaseMapName_109
		{
			get
			{
				return _mapBaseImages[8, 3];
			}
			set
			{
				if ((_mapBaseImages[8, 3] == null && value != null)
					|| (_mapBaseImages[8, 3] != null && !_mapBaseImages[8, 3].Equals(value)))
				{
					_mapBaseImages[8, 3] = value;
				}
			}
		}

		public string BaseMapName_110
		{
			get
			{
				return _mapBaseImages[9, 3];
			}
			set
			{
				if ((_mapBaseImages[9, 3] == null && value != null)
					|| (_mapBaseImages[9, 3] != null && !_mapBaseImages[9, 3].Equals(value)))
				{
					_mapBaseImages[9, 3] = value;
				}
			}
		}

		public string BaseMapName_111
		{
			get
			{
				return _mapBaseImages[10, 3];
			}
			set
			{
				if ((_mapBaseImages[10, 3] == null && value != null)
					|| (_mapBaseImages[10, 3] != null && !_mapBaseImages[10, 3].Equals(value)))
				{
					_mapBaseImages[10, 3] = value;
				}
			}
		}

		public string BaseMapName_112
		{
			get
			{
				return _mapBaseImages[11, 3];
			}
			set
			{
				if ((_mapBaseImages[11, 3] == null && value != null)
					|| (_mapBaseImages[11, 3] != null && !_mapBaseImages[11, 3].Equals(value)))
				{
					_mapBaseImages[11, 3] = value;
				}
			}
		}

		public string BaseMapName_113
		{
			get
			{
				return _mapBaseImages[12, 3];
			}
			set
			{
				if ((_mapBaseImages[12, 3] == null && value != null)
					|| (_mapBaseImages[12, 3] != null && !_mapBaseImages[12, 3].Equals(value)))
				{
					_mapBaseImages[12, 3] = value;
				}
			}
		}

		public string BaseMapName_114
		{
			get
			{
				return _mapBaseImages[13, 3];
			}
			set
			{
				if ((_mapBaseImages[13, 3] == null && value != null)
					|| (_mapBaseImages[13, 3] != null && !_mapBaseImages[13, 3].Equals(value)))
				{
					_mapBaseImages[13, 3] = value;
				}
			}
		}

		public string BaseMapName_115
		{
			get
			{
				return _mapBaseImages[14, 3];
			}
			set
			{
				if ((_mapBaseImages[14, 3] == null && value != null)
					|| (_mapBaseImages[14, 3] != null && !_mapBaseImages[14, 3].Equals(value)))
				{
					_mapBaseImages[14, 3] = value;
				}
			}
		}

		public string BaseMapName_116
		{
			get
			{
				return _mapBaseImages[15, 3];
			}
			set
			{
				if ((_mapBaseImages[15, 3] == null && value != null)
					|| (_mapBaseImages[15, 3] != null && !_mapBaseImages[15, 3].Equals(value)))
				{
					_mapBaseImages[15, 3] = value;
				}
			}
		}

		public string BaseMapName_117
		{
			get
			{
				return _mapBaseImages[16, 3];
			}
			set
			{
				if ((_mapBaseImages[16, 3] == null && value != null)
					|| (_mapBaseImages[16, 3] != null && !_mapBaseImages[16, 3].Equals(value)))
				{
					_mapBaseImages[16, 3] = value;
				}
			}
		}

		public string BaseMapName_118
		{
			get
			{
				return _mapBaseImages[17, 3];
			}
			set
			{
				if ((_mapBaseImages[17, 3] == null && value != null)
					|| (_mapBaseImages[17, 3] != null && !_mapBaseImages[17, 3].Equals(value)))
				{
					_mapBaseImages[17, 3] = value;
				}
			}
		}

		public string BaseMapName_119
		{
			get
			{
				return _mapBaseImages[18, 3];
			}
			set
			{
				if ((_mapBaseImages[18, 3] == null && value != null)
					|| (_mapBaseImages[18, 3] != null && !_mapBaseImages[18, 3].Equals(value)))
				{
					_mapBaseImages[18, 3] = value;
				}
			}
		}

		public string BaseMapName_120
		{
			get
			{
				return _mapBaseImages[19, 3];
			}
			set
			{
				if ((_mapBaseImages[19, 3] == null && value != null)
					|| (_mapBaseImages[19, 3] != null && !_mapBaseImages[19, 3].Equals(value)))
				{
					_mapBaseImages[19, 3] = value;
				}
			}
		}

		public string BaseMapName_121
		{
			get
			{
				return _mapBaseImages[20, 3];
			}
			set
			{
				if ((_mapBaseImages[20, 3] == null && value != null)
					|| (_mapBaseImages[20, 3] != null && !_mapBaseImages[20, 3].Equals(value)))
				{
					_mapBaseImages[20, 3] = value;
				}
			}
		}

		public string BaseMapName_122
		{
			get
			{
				return _mapBaseImages[21, 3];
			}
			set
			{
				if ((_mapBaseImages[21, 3] == null && value != null)
					|| (_mapBaseImages[21, 3] != null && !_mapBaseImages[21, 3].Equals(value)))
				{
					_mapBaseImages[21, 3] = value;
				}
			}
		}

		public string BaseMapName_123
		{
			get
			{
				return _mapBaseImages[22, 3];
			}
			set
			{
				if ((_mapBaseImages[22, 3] == null && value != null)
					|| (_mapBaseImages[22, 3] != null && !_mapBaseImages[22, 3].Equals(value)))
				{
					_mapBaseImages[22, 3] = value;
				}
			}
		}

		public string BaseMapName_124
		{
			get
			{
				return _mapBaseImages[23, 3];
			}
			set
			{
				if ((_mapBaseImages[23, 3] == null && value != null)
					|| (_mapBaseImages[23, 3] != null && !_mapBaseImages[23, 3].Equals(value)))
				{
					_mapBaseImages[23, 3] = value;
				}
			}
		}

		#endregion Properties


		#region Read/Write Xml

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);

			writer.WriteElementString("UserMapImage_201", UserMapImage_201);
			writer.WriteElementString("UserMapName_201", UserMapName_201);
			writer.WriteElementString("UserMapSize_201", UserMapSize_201.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("UserMapImage_202", UserMapImage_202);
			writer.WriteElementString("UserMapName_202", UserMapName_202);
			writer.WriteElementString("UserMapSize_202", UserMapSize_202.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("UserMapImage_203", UserMapImage_203);
			writer.WriteElementString("UserMapName_203", UserMapName_203);
			writer.WriteElementString("UserMapSize_203", UserMapSize_203.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("UserMapImage_204", UserMapImage_204);
			writer.WriteElementString("UserMapName_204", UserMapName_204);
			writer.WriteElementString("UserMapSize_204", UserMapSize_204.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("UserMapImage_205", UserMapImage_205);
			writer.WriteElementString("UserMapName_205", UserMapName_205);
			writer.WriteElementString("UserMapSize_205", UserMapSize_205.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("UserMapImage_206", UserMapImage_206);
			writer.WriteElementString("UserMapName_206", UserMapName_206);
			writer.WriteElementString("UserMapSize_206", UserMapSize_206.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("UserMapImage_207", UserMapImage_207);
			writer.WriteElementString("UserMapName_207", UserMapName_207);
			writer.WriteElementString("UserMapSize_207", UserMapSize_207.ToString(CultureInfo.InvariantCulture));
			writer.WriteElementString("UserMapImage_208", UserMapImage_208);
			writer.WriteElementString("UserMapName_208", UserMapName_208);
			writer.WriteElementString("UserMapSize_208", UserMapSize_208.ToString(CultureInfo.InvariantCulture));

			writer.WriteElementString("BaseMapName_101", BaseMapName_101);
			writer.WriteElementString("BaseMapName_102", BaseMapName_102);
			writer.WriteElementString("BaseMapName_103", BaseMapName_103);
			writer.WriteElementString("BaseMapName_104", BaseMapName_104);
			writer.WriteElementString("BaseMapName_105", BaseMapName_105);
			writer.WriteElementString("BaseMapName_106", BaseMapName_106);
			writer.WriteElementString("BaseMapName_107", BaseMapName_107);
			writer.WriteElementString("BaseMapName_108", BaseMapName_108);
			writer.WriteElementString("BaseMapName_109", BaseMapName_109);
			writer.WriteElementString("BaseMapName_110", BaseMapName_110);
			writer.WriteElementString("BaseMapName_111", BaseMapName_111);
			writer.WriteElementString("BaseMapName_112", BaseMapName_112);
			writer.WriteElementString("BaseMapName_113", BaseMapName_113);
			writer.WriteElementString("BaseMapName_114", BaseMapName_114);
			writer.WriteElementString("BaseMapName_115", BaseMapName_115);
			writer.WriteElementString("BaseMapName_116", BaseMapName_116);
			writer.WriteElementString("BaseMapName_117", BaseMapName_117);
			writer.WriteElementString("BaseMapName_118", BaseMapName_118);
			writer.WriteElementString("BaseMapName_119", BaseMapName_119);
			writer.WriteElementString("BaseMapName_120", BaseMapName_120);
			writer.WriteElementString("BaseMapName_121", BaseMapName_121);
			writer.WriteElementString("BaseMapName_122", BaseMapName_122);
			writer.WriteElementString("BaseMapName_123", BaseMapName_123);
			writer.WriteElementString("BaseMapName_124", BaseMapName_124);
		}

		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);

			UserMapImage_201 = reader.ReadElementString("UserMapImage_201");
			UserMapName_201 = reader.ReadElementString("UserMapName_201");
			UserMapSize_201 = bool.Parse(reader.ReadElementString("UserMapSize_201"));
			UserMapImage_202 = reader.ReadElementString("UserMapImage_202");
			UserMapName_202 = reader.ReadElementString("UserMapName_202");
			UserMapSize_202 = bool.Parse(reader.ReadElementString("UserMapSize_202"));
			UserMapImage_203 = reader.ReadElementString("UserMapImage_203");
			UserMapName_203 = reader.ReadElementString("UserMapName_203");
			UserMapSize_203 = bool.Parse(reader.ReadElementString("UserMapSize_203"));
			UserMapImage_204 = reader.ReadElementString("UserMapImage_204");
			UserMapName_204 = reader.ReadElementString("UserMapName_204");
			UserMapSize_204 = bool.Parse(reader.ReadElementString("UserMapSize_204"));
			UserMapImage_205 = reader.ReadElementString("UserMapImage_205");
			UserMapName_205 = reader.ReadElementString("UserMapName_205");
			UserMapSize_205 = bool.Parse(reader.ReadElementString("UserMapSize_205"));
			UserMapImage_206 = reader.ReadElementString("UserMapImage_206");
			UserMapName_206 = reader.ReadElementString("UserMapName_206");
			UserMapSize_206 = bool.Parse(reader.ReadElementString("UserMapSize_206"));
			UserMapImage_207 = reader.ReadElementString("UserMapImage_207");
			UserMapName_207 = reader.ReadElementString("UserMapName_207");
			UserMapSize_207 = bool.Parse(reader.ReadElementString("UserMapSize_207"));
			UserMapImage_208 = reader.ReadElementString("UserMapImage_208");
			UserMapName_208 = reader.ReadElementString("UserMapName_208");
			UserMapSize_208 = bool.Parse(reader.ReadElementString("UserMapSize_208"));

			BaseMapName_101 = reader.ReadElementString("BaseMapName_101");
			BaseMapName_102 = reader.ReadElementString("BaseMapName_102");
			BaseMapName_103 = reader.ReadElementString("BaseMapName_103");
			BaseMapName_104 = reader.ReadElementString("BaseMapName_104");
			BaseMapName_105 = reader.ReadElementString("BaseMapName_105");
			BaseMapName_106 = reader.ReadElementString("BaseMapName_106");
			BaseMapName_107 = reader.ReadElementString("BaseMapName_107");
			BaseMapName_108 = reader.ReadElementString("BaseMapName_108");
			BaseMapName_109 = reader.ReadElementString("BaseMapName_109");
			BaseMapName_110 = reader.ReadElementString("BaseMapName_110");
			BaseMapName_111 = reader.ReadElementString("BaseMapName_111");
			BaseMapName_112 = reader.ReadElementString("BaseMapName_112");
			BaseMapName_113 = reader.ReadElementString("BaseMapName_113");
			BaseMapName_114 = reader.ReadElementString("BaseMapName_114");
			BaseMapName_115 = reader.ReadElementString("BaseMapName_115");
			BaseMapName_116 = reader.ReadElementString("BaseMapName_116");
			BaseMapName_117 = reader.ReadElementString("BaseMapName_117");
			BaseMapName_118 = reader.ReadElementString("BaseMapName_118");
			BaseMapName_119 = reader.ReadElementString("BaseMapName_119");
			BaseMapName_120 = reader.ReadElementString("BaseMapName_120");
			BaseMapName_121 = reader.ReadElementString("BaseMapName_121");
			BaseMapName_122 = reader.ReadElementString("BaseMapName_122");
			BaseMapName_123 = reader.ReadElementString("BaseMapName_123");
			BaseMapName_124 = reader.ReadElementString("BaseMapName_124");

			Refresh();
		}

		#endregion Read/Write Xml

	}
}
