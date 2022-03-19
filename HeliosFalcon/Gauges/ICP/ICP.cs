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

namespace GadrocsWorkshop.Helios.Gauges.Falcon.ICP
{
	using GadrocsWorkshop.Helios.ComponentModel;
	using GadrocsWorkshop.Helios.Interfaces.Falcon;
	using System;
	using System.Xml;
	using System.Windows;
	using System.Globalization;

	[HeliosControl("Helios.Falcon.ICP", "Falcon BMS ICP", "Falcon Simulator", typeof(GaugeRenderer))]
	public class ICP : BaseGauge
	{
		private enum SwitchAction
		{
			None,
			Increment,
			Decrement
		}

		private enum SwitchPosition
		{
			One,
			Two,
			Three
		}

		private enum WheelType
		{
			SYM,
			BRT,
			DEPR,
			CONT,
			DRIFT
		}

		private SwitchPosition _switchDRIFTPosition = SwitchPosition.Two;
		private WheelType _wheelType;

		private FalconInterface _falconInterface;
		private GaugeImage _backplate;
		private GaugeImage _faceplate;
		private GaugeImage _buttonCOMM1;
		private GaugeImage _buttonCOMM2;
		private GaugeImage _buttonIFF;
		private GaugeImage _buttonLIST;
		private GaugeImage _buttonAA;
		private GaugeImage _buttonAG;
		private GaugeImage _button01;
		private GaugeImage _button02;
		private GaugeImage _button03;
		private GaugeImage _button04;
		private GaugeImage _button05;
		private GaugeImage _button06;
		private GaugeImage _button07;
		private GaugeImage _button08;
		private GaugeImage _button09;
		private GaugeImage _button00;
		private GaugeImage _buttonRCL;
		private GaugeImage _buttonENTR;
		private GaugeImage _buttonWX;
		private GaugeImage _buttonDCS;
		private GaugeImage _rockerICP;
		private GaugeImage _rockerFLIR;
		private GaugeImage _wheelSYM;
		private GaugeImage _wheelBRT;
		private GaugeImage _wheelDEPR;
		private GaugeImage _wheelCONT;
		private GaugeImage _switchDRIFT;

		private HeliosTrigger _buttonCOMM1Trigger;
		private HeliosTrigger _buttonCOMM2Trigger;
		private HeliosTrigger _buttonIFFTrigger;
		private HeliosTrigger _buttonLISTTrigger;
		private HeliosTrigger _buttonAATrigger;
		private HeliosTrigger _buttonAGTrigger;
		private HeliosTrigger _button01Trigger;
		private HeliosTrigger _button02Trigger;
		private HeliosTrigger _button03Trigger;
		private HeliosTrigger _button04Trigger;
		private HeliosTrigger _button05Trigger;
		private HeliosTrigger _button06Trigger;
		private HeliosTrigger _button07Trigger;
		private HeliosTrigger _button08Trigger;
		private HeliosTrigger _button09Trigger;
		private HeliosTrigger _button00Trigger;
		private HeliosTrigger _buttonRCLTrigger;
		private HeliosTrigger _buttonENTRTrigger;
		private HeliosTrigger _buttonWXTrigger;
		private HeliosTrigger _buttonDCSUpTrigger;
		private HeliosTrigger _buttonDCSDownTrigger;
		private HeliosTrigger _buttonDCSLeftTrigger;
		private HeliosTrigger _buttonDCSRightTrigger;
		private HeliosTrigger _rockerICPUpTrigger;
		private HeliosTrigger _rockerICPDownTrigger;
		private HeliosTrigger _rockerFLIRUpTrigger;
		private HeliosTrigger _rockerFLIRDownTrigger;
		private HeliosTrigger _wheelSYMUpTrigger;
		private HeliosTrigger _wheelSYMDownTrigger;
		private HeliosTrigger _wheelBRTUpTrigger;
		private HeliosTrigger _wheelBRTDownTrigger;
		private HeliosTrigger _wheelDEPRUpTrigger;
		private HeliosTrigger _wheelDEPRDownTrigger;
		private HeliosTrigger _wheelCONTUpTrigger;
		private HeliosTrigger _wheelCONTDownTrigger;
		private HeliosTrigger _switchDRIFTPositionOneTrigger;
		private HeliosTrigger _switchDRIFTPositionTwoTrigger;
		private HeliosTrigger _switchDRIFTPositionThreeTrigger;

