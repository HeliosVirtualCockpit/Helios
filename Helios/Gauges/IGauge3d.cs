namespace GadrocsWorkshop.Helios.Controls.Capabilities
{
    using GadrocsWorkshop.Helios.Gauges;
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;


    public interface IGauge3d
    {
        #region Properties
        string Image { get; set; }
        double BasePitch { get; set; }
        double BaseRoll { get; set; }
        double BaseYaw { get; set; }
        double Pitch { get; set; }
        double Roll { get; set; }
        double Yaw { get; set; }
        double FieldOfView { get; set; }
        Color LightingColor { get; set; }
        Color LightingColorAlt { get; set; }
        bool LightingAltEnabled { get; set; }
        double LightingBrightness { get; set; }
        double LightingAltBrightness { get; set; }
        double LightingX { get; set; }
        double LightingY { get; set; }
        double LightingZ { get; set; }
        void ScaleChildren(double scaleX, double scaleY);
        void Reset();

        #endregion
    }
}
