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
        double InitialAngleX { get; set; }
        double InitialAngleZ { get; set; }
        double InitialAngleY { get; set; }
        double X { get; set; }
        double Z { get; set; }
        double Y { get; set; }
        double FieldOfView { get; set; }
        Color LightingColor { get; set; }
        double LightingBrightness { get; set; }
        double LightingX { get; set; }
        double LightingY { get; set; }
        double LightingZ { get; set; }

        #endregion
        void ScaleChildren(double scaleX, double scaleY);
        void Reset();

    }
}