		Rect _rectButtonCOMM1 = new Rect(86, 14, 66, 66);
		Rect _rectButtonCOMM2 = new Rect(171, 14, 66, 66);
		Rect _rectButtonIFF = new Rect(256, 14, 66, 66);
		Rect _rectButtonLIST = new Rect(341, 14, 66, 66);
		Rect _rectButtonAA = new Rect(426, 14, 66, 66);
		Rect _rectButtonAG = new Rect(511, 14, 66, 66);
		Rect _rectButton01 = new Rect(87, 102, 66, 66);
		Rect _rectButton02 = new Rect(176, 102, 66, 66);
		Rect _rectButton03 = new Rect(269, 102, 66, 66);
		Rect _rectButton04 = new Rect(87, 194, 66, 66);
		Rect _rectButton05 = new Rect(176, 194, 66, 66);
		Rect _rectButton06 = new Rect(269, 194, 66, 66);
		Rect _rectButton07 = new Rect(87, 286, 66, 66);
		Rect _rectButton08 = new Rect(176, 286, 66, 66);
		Rect _rectButton09 = new Rect(269, 286, 66, 66);
		Rect _rectButton00 = new Rect(366, 286, 66, 66);
		Rect _rectButtonRCL = new Rect(391, 102, 66, 66);
		Rect _rectButtonENTR = new Rect(391, 194, 66, 66);
		Rect _rectButtonWX = new Rect(506, 132, 50, 50);
		Rect _rectButtonDCSCenter = new Rect(216, 373, 80, 80);
		Rect _rectButtonDCSUp = new Rect(231, 338, 50, 50);
		Rect _rectButtonDCSDown = new Rect(231, 438, 50, 50);
		Rect _rectButtonDCSLeft = new Rect(181, 388, 50, 50);
		Rect _rectButtonDCSRight = new Rect(281, 388, 50, 50);
		Rect _rectRockerICPUp = new Rect(97, 360, 60, 60);
		Rect _rectRockerICPCenter = new Rect(97, 360, 50, 120);
		Rect _rectRockerICPDown = new Rect(97, 420, 60, 60);
		Rect _rectRockerFLIRUp = new Rect(506, 200, 60, 60);
		Rect _rectRockerFLIRCenter = new Rect(506, 200, 50, 120);
		Rect _rectRockerFLIRDown = new Rect(506, 260, 60, 60);
		Rect _rectWheelSYMUp = new Rect(7, 78, 44, 75);
		Rect _rectWheelSYMCenter = new Rect(7, 78, 44, 150);
		Rect _rectWheelSYMDown = new Rect(7, 153, 44, 75);
		Rect _rectWheelBRTUp = new Rect(7, 263, 44, 75);
		Rect _rectWheelBRTCenter = new Rect(7, 263, 44, 150);
		Rect _rectWheelBRTDown = new Rect(7, 338, 44, 75);
		Rect _rectWheelDEPRUp = new Rect(607, 78, 44, 75);
		Rect _rectWheelDEPRCenter = new Rect(607, 78, 44, 150);
		Rect _rectWheelDEPRDown = new Rect(607, 153, 44, 75);
		Rect _rectWheelCONTUp = new Rect(607, 263, 44, 75);
		Rect _rectWheelCONTCenter = new Rect(607, 263, 44, 150);
		Rect _rectWheelCONTDown = new Rect(607, 338, 44, 75);
		Rect _rectSwitchDRIFT = new Rect(368, 361, 55, 110);

