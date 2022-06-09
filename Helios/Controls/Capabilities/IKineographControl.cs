﻿namespace GadrocsWorkshop.Helios.Controls.Capabilities
{
    using System;
    using System.Windows;
    using System.Windows.Media;
    using System.Collections.Generic;


    public interface IKineographControl
    {
        #region Properties
        string AnimationFrameImageNamePattern { get; set; }
        int AnimationFrameNumber { get; set; }
        int AnimationFrameCount { get; set; }
        List<ImageSource> AnimationFrames { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        #endregion
    }
}
