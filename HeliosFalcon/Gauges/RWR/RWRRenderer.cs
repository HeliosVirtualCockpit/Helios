//  Copyright 2014 Craig Courtney
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
using System.Windows;
using System.Windows.Media;

namespace GadrocsWorkshop.Helios.Interfaces.Falcon.Gauges.RWR
{
	public class RWRRenderer : HeliosVisualRenderer
	{
		private RWR _rwr;

		private TextFormat _format;
		private Brush _scopeBrush;
		private Pen _scopePen;

		private ImageBrush _bezelBrush;
		private Rect _bezelRect = new Rect(0d, 0d, 400d, 387d);

		private Point _center = new Point (200, 193.5);
		private double _scaleX;
		private double _scaleY;

		private Rect _symbolBounds = new Rect(175, 168.5, 50, 50);
		private Rect _symbolSecondBounds = new Rect(175, 174.5, 50, 50);
		protected string[] _rwrInfo;

		public RWRRenderer()
		{
		}

		protected override void OnPropertyChanged(Helios.ComponentModel.PropertyNotificationEventArgs args)
		{
			if (args.PropertyName.Equals("Visual"))
			{
				_rwr = args.NewValue as RWR;
			}
			base.OnPropertyChanged(args);
		}

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			drawingContext.PushTransform(new ScaleTransform(_scaleX, _scaleY));

			drawingContext.DrawEllipse(Brushes.Black, null, _center, 187d, 187d);


			if (_rwr != null && _rwr.IsOn)
			{
				bool primary = DateTime.Now.Millisecond < 500;

				// Draw heart beat lines
				drawingContext.DrawLine(_scopePen, new Point(160d, 193.5), new Point(180d, 193.5d));
				drawingContext.DrawLine(_scopePen, new Point(220d, 193.5), new Point(240d, 193.5d));
				drawingContext.DrawLine(_scopePen, new Point(200d, 153.5), new Point(200d, 173.5));
				drawingContext.DrawLine(_scopePen, new Point(200d, 213.5), new Point(200d, 233.5));
				if (primary)
				{
					drawingContext.DrawLine(_scopePen, new Point(220d, 193.5d), new Point(220d, 203.5d));
				}
				else
				{
					drawingContext.DrawLine(_scopePen, new Point(220d, 183.5d), new Point(220d, 193.5d));
				}

				if (_rwr.Contacts != null)
				{
					
					foreach (RadarContact contact in _rwr.Contacts)
					{
						if (contact.Visible)
						{
							double y;
							if (contact.Lethality > 1f)
							{
								y = -((2.0f - contact.Lethality) * 178.0d);
							}
							else
							{
								//lethality values range in 1 = lowest priority to .01 = highest priority
								//substract -(1.0f - contact.Lethality) * 178.0d) from the elipse radious (187d) to get the inverse value
								//so that the symbol maps into the correct threat ring
								y = -(187d - ((1.0f - contact.Lethality) * 178.0d));
							}

							drawingContext.PushTransform(new RotateTransform(contact.RelativeBearing));
							drawingContext.PushTransform(new TranslateTransform(0d, y));
							drawingContext.PushTransform(new RotateTransform(-contact.RelativeBearing));

							if (contact.NewDetection && !primary)
							{
								_format.FontSize = 24;
							}
							else
							{
								_format.FontSize = 22;
							}

							_rwrInfo = _rwr.RwrInfo;

							string csList = new String('\0', 40);
							if (RwrInfoContains("cslst"))
							{
								csList = RwrInfoGetKeyContent("cslst").PadRight(40, '\0');
							}

							DrawContactSymbol(drawingContext, contact, primary, csList[contact.ContactId]);

							if (contact.Selected)
							{
								drawingContext.DrawLine(_scopePen, new Point(200d, 168.5d), new Point(225d, 193.5d));
								drawingContext.DrawLine(_scopePen, new Point(225d, 193.5d), new Point(200d, 218.5d));
								drawingContext.DrawLine(_scopePen, new Point(200d, 218.5d), new Point(175d, 193.5d));
								drawingContext.DrawLine(_scopePen, new Point(175d, 193.5d), new Point(200d, 168.5d));
							}

							if ((contact.MissileActivity && !contact.MissileLaunch) ||
								(contact.MissileActivity && contact.MissileLaunch && _rwr.Flash4Hz))
							{
								drawingContext.DrawEllipse(null, _scopePen, _center, 25, 25);
							}

							drawingContext.Pop();
							drawingContext.Pop();
							drawingContext.Pop();
						}
					}
				}
			}