		private const string _backplateOffImage = "{HeliosFalcon}/Gauges/ICP/icp_backplate_off.xaml";
		private const string _backplateDimImage = "{HeliosFalcon}/Gauges/ICP/icp_backplate_dim.xaml";
		private const string _backplateBrtImage = "{HeliosFalcon}/Gauges/ICP/icp_backplate_brt.xaml";
		private const string _faceplateOffImage = "{HeliosFalcon}/Gauges/ICP/icp_faceplate_off.png";
		private const string _faceplateLitImage = "{HeliosFalcon}/Gauges/ICP/icp_faceplate_lit.png";
		private const string _buttonCOMM1Image = "{HeliosFalcon}/Gauges/ICP/icp_comm1.png";
		private const string _buttonCOMM1PressImage = "{HeliosFalcon}/Gauges/ICP/icp_comm1_press.png";
		private const string _buttonCOMM2Image = "{HeliosFalcon}/Gauges/ICP/icp_comm2.png";
		private const string _buttonCOMM2PressImage = "{HeliosFalcon}/Gauges/ICP/icp_comm2_press.png";
		private const string _buttonIFFImage = "{HeliosFalcon}/Gauges/ICP/icp_iff.png";
		private const string _buttonIFFPressImage = "{HeliosFalcon}/Gauges/ICP/icp_iff_press.png";
		private const string _buttonLISTImage = "{HeliosFalcon}/Gauges/ICP/icp_list.png";
		private const string _buttonLISTPressImage = "{HeliosFalcon}/Gauges/ICP/icp_list_press.png";
		private const string _buttonAAImage = "{HeliosFalcon}/Gauges/ICP/icp_a_a.png";
		private const string _buttonAAPressImage = "{HeliosFalcon}/Gauges/ICP/icp_a_a_press.png";
		private const string _buttonAGImage = "{HeliosFalcon}/Gauges/ICP/icp_a_g.png";
		private const string _buttonAGPressImage = "{HeliosFalcon}/Gauges/ICP/icp_a_g_press.png";
		private const string _button01Image = "{HeliosFalcon}/Gauges/ICP/icp_1.png";
		private const string _button01PressImage = "{HeliosFalcon}/Gauges/ICP/icp_1_press.png";
		private const string _button02Image = "{HeliosFalcon}/Gauges/ICP/icp_2.png";
		private const string _button02PressImage = "{HeliosFalcon}/Gauges/ICP/icp_2_press.png";
		private const string _button03Image = "{HeliosFalcon}/Gauges/ICP/icp_3.png";
		private const string _button03PressImage = "{HeliosFalcon}/Gauges/ICP/icp_3_press.png";
		private const string _button04Image = "{HeliosFalcon}/Gauges/ICP/icp_4.png";
		private const string _button04PressImage = "{HeliosFalcon}/Gauges/ICP/icp_4_press.png";
		private const string _button05Image = "{HeliosFalcon}/Gauges/ICP/icp_5.png";
		private const string _button05PressImage = "{HeliosFalcon}/Gauges/ICP/icp_5_press.png";
		private const string _button06Image = "{HeliosFalcon}/Gauges/ICP/icp_6.png";
		private const string _button06PressImage = "{HeliosFalcon}/Gauges/ICP/icp_6_press.png";
		private const string _button07Image = "{HeliosFalcon}/Gauges/ICP/icp_7.png";
		private const string _button07PressImage = "{HeliosFalcon}/Gauges/ICP/icp_7_press.png";
		private const string _button08Image = "{HeliosFalcon}/Gauges/ICP/icp_8.png";
		private const string _button08PressImage = "{HeliosFalcon}/Gauges/ICP/icp_8_press.png";
		private const string _button09Image = "{HeliosFalcon}/Gauges/ICP/icp_9.png";
		private const string _button09PressImage = "{HeliosFalcon}/Gauges/ICP/icp_9_press.png";
		private const string _button00Image = "{HeliosFalcon}/Gauges/ICP/icp_0.png";
		private const string _button00PressImage = "{HeliosFalcon}/Gauges/ICP/icp_0_press.png";
		private const string _buttonRCLImage = "{HeliosFalcon}/Gauges/ICP/icp_rcl.png";
		private const string _buttonRCLPressImage = "{HeliosFalcon}/Gauges/ICP/icp_rcl_press.png";
		private const string _buttonENTRImage = "{HeliosFalcon}/Gauges/ICP/icp_enter.png";
		private const string _buttonENTRPressImage = "{HeliosFalcon}/Gauges/ICP/icp_enter_press.png";
		private const string _buttonWXImage = "{HeliosFalcon}/Gauges/ICP/icp_wx.png";
		private const string _buttonWXPressImage = "{HeliosFalcon}/Gauges/ICP/icp_wx_press.png";
		private const string _buttonDCSCenterImage = "{HeliosFalcon}/Gauges/ICP/icp_dcs_center.png";
		private const string _buttonDCSUpImage = "{HeliosFalcon}/Gauges/ICP/icp_dcs_up.png";
		private const string _buttonDCSDownImage = "{HeliosFalcon}/Gauges/ICP/icp_dcs_down.png";
		private const string _buttonDCSLeftImage = "{HeliosFalcon}/Gauges/ICP/icp_dcs_left.png";
		private const string _buttonDCSRightImage = "{HeliosFalcon}/Gauges/ICP/icp_dcs_right.png";
		private const string _rockerUpImage = "{HeliosFalcon}/Gauges/ICP/icp_rocker_up.png";
		private const string _rockerCenterImage = "{HeliosFalcon}/Gauges/ICP/icp_rocker_center.png";
		private const string _rockerDownImage = "{HeliosFalcon}/Gauges/ICP/icp_rocker_down.png";
		private const string _wheelUpOffImage = "{HeliosFalcon}/Gauges/ICP/icp_wheel_up_off.png";
		private const string _wheelCenterOffImage = "{HeliosFalcon}/Gauges/ICP/icp_wheel_center_off.png";
		private const string _wheelDownOffImage = "{HeliosFalcon}/Gauges/ICP/icp_wheel_down_off.png";
		private const string _wheelUpLitImage = "{HeliosFalcon}/Gauges/ICP/icp_wheel_up_lit.png";
		private const string _wheelCenterLitImage = "{HeliosFalcon}/Gauges/ICP/icp_wheel_center_lit.png";
		private const string _wheelDownLitImage = "{HeliosFalcon}/Gauges/ICP/icp_wheel_down_lit.png";
		private const string _switchPositionOneImage = "{HeliosFalcon}/Gauges/ICP/icp_switch_up.png";
		private const string _switchPositionTwoImage = "{HeliosFalcon}/Gauges/ICP/icp_switch_center.png";
		private const string _switchPositionThreeImage = "{HeliosFalcon}/Gauges/ICP/icp_switch_down.png";

		private static Rect _rectBase = new Rect(0d, 0d, 660d, 500d);
		private double _backlight;
		private bool _inFlightLastValue = true;

		private Point _mouseDownLocation;
		private bool _mouseAction;
		private int _clickType = 0;

		public ICP()
			: base("ICP", new Size(_rectBase.Width, _rectBase.Height))
		{
			AddComponents();
			AddActions();
		}

		#region Components

		private void AddComponents()
		{
			_backplate = new GaugeImage(_backplateOffImage, _rectBase);
			Components.Add(_backplate);

			_faceplate = new GaugeImage(_faceplateOffImage, _rectBase);
			Components.Add(_faceplate);

			_buttonCOMM1 = new GaugeImage(_buttonCOMM1Image, _rectButtonCOMM1);
			Components.Add(_buttonCOMM1);

			_buttonCOMM2 = new GaugeImage(_buttonCOMM2Image, _rectButtonCOMM2);
			Components.Add(_buttonCOMM2);

			_buttonIFF = new GaugeImage(_buttonIFFImage, _rectButtonIFF);
			Components.Add(_buttonIFF);

			_buttonLIST = new GaugeImage(_buttonLISTImage, _rectButtonLIST);
			Components.Add(_buttonLIST);

			_buttonAA = new GaugeImage(_buttonAAImage, _rectButtonAA);
			Components.Add(_buttonAA);

			_buttonAG = new GaugeImage(_buttonAGImage, _rectButtonAG);
			Components.Add(_buttonAG);

			_button01 = new GaugeImage(_button01Image, _rectButton01);
			Components.Add(_button01);

			_button02 = new GaugeImage(_button02Image, _rectButton02);
			Components.Add(_button02);

			_button03 = new GaugeImage(_button03Image, _rectButton03);
			Components.Add(_button03);

			_button04 = new GaugeImage(_button04Image, _rectButton04);
			Components.Add(_button04);

			_button05 = new GaugeImage(_button05Image, _rectButton05);
			Components.Add(_button05);

			_button06 = new GaugeImage(_button06Image, _rectButton06);
			Components.Add(_button06);

			_button07 = new GaugeImage(_button07Image, _rectButton07);
			Components.Add(_button07);

			_button08 = new GaugeImage(_button08Image, _rectButton08);
			Components.Add(_button08);

			_button09 = new GaugeImage(_button09Image, _rectButton09);
			Components.Add(_button09);

			_button00 = new GaugeImage(_button00Image, _rectButton00);
			Components.Add(_button00);

			_buttonRCL = new GaugeImage(_buttonRCLImage, _rectButtonRCL);
			Components.Add(_buttonRCL);

			_buttonENTR = new GaugeImage(_buttonENTRImage, _rectButtonENTR);
			Components.Add(_buttonENTR);

			_buttonWX = new GaugeImage(_buttonWXImage, _rectButtonWX);
			Components.Add(_buttonWX);

			_buttonDCS = new GaugeImage(_buttonDCSCenterImage, _rectButtonDCSCenter);
			Components.Add(_buttonDCS);

			_rockerICP = new GaugeImage(_rockerCenterImage, _rectRockerICPCenter);
			Components.Add(_rockerICP);

			_rockerFLIR = new GaugeImage(_rockerCenterImage, _rectRockerFLIRCenter);
			Components.Add(_rockerFLIR);

			_wheelSYM = new GaugeImage(_wheelCenterOffImage, _rectWheelSYMCenter);
			Components.Add(_wheelSYM);

			_wheelBRT = new GaugeImage(_wheelCenterOffImage, _rectWheelBRTCenter);
			Components.Add(_wheelBRT);

			_wheelDEPR = new GaugeImage(_wheelCenterOffImage, _rectWheelDEPRCenter);
			Components.Add(_wheelDEPR);

			_wheelCONT = new GaugeImage(_wheelCenterOffImage, _rectWheelCONTCenter);
			Components.Add(_wheelCONT);

			_switchDRIFT = new GaugeImage(_switchPositionTwoImage, _rectSwitchDRIFT);
			Components.Add(_switchDRIFT);
		}

		#endregion Components

		#region Actions