			if (_bezelBrush != null)
			{
				drawingContext.DrawRectangle(_bezelBrush, null, _bezelRect);
			}

			drawingContext.Pop();
		}

		private string RwrInfoGetKeyContent(string key)
		{
			for (int i = 0; i < _rwrInfo.Length; i++)
			{
				if (_rwrInfo[i].StartsWith(key + ">")) return _rwrInfo[i].Replace(key + ">", string.Empty).Replace("\0", string.Empty);
			}

			return string.Empty;
		}

		private bool RwrInfoContains(string key)
		{
			for (int i = 0; i < _rwrInfo.Length; i++)
			{
				if (_rwrInfo[i].StartsWith(key + ">")) return true;
			}

			return false;
		}

		private void DrawContactSymbol(DrawingContext drawingContext, RadarContact contact, bool primary, char customSymbol)
		{
			int symbol = (int)contact.Symbol;

			switch (contact.Symbol)
			{
				case RadarSymbols.NONE:
					break;
				case RadarSymbols.UNKNOWN:
					_format.RenderText(drawingContext, _scopeBrush, "U", _symbolBounds);
					break;
				case RadarSymbols.ADVANCED_INTERCEPTOR:
					break;
				case RadarSymbols.BASIC_INTERCEPTOR:
					break;
				case RadarSymbols.ACTIVE_MISSILE:
					_format.RenderText(drawingContext, _scopeBrush, "M", _symbolBounds);
					break;
				case RadarSymbols.HAWK:
					_format.RenderText(drawingContext, _scopeBrush, "H", _symbolBounds);
					break;
				case RadarSymbols.PATRIOT:
					_format.RenderText(drawingContext, _scopeBrush, "P", _symbolBounds);
					break;
				case RadarSymbols.SA2:
					_format.RenderText(drawingContext, _scopeBrush, "2", _symbolBounds);
					break;
				case RadarSymbols.SA3:
					_format.RenderText(drawingContext, _scopeBrush, "3", _symbolBounds);
					break;
				case RadarSymbols.SA4:
					_format.RenderText(drawingContext, _scopeBrush, "4", _symbolBounds);
					break;
				case RadarSymbols.SA5:
					_format.RenderText(drawingContext, _scopeBrush, "5", _symbolBounds);
					break;
				case RadarSymbols.SA6:
					_format.RenderText(drawingContext, _scopeBrush, "6", _symbolBounds);
					break;
				case RadarSymbols.SA8:
					_format.RenderText(drawingContext, _scopeBrush, "8", _symbolBounds);
					break;
				case RadarSymbols.SA9:
					_format.RenderText(drawingContext, _scopeBrush, "9", _symbolBounds);
					break;
				case RadarSymbols.SA10:
					_format.RenderText(drawingContext, _scopeBrush, "10", _symbolBounds);
					break;
				case RadarSymbols.SA13:
					_format.RenderText(drawingContext, _scopeBrush, "13", _symbolBounds);
					break;
				case RadarSymbols.AAA:
					if (primary)
					{
						_format.RenderText(drawingContext, _scopeBrush, "A", _symbolBounds);
					}
					else
					{
						_format.RenderText(drawingContext, _scopeBrush, "S", _symbolBounds);
					}
					break;
				case RadarSymbols.NAVAL:
					break;
				case RadarSymbols.CHAPARAL:
					_format.RenderText(drawingContext, _scopeBrush, "C", _symbolBounds);
					break;
				case RadarSymbols.SA15:
					if (primary)
					{
						_format.RenderText(drawingContext, _scopeBrush, "15", _symbolBounds);
					}
					else
					{
						_format.RenderText(drawingContext, _scopeBrush, "M", _symbolBounds);
					}
					break;
				case RadarSymbols.NIKE:
					_format.RenderText(drawingContext, _scopeBrush, "N", _symbolBounds);
					break;
				case RadarSymbols.A1:
					_format.RenderText(drawingContext, _scopeBrush, "A", _symbolBounds);
					_format.RenderText(drawingContext, _scopeBrush, ".", _symbolSecondBounds);
					break;
				case RadarSymbols.A2:
					_format.RenderText(drawingContext, _scopeBrush, "A", _symbolBounds);
					_format.RenderText(drawingContext, _scopeBrush, "..", _symbolSecondBounds);
					break;
				case RadarSymbols.A3:
					_format.RenderText(drawingContext, _scopeBrush, "A", _symbolBounds);
					_format.RenderText(drawingContext, _scopeBrush, "...", _symbolSecondBounds);
					break;
				case RadarSymbols.PDOT:
					_format.RenderText(drawingContext, _scopeBrush, "P", _symbolBounds);
					_format.RenderText(drawingContext, _scopeBrush, ".", _symbolSecondBounds);
					break;
				case RadarSymbols.PSLASH:
					_format.RenderText(drawingContext, _scopeBrush, "P|", _symbolBounds);
					break;
				case RadarSymbols.UNK1:
					_format.RenderText(drawingContext, _scopeBrush, "U", _symbolBounds);
					_format.RenderText(drawingContext, _scopeBrush, ".", _symbolSecondBounds);
					break;
				case RadarSymbols.UNK2:
					_format.RenderText(drawingContext, _scopeBrush, "U", _symbolBounds);
					_format.RenderText(drawingContext, _scopeBrush, "..", _symbolSecondBounds);
					break;
				case RadarSymbols.UNK3:
					_format.RenderText(drawingContext, _scopeBrush, "U", _symbolBounds);
					_format.RenderText(drawingContext, _scopeBrush, "...", _symbolSecondBounds);
					break;
				case RadarSymbols.KSAM:
					_format.RenderText(drawingContext, _scopeBrush, "C", _symbolBounds);
					break;
				case RadarSymbols.V1:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "1", _symbolBounds);
					break;
				case RadarSymbols.V4:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "4", _symbolBounds);
					break;
				case RadarSymbols.V5:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "5", _symbolBounds);
					break;
				case RadarSymbols.V6:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "6", _symbolBounds);
					break;
				case RadarSymbols.V14:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "14", _symbolBounds);
					break;
				case RadarSymbols.V15:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "15", _symbolBounds);
					break;
				case RadarSymbols.V16:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "16", _symbolBounds);
					break;
				case RadarSymbols.V18:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "18", _symbolBounds);
					break;
				case RadarSymbols.V19:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "19", _symbolBounds);
					break;
				case RadarSymbols.V20:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "20", _symbolBounds);
					break;
				case RadarSymbols.V21:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "21", _symbolBounds);
					break;
				case RadarSymbols.V22:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "22", _symbolBounds);
					break;
				case RadarSymbols.V23:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "23", _symbolBounds);
					break;
				case RadarSymbols.V25:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "25", _symbolBounds);
					break;
				case RadarSymbols.V27:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "27", _symbolBounds);
					break;
				case RadarSymbols.V29:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "29", _symbolBounds);
					break;
				case RadarSymbols.V30:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "30", _symbolBounds);
					break;
				case RadarSymbols.V31:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "31", _symbolBounds);
					break;
				case RadarSymbols.VP:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "P", _symbolBounds);
					break;
				case RadarSymbols.VPD:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "PD", _symbolBounds);
					break;
				case RadarSymbols.VA:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "A", _symbolBounds);
					break;
				case RadarSymbols.VB:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "B", _symbolBounds);
					break;
				case RadarSymbols.VS:
					DrawCarret(drawingContext);
					_format.RenderText(drawingContext, _scopeBrush, "S", _symbolBounds);
					break;
				case RadarSymbols.SEARCH:
					_format.RenderText(drawingContext, _scopeBrush, "S", _symbolBounds);
					break;
				case RadarSymbols.Aa:
					_format.RenderText(drawingContext, _scopeBrush, "A|", _symbolBounds);
					break;
				case RadarSymbols.Ab:
					_format.RenderText(drawingContext, _scopeBrush, "|A|", _symbolBounds);
					break;
				case RadarSymbols.Ac:
					_format.RenderText(drawingContext, _scopeBrush, "|U|", _symbolBounds);
					_format.RenderText(drawingContext, _scopeBrush, "|", _symbolBounds);
					break;
				case RadarSymbols.MIB_F_S:
					if (primary)
					{
						_format.RenderText(drawingContext, _scopeBrush, "F", _symbolBounds);
					}
					else
					{
						_format.RenderText(drawingContext, _scopeBrush, "S", _symbolBounds);
					}
					break;
				case RadarSymbols.MIB_F_A:
					if (primary)
					{
						_format.RenderText(drawingContext, _scopeBrush, "F", _symbolBounds);
					}
					else
					{
						_format.RenderText(drawingContext, _scopeBrush, "A", _symbolBounds);
					}
					break;
				case RadarSymbols.MIB_F_M:
					if (primary)
					{
						_format.RenderText(drawingContext, _scopeBrush, "F", _symbolBounds);
					}
					else
					{
						_format.RenderText(drawingContext, _scopeBrush, "M", _symbolBounds);
					}
					break;
				case RadarSymbols.MIB_F_U:
					if (primary)
					{
						_format.RenderText(drawingContext, _scopeBrush, "F", _symbolBounds);
					}
					else
					{
						_format.RenderText(drawingContext, _scopeBrush, "U", _symbolBounds);
					}
					break;
				case RadarSymbols.MIB_F_BW:
					break;
				case RadarSymbols.MIB_BW_S:
					break;
				case RadarSymbols.MIB_BW_A:
					break;
				case RadarSymbols.MIB_BW_M:
					break;
				default:
					if(symbol == 66)
					{
						if (primary)
						{
							_format.RenderText(drawingContext, _scopeBrush, "F", _symbolBounds);
						}
						else
						{
							_format.RenderText(drawingContext, _scopeBrush, "A", _symbolBounds);
						}
					}
					else if(symbol >= 100)
					{
						if(customSymbol == '1')
						{
							DrawCarret(drawingContext);
							_format.RenderText(drawingContext, _scopeBrush, (symbol - 100).ToString(), _symbolBounds);
						}
						else
						{
							_format.RenderText(drawingContext, _scopeBrush, (symbol - 100).ToString(), _symbolBounds);
						}
					}
					else if(symbol >= 65)
					{
						if(customSymbol == '1')
						{
							DrawCarret(drawingContext);
							_format.RenderText(drawingContext, _scopeBrush, ((char)symbol).ToString(), _symbolBounds);
						}
						else
						{
							_format.RenderText(drawingContext, _scopeBrush, ((char)symbol).ToString(), _symbolBounds);
						}
					}
					break;
			}
		}

		private void DrawCarret(DrawingContext drawingContext)
		{
			drawingContext.DrawLine(_scopePen, new Point(200, 173.5), new Point(190, 183.5));
			drawingContext.DrawLine(_scopePen, new Point(200, 173.5), new Point(210, 183.5));
		}

		protected override void OnRefresh()
		{
			_format = new TextFormat
			{
				FontFamily = ConfigManager.FontManager.GetFontFamilyByName("Lucida Console Regular"),
				FontSize = 12,
				FontStyle = FontStyles.Normal,
				FontWeight = FontWeights.Normal,
				VerticalAlignment = TextVerticalAlignment.Center,
				HorizontalAlignment = TextHorizontalAlignment.Center
			};

			_scopeBrush = new SolidColorBrush(Color.FromRgb(0, 255, 0));
			_scopePen = new Pen(_scopeBrush, 1.5d);

			if (_rwr != null)
			{
				_scaleX = _rwr.Width / _rwr.NativeSize.Width;
				_scaleY = _rwr.Height / _rwr.NativeSize.Height;

				ImageSource image = ConfigManager.ImageManager.LoadImage(_rwr.BezelImage);
				if (image != null)
				{
					_bezelBrush = new ImageBrush(image)
					{
						Stretch = Stretch.Fill,
						TileMode = TileMode.None,
						Viewport = new Rect(0d, 0d, 1d, 1d),
						ViewportUnits = BrushMappingMode.RelativeToBoundingBox
					};
				}
				else
				{
					_bezelBrush = null;
				}
			}
			else
			{
				_bezelBrush = null;
			}
		}
	}
}