		private void AddActions()
		{
			_buttonCOMM1Trigger = new HeliosTrigger(this, "", "", "Button COMM1 Pushed", "Fired when button COMM1 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonCOMM1Trigger);

			_buttonCOMM2Trigger = new HeliosTrigger(this, "", "", "Button COMM2 Pushed", "Fired when button COMM2 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonCOMM2Trigger);

			_buttonIFFTrigger = new HeliosTrigger(this, "", "", "Button IFF Pushed", "Fired when button IFF is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonIFFTrigger);

			_buttonLISTTrigger = new HeliosTrigger(this, "", "", "Button LIST Pushed", "Fired when button LIST is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonLISTTrigger);

			_buttonAATrigger = new HeliosTrigger(this, "", "", "Button A-A Pushed", "Fired when button A-A is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonAATrigger);

			_buttonAGTrigger = new HeliosTrigger(this, "", "", "Button A-G Pushed", "Fired when button A-G is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonAGTrigger);

			_button01Trigger = new HeliosTrigger(this, "", "", "Button 01 T-ILS Pushed", "Fired when button 01 T-ILS is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button01Trigger);

			_button02Trigger = new HeliosTrigger(this, "", "", "Button 02 ALOW Pushed", "Fired when button 02 ALOW is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button02Trigger);

			_button03Trigger = new HeliosTrigger(this, "", "", "Button 03 Pushed", "Fired when button 03 is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button03Trigger);

			_button04Trigger = new HeliosTrigger(this, "", "", "Button 04 STPT Pushed", "Fired when button 04 STPT is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button04Trigger);

			_button05Trigger = new HeliosTrigger(this, "", "", "Button 05 CRUS Pushed", "Fired when button 05 CRUS is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button05Trigger);

			_button06Trigger = new HeliosTrigger(this, "", "", "Button 06 TIME Pushed", "Fired when button 06 TIME is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button06Trigger);

			_button07Trigger = new HeliosTrigger(this, "", "", "Button 07 MARK Pushed", "Fired when button 07 MARK is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button07Trigger);

			_button08Trigger = new HeliosTrigger(this, "", "", "Button 08 FIX Pushed", "Fired when button 08 FIX is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button08Trigger);

			_button09Trigger = new HeliosTrigger(this, "", "", "Button 09 A-CAL Pushed", "Fired when button 09 A-CAL is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button09Trigger);

			_button00Trigger = new HeliosTrigger(this, "", "", "Button 00 M-SEL Pushed", "Fired when button 00 M-SEL is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_button00Trigger);

			_buttonRCLTrigger = new HeliosTrigger(this, "", "", "Button RCL Pushed", "Fired when button RCL is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonRCLTrigger);

			_buttonENTRTrigger = new HeliosTrigger(this, "", "", "Button ENTR Pushed", "Fired when button ENTR is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonENTRTrigger);

			_buttonWXTrigger = new HeliosTrigger(this, "", "", "Button WX Pushed", "Fired when button WX is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonWXTrigger);

			_buttonDCSUpTrigger = new HeliosTrigger(this, "", "", "Button DCS-UP Pushed", "Fired when button DCS-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonDCSUpTrigger);

			_buttonDCSDownTrigger = new HeliosTrigger(this, "", "", "Button DCS-DOWN Pushed", "Fired when button DCS-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonDCSDownTrigger);

			_buttonDCSLeftTrigger = new HeliosTrigger(this, "", "", "Button DCS-RTN Pushed", "Fired when button DCS-RTN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonDCSLeftTrigger);

			_buttonDCSRightTrigger = new HeliosTrigger(this, "", "", "Button DCS-SEQ Pushed", "Fired when button DCS-SEQ is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_buttonDCSRightTrigger);

			_rockerICPUpTrigger = new HeliosTrigger(this, "", "", "Rocker ICP-UP Pushed", "Fired when rocker ICP-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerICPUpTrigger);

			_rockerICPDownTrigger = new HeliosTrigger(this, "", "", "Rocker ICP-DOWN Pushed", "Fired when rocker ICP-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerICPDownTrigger);

			_rockerFLIRUpTrigger = new HeliosTrigger(this, "", "", "Rocker FLIR-UP Pushed", "Fired when rocker FLIR-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerFLIRUpTrigger);

			_rockerFLIRDownTrigger = new HeliosTrigger(this, "", "", "Rocker FLIR-DOWN Pushed", "Fired when rocker FLIR-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_rockerFLIRDownTrigger);

			_wheelSYMUpTrigger = new HeliosTrigger(this, "", "", "Wheel SYM-UP Pushed", "Fired when wheel SYM-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_wheelSYMUpTrigger);

			_wheelSYMDownTrigger = new HeliosTrigger(this, "", "", "Wheel SYM-DOWN Pushed", "Fired when wheel SYM-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_wheelSYMDownTrigger);

			_wheelBRTUpTrigger = new HeliosTrigger(this, "", "", "Wheel BRT-UP Pushed", "Fired when wheel BRT-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_wheelBRTUpTrigger);

			_wheelBRTDownTrigger = new HeliosTrigger(this, "", "", "Wheel BRT-DOWN Pushed", "Fired when wheel BRT-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_wheelBRTDownTrigger);

			_wheelDEPRUpTrigger = new HeliosTrigger(this, "", "", "Wheel DEPR-RET-UP Pushed", "Fired when wheel DEPR-RET-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_wheelDEPRUpTrigger);

			_wheelDEPRDownTrigger = new HeliosTrigger(this, "", "", "Wheel DEPR-RET-DOWN Pushed", "Fired when wheel DEPR-RET-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_wheelDEPRDownTrigger);

			_wheelCONTUpTrigger = new HeliosTrigger(this, "", "", "Wheel CONT-UP Pushed", "Fired when wheel CONT-UP is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_wheelCONTUpTrigger);

			_wheelCONTDownTrigger = new HeliosTrigger(this, "", "", "Wheel CONT-DOWN Pushed", "Fired when wheel CONT-DOWN is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_wheelCONTDownTrigger);

			_switchDRIFTPositionOneTrigger = new HeliosTrigger(this, "", "", "Switch DRIFT-C/O Pushed", "Fired when switch DRIFT-C/O is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_switchDRIFTPositionOneTrigger);

			_switchDRIFTPositionTwoTrigger = new HeliosTrigger(this, "", "", "Switch DRIFT-NORM Pushed", "Fired when switch DRIFT-NORM is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_switchDRIFTPositionTwoTrigger);

			_switchDRIFTPositionThreeTrigger = new HeliosTrigger(this, "", "", "Switch DRIFT-WARN-RESET Pushed", "Fired when switch DRIFT-WARN-RESET is pushed.", "Always returns true.", BindingValueUnits.Boolean);
			Triggers.Add(_switchDRIFTPositionThreeTrigger);
		}

		#endregion Actions

		#region Methods

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

		private void Profile_ProfileStarted(object sender, EventArgs e)
		{
			if (Parent.Profile.Interfaces.ContainsKey("Falcon"))
			{
				_falconInterface = Parent.Profile.Interfaces["Falcon"] as FalconInterface;
			}
		}

		private void Profile_ProfileTick(object sender, EventArgs e)
		{
			if (_falconInterface != null)
			{
				BindingValue runtimeFlying = GetValue("Runtime", "Flying");
				bool inFlight = runtimeFlying.BoolValue;

				if (inFlight)
				{
					ProcessBindingValues();
					 _inFlightLastValue = true;
				}
				else
				{
					if (_inFlightLastValue)
					{
						ResetICP();
						_inFlightLastValue = false;
					}
				}
			}
		}

		private void Profile_ProfileStopped(object sender, EventArgs e)
		{
			_falconInterface = null;
		}

		private void ProcessBindingValues()
		{
			BindingValue backlight = GetValue("Lighting", "instrument backlight");
			Backlight = backlight.DoubleValue;
		}

		private void ProcessBacklightValues()
		{
			if (Backlight == 1)
			{
				_backplate.Image = _backplateDimImage;
				_faceplate.Image = _faceplateLitImage;
			}
			else if (Backlight == 2)
			{
				_backplate.Image = _backplateBrtImage;
				_faceplate.Image = _faceplateLitImage;
			}
			else
			{
				_backplate.Image = _backplateOffImage;
				_faceplate.Image = _faceplateOffImage;
			}

			SetWheelBacklights();

			Refresh();
		}

		public override void Reset()
		{
			ResetICP();
		}

		private void ResetICP()
		{
			Backlight = 0d;
			_switchDRIFTPosition = SwitchPosition.Two;
			_switchDRIFT.Image = _switchPositionTwoImage;

			ProcessBacklightValues();
		}

		private BindingValue GetValue(string device, string name)
		{
			return _falconInterface?.GetValue(device, name) ?? BindingValue.Empty;
		}

		#endregion Methods

		#region Mouse Actions

		public override void MouseDown(Point location)
		{
			Point point = new Point(location.X * _rectBase.Width / Width, location.Y * _rectBase.Height / Height);

			_mouseAction = true;

			if (_rectButtonCOMM1.Contains(point))
			{
				_buttonCOMM1.Image = _buttonCOMM1PressImage;
				_buttonCOMM1Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonCOMM2.Contains(point))
			{
				_buttonCOMM2.Image = _buttonCOMM2PressImage;
				_buttonCOMM2Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonIFF.Contains(point))
			{
				_buttonIFF.Image = _buttonIFFPressImage;
				_buttonIFFTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonLIST.Contains(point))
			{
				_buttonLIST.Image = _buttonLISTPressImage;
				_buttonLISTTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonAA.Contains(point))
			{
				_buttonAA.Image = _buttonAAPressImage;
				_buttonAATrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonAG.Contains(point))
			{
				_buttonAG.Image = _buttonAGPressImage;
				_buttonAGTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton01.Contains(point))
			{
				_button01.Image = _button01PressImage;
				_button01Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton02.Contains(point))
			{
				_button02.Image = _button02PressImage;
				_button02Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton03.Contains(point))
			{
				_button03.Image = _button03PressImage;
				_button03Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton04.Contains(point))
			{
				_button04.Image = _button04PressImage;
				_button04Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton05.Contains(point))
			{
				_button05.Image = _button05PressImage;
				_button05Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton06.Contains(point))
			{
				_button06.Image = _button06PressImage;
				_button06Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton07.Contains(point))
			{
				_button07.Image = _button07PressImage;
				_button07Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton08.Contains(point))
			{
				_button08.Image = _button08PressImage;
				_button08Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton09.Contains(point))
			{
				_button09.Image = _button09PressImage;
				_button09Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButton00.Contains(point))
			{
				_button00.Image = _button00PressImage;
				_button00Trigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonRCL.Contains(point))
			{
				_buttonRCL.Image = _buttonRCLPressImage;
				_buttonRCLTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonENTR.Contains(point))
			{
				_buttonENTR.Image = _buttonENTRPressImage;
				_buttonENTRTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonWX.Contains(point))
			{
				_buttonWX.Image = _buttonWXPressImage;
				_buttonWXTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonDCSUp.Contains(point))
			{
				_buttonDCS.Image = _buttonDCSUpImage;
				_buttonDCSUpTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonDCSDown.Contains(point))
			{
				_buttonDCS.Image = _buttonDCSDownImage;
				_buttonDCSDownTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonDCSLeft.Contains(point))
			{
				_buttonDCS.Image = _buttonDCSLeftImage;
				_buttonDCSLeftTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectButtonDCSRight.Contains(point))
			{
				_buttonDCS.Image = _buttonDCSRightImage;
				_buttonDCSRightTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectRockerICPUp.Contains(point))
			{
				_rockerICP.Image = _rockerUpImage;
				_rockerICPUpTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectRockerICPDown.Contains(point))
			{
				_rockerICP.Image = _rockerDownImage;
				_rockerICPDownTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectRockerFLIRUp.Contains(point))
			{
				_rockerFLIR.Image = _rockerUpImage;
				_rockerFLIRUpTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectRockerFLIRDown.Contains(point))
			{
				_rockerFLIR.Image = _rockerDownImage;
				_rockerFLIRDownTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelSYMUp.Contains(point) && TouchMode)
			{
				_wheelSYM.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
				_wheelSYMUpTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelSYMDown.Contains(point) && TouchMode)
			{
				_wheelSYM.Image = Backlight == 0 ? _wheelDownOffImage : _wheelDownLitImage;
				_wheelSYMDownTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelBRTUp.Contains(point) && TouchMode)
			{
				_wheelBRT.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
				_wheelBRTUpTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelBRTDown.Contains(point) && TouchMode)
			{
				_wheelBRT.Image = Backlight == 0 ? _wheelDownOffImage : _wheelDownLitImage;
				_wheelBRTDownTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelDEPRUp.Contains(point) && TouchMode)
			{
				_wheelDEPR.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
				_wheelDEPRUpTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelDEPRDown.Contains(point) && TouchMode)
			{
				_wheelDEPR.Image = Backlight == 0 ? _wheelDownOffImage : _wheelDownLitImage;
				_wheelDEPRDownTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelCONTUp.Contains(point) && TouchMode)
			{
				_wheelCONT.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
				_wheelCONTUpTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelCONTDown.Contains(point) && TouchMode)
			{
				_wheelCONT.Image = Backlight == 0 ? _wheelDownOffImage : _wheelDownLitImage;
				_wheelCONTDownTrigger.FireTrigger(new BindingValue(true));
			}
			else if (_rectWheelSYMCenter.Contains(point) && SwipeMode)
			{
				_wheelType = WheelType.SYM;
				_mouseDownLocation = point;
				_mouseAction = false;
			}
			else if (_rectWheelBRTCenter.Contains(point) && SwipeMode)
			{
				_wheelType = WheelType.BRT;
				_mouseDownLocation = point;
				_mouseAction = false;
			}
			else if (_rectWheelDEPRCenter.Contains(point) && SwipeMode)
			{
				_wheelType = WheelType.DEPR;
				_mouseDownLocation = point;
				_mouseAction = false;
			}
			else if (_rectWheelCONTCenter.Contains(point) && SwipeMode)
			{
				_wheelType = WheelType.CONT;
				_mouseDownLocation = point;
				_mouseAction = false;
			}
			else if (_rectSwitchDRIFT.Contains(point))
			{
				_wheelType = WheelType.DRIFT;
				_mouseDownLocation = point;
				_mouseAction = false;
			}

			Refresh();
		}

		public override void MouseDrag(Point location)
		{
			Point point = new Point(location.X * _rectBase.Width / Width, location.Y * _rectBase.Height / Height);

			if (!_mouseAction)
			{
				Vector swipeVector = point - _mouseDownLocation;

				SwitchAction action = SwitchAction.None;

				if (swipeVector.Y < -5)
				{
					action = SwitchAction.Decrement;
				}
				else if (swipeVector.Y > 5)
				{
					action = SwitchAction.Increment;
				}

				if (action != SwitchAction.None)
				{
					ThrowSwitch(action);
					_mouseAction = true;
				}
			}
		}

		public override void MouseUp(Point location)
		{
			_buttonCOMM1.Image = _buttonCOMM1Image;
			_buttonCOMM2.Image = _buttonCOMM2Image;
			_buttonIFF.Image = _buttonIFFImage;
			_buttonLIST.Image = _buttonLISTImage;
			_buttonAA.Image = _buttonAAImage;
			_buttonAG.Image = _buttonAGImage;
			_button01.Image = _button01Image;
			_button02.Image = _button02Image;
			_button03.Image = _button03Image;
			_button04.Image = _button04Image;
			_button05.Image = _button05Image;
			_button06.Image = _button06Image;
			_button07.Image = _button07Image;
			_button08.Image = _button08Image;
			_button09.Image = _button09Image;
			_button00.Image = _button00Image;
			_buttonRCL.Image = _buttonRCLImage;
			_buttonENTR.Image = _buttonENTRImage;
			_buttonWX.Image = _buttonWXImage;
			_buttonDCS.Image = _buttonDCSCenterImage;
			_rockerICP.Image = _rockerCenterImage;
			_rockerFLIR.Image = _rockerCenterImage;

			SetWheelBacklights();

			if (_switchDRIFTPosition == SwitchPosition.Three)
			{
				_switchDRIFTPosition = SwitchPosition.Two;
				_switchDRIFT.Image = _switchPositionTwoImage;
				_switchDRIFTPositionTwoTrigger.FireTrigger(new BindingValue(true));
			}

			Refresh();
		}

		private void SetWheelBacklights()
		{
			_wheelSYM.Image = Backlight == 0 ? _wheelCenterOffImage : _wheelCenterLitImage;
			_wheelBRT.Image = Backlight == 0 ? _wheelCenterOffImage : _wheelCenterLitImage;
			_wheelDEPR.Image = Backlight == 0 ? _wheelCenterOffImage : _wheelCenterLitImage;
			_wheelCONT.Image = Backlight == 0 ? _wheelCenterOffImage : _wheelCenterLitImage;
		}

		private void ThrowSwitch(SwitchAction action)
		{
			switch (_wheelType)
			{
				case WheelType.SYM:
					if (action == SwitchAction.Increment)
					{
						_wheelSYM.Image = Backlight == 0 ? _wheelDownOffImage : _wheelDownLitImage;
						_wheelSYMDownTrigger.FireTrigger(new BindingValue(true));
					}
					else
					{
						_wheelSYM.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
						_wheelSYMUpTrigger.FireTrigger(new BindingValue(true));
					}
					break;
				case WheelType.BRT:
					if (action == SwitchAction.Increment)
					{
						_wheelBRT.Image = Backlight == 0 ? _wheelDownOffImage : _wheelDownLitImage;
						_wheelBRTDownTrigger.FireTrigger(new BindingValue(true));
					}
					else
					{
						_wheelBRT.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
						_wheelBRTUpTrigger.FireTrigger(new BindingValue(true));
					}
					break;
				case WheelType.DEPR:
					if (action == SwitchAction.Increment)
					{
						_wheelDEPR.Image = Backlight == 0 ? _wheelDownOffImage : _wheelDownLitImage;
						_wheelDEPRDownTrigger.FireTrigger(new BindingValue(true));
					}
					else
					{
						_wheelDEPR.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
						_wheelDEPR.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
						_wheelDEPRUpTrigger.FireTrigger(new BindingValue(true));
					}
					break;
				case WheelType.CONT:
					if (action == SwitchAction.Increment)
					{
						_wheelCONT.Image = Backlight == 0 ? _wheelDownOffImage : _wheelDownLitImage;
						_wheelCONTDownTrigger.FireTrigger(new BindingValue(true));
					}
					else
					{
						_wheelCONT.Image = Backlight == 0 ? _wheelUpOffImage : _wheelUpLitImage;
						_wheelCONTUpTrigger.FireTrigger(new BindingValue(true));
					}
					break;
				case WheelType.DRIFT:
					if (action == SwitchAction.Increment)
					{
						switch (_switchDRIFTPosition)
						{
							case SwitchPosition.One:
								_switchDRIFTPosition = SwitchPosition.Two;
								_switchDRIFT.Image = _switchPositionTwoImage;
								_switchDRIFTPositionTwoTrigger.FireTrigger(new BindingValue(true));
								break;
							case SwitchPosition.Two:
								_switchDRIFTPosition = SwitchPosition.Three;
								_switchDRIFT.Image = _switchPositionThreeImage;
								_switchDRIFTPositionThreeTrigger.FireTrigger(new BindingValue(true));
								break;
						}
					}
					else if (action == SwitchAction.Decrement)
					{
						switch (_switchDRIFTPosition)
						{
							case SwitchPosition.Two:
								_switchDRIFTPosition = SwitchPosition.One;
								_switchDRIFT.Image = _switchPositionOneImage;
								_switchDRIFTPositionOneTrigger.FireTrigger(new BindingValue(true));
								break;
							case SwitchPosition.Three:
								_switchDRIFTPosition = SwitchPosition.Two;
								_switchDRIFT.Image = _switchPositionTwoImage;
								_switchDRIFTPositionTwoTrigger.FireTrigger(new BindingValue(true));
								break;
						}
					}
					break;
			}

			Refresh();
		}

		#endregion Mouse Actions

		#region Properties

		private bool TouchMode => !Convert.ToBoolean(ClickType);

		private bool SwipeMode => Convert.ToBoolean(ClickType);

		public int ClickType
		{
			get => _clickType;

			set
			{
				if (_clickType == value)
				{
					return;
				}

				int oldValue = _clickType;
				_clickType = value;
				OnPropertyChanged("ClickType", oldValue, value, true);
			}
		}

		private double Backlight
		{
			get => _backlight;

			set
			{
				double oldValue = _backlight;
				_backlight = value;
				if (!_backlight.Equals(oldValue))
				{
					ProcessBacklightValues();
				}
			}
		}

		#endregion Properties

		#region Read/Write Xml

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);

			writer.WriteElementString("ClickType", _clickType.ToString(CultureInfo.InvariantCulture));
		}

		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);

			try
			{
				ClickType = int.Parse(reader.ReadElementString("ClickType"), CultureInfo.InvariantCulture);
			}
			catch
			{
				ClickType = 0;
			}
		}

		#endregion Read/Write Xml
	}
}
