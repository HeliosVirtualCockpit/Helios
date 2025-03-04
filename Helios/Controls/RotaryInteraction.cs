// Copyright 2014 Craig Courtney
// Copyright 2020 Helios Contributors
// 
// Helios is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Helios is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Windows;
using GadrocsWorkshop.Helios.Controls.Capabilities;
using NLog;

namespace GadrocsWorkshop.Helios.Controls
{
    /// <summary>
    /// a rotary control that implements multiple update methods
    /// </summary>
    public interface IRotaryBase : IRotaryControl, IPulsedControl
    {
        // no code
    }

    /// <summary>
    /// implementations of rotary styles from "ClickStyle" enumeration implement this interface
    /// </summary>
    public interface IRotaryInteraction
    {
        bool VisualizeDragging { get; }

        /// <summary>
        /// </summary>
        /// <param name="control"></param>
        /// <param name="location"></param>
        /// <returns>true if the control was updated</returns>
        bool Update(IRotaryBase control, Point location);
    }

    public class TouchRotaryInteraction : IRotaryInteraction
    {
        private int _lastRepeat;
        private int _lastPulse = int.MinValue;
        private bool _repeating;
        private readonly bool _increment;
        private readonly double _repeatDelay = 750d;
        private double _repeatRate = 200d;

        public int Pulses => _increment ? 1 : -1;

        public TouchRotaryInteraction(double initialControlAngle, Point centerPoint, Point location, double sensitivity)
        {
            _ = initialControlAngle;
            _ = location;
            _ = sensitivity;


            _increment = location.X > centerPoint.X;
            _repeating = false;
            _repeatRate = 200d;
            _lastRepeat = Environment.TickCount & int.MaxValue;
        }

        private bool Update(out int pulses)
        {
            int currentTick = Environment.TickCount & int.MaxValue;
            int numPulses = 0;

            if (_repeating && (currentTick < _lastPulse || currentTick - _lastPulse > _repeatRate))
            {
                numPulses += Pulses;
                _lastPulse = currentTick;
            }

            if (currentTick < _lastRepeat || currentTick - _lastRepeat > _repeatDelay)
            {
                if (_repeating && _repeatRate > 33)
                {
                    _repeatRate = _repeatRate / 2;
                    if (_repeatRate < 33)
                    {
                        _repeatRate = 33;
                    }
                }

                numPulses += Pulses;
                _lastPulse = currentTick;
                _lastRepeat = currentTick;
                _repeating = true;
            }

            pulses = numPulses;
            return numPulses != 0;
        }

        public bool VisualizeDragging => false;

        public bool Update(IRotaryBase control, Point location)
        {
            int currentTick = Environment.TickCount & int.MaxValue;
            int numPulses = 0;

            if (_repeating && (currentTick < _lastPulse || currentTick - _lastPulse > _repeatRate))
            {
                numPulses += Pulses;
                _lastPulse = currentTick;
            }

            if (currentTick < _lastRepeat || currentTick - _lastRepeat > _repeatDelay)
            {
                if (_repeating && _repeatRate > 33)
                {
                    _repeatRate = _repeatRate / 2;
                    if (_repeatRate < 33)
                    {
                        _repeatRate = 33;
                    }
                }

                numPulses += Pulses;
                _lastPulse = currentTick;
                _lastRepeat = currentTick;
                _repeating = true;
            }

            if (numPulses == 0)
            {
                return false;
            }

            control.Pulse(numPulses);
            return true;
        }
    }

    public class SwipeRotaryInteraction : IRotaryInteraction
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// calibration curve from relative sensitivity to multiplier applied to default swipe threshold angle
        /// </summary>
        private static readonly CalibrationPointCollectionDouble SwipeCalibration =
            new CalibrationPointCollectionDouble(-1d, 2d, 1d, 0.5d);

        private const double SWIPE_SENSITIVY_BASE = 45d;

        static SwipeRotaryInteraction()
        {
            SwipeCalibration.Add(new CalibrationPointDouble(0.0d, 1d));
        }

        /// <summary>
        /// the last drag location where we reported an update, against which we measure the
        /// minimum required motion based on sensitivity
        /// </summary>
        private Point _lastUpdateLocation;

        /// <summary>
        /// the center point of the control as it was when we started this interaction
        /// </summary>
        private readonly Point _centerPoint;

        /// <summary>
        /// degrees of movement after which a swipe interaction will pulse the control to move by its
        /// configured increment (control-specific)
        /// </summary>
        private readonly double _swipeThreshold = 45d;

        /// <summary>
        /// Indicates if the direction of swipe is to be reversed
        /// </summary>
        private readonly bool _reversed = false;

        public SwipeRotaryInteraction()
        {
        }

        public SwipeRotaryInteraction(double initialControlAngle, Point centerPoint, Point location, double sensitivity, bool reversed = false)
        {
            _reversed = reversed;
            _ = initialControlAngle;
            _centerPoint = centerPoint;
            _swipeThreshold = SWIPE_SENSITIVY_BASE * SwipeCalibration.Interpolate(sensitivity);
            _lastUpdateLocation = location;
        }

        public bool VisualizeDragging => false;

        private double GetAngle(Point startPoint, Point endPoint) =>
            Vector.AngleBetween(VectorFromCenter(startPoint), VectorFromCenter(endPoint));

        private Vector VectorFromCenter(Point devicePosition) => devicePosition - _centerPoint;

        public bool Update(IRotaryBase control, Point location)
        {
            double newAngle = GetAngle(_lastUpdateLocation, location);

            if (!(Math.Abs(newAngle) > _swipeThreshold))
            {
                // Logger.Debug("swipe interaction ignored change of {Degrees} against threshold of {Threshold} at {Location}", newAngle,
                // _swipeThreshold, location);
                return false;
            }

            Logger.Debug(
                "swipe interaction pulsing rotary after change of {Degrees} against threshold of {Threshold} at {Location}",
                newAngle, _swipeThreshold, location);
            _lastUpdateLocation = location;
            //control.Pulse(newAngle > 0 ? 1 : -1);
            if (!_reversed)
            {
                control.Pulse(newAngle > 0 ? 1 : -1);
            }
            else
            {
                control.Pulse(newAngle > 0 ? -1 : 1);
            }
            return true;
        }
    }

    /// <summary>
    /// Context object for one interaction using interaction style which involves dragging
    /// a line from a point within the clickable area to any point, which creates
    /// a line from the center of the control to that point.  Then the control follows the
    /// rotation that this radial spoke makes as the drag point is moved.
    /// </summary>
    public class RadialRotaryInteraction : IRotaryInteraction
    {
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// calibration curve from relative sensitivity to degrees of movement required for an update
        /// </summary>
        private static readonly CalibrationPointCollectionDouble RadialCalibration =
            new CalibrationPointCollectionDouble(-1d, 90d, 1d, 0.5d);

        static RadialRotaryInteraction()
        {
            RadialCalibration.Add(new CalibrationPointDouble(0d, 5d));
        }

        private readonly double _initialControlAngle;
        private readonly Point _centerPoint;

        /// <summary>
        /// if HasValue, then this is the drag angle at which we attached the handle to the control
        /// otherwise null to indicate we have not attached yet
        /// </summary>
        private double? _initialDragAngle;

        private Vector _lastUpdate;
        private double _rotations;
        public Point DragPoint { get; private set; }

        /// <summary>
        /// degrees of movement after which a radial interaction will update the control to match the angle from
        /// center of control to the current drag location
        /// </summary>
        private readonly double _radialThreshold;

        public RadialRotaryInteraction(double initialControlAngle, Point centerPoint, Point mouseDownLocation,
            double relativeSensitivity)
        {
            _initialControlAngle = initialControlAngle;
            _centerPoint = centerPoint;
            _radialThreshold = RadialCalibration.Interpolate(relativeSensitivity);
            DragPoint = mouseDownLocation;
        }

        public bool VisualizeDragging => _initialDragAngle.HasValue;

        public bool Update(IRotaryBase control, Point location)
        {
            // for visualization purposes, this is where we are, even if we don't update now
            DragPoint = location;

            // calculate new drag vector and change versus vector where we last updated
            Vector dragVector = location - _centerPoint;

            if (!_initialDragAngle.HasValue)
            {
                // see if we are far enough from center to start dragging
                HandleAttachMode(dragVector);
                return false;
            }

            // see if we dragged enough radially for update
            double currentAngle = Vector.AngleBetween(new Vector(0d, 1d), dragVector);
            double changeAngle = Vector.AngleBetween(_lastUpdate, dragVector);
            if (Math.Abs(changeAngle) < _radialThreshold)
            {
                // Logger.Debug("radial interaction ignored change of {Degrees} against threshold of {Threshold} at {Location}", changeAngle,
                // _radialThreshold, dragPoint);
                return false;
            }

            Logger.Debug(
                "radial interaction handle now at {Angle} after change of {Degrees} against threshold of {Threshold} at {Location}",
                currentAngle, changeAngle,
                _radialThreshold, location);

            // commit to this change
            _lastUpdate = dragVector;
            if (currentAngle - changeAngle < -180d)
            {
                // wrapped clockwise
                Logger.Debug(
                    "radial interaction wrapped clockwise to {AbsoluteDegrees} after change of {Degrees} against threshold of {Threshold} at {Location}",
                    currentAngle, changeAngle, _radialThreshold, location);
                _rotations += 360d;
            }
            else if (currentAngle - changeAngle > 180d)
            {
                // wrapped counter clockwise
                Logger.Debug(
                    "radial interaction wrapped counter-clockwise to {AbsoluteDegrees} after change of {Degrees} against threshold of {Threshold} at {Location}",
                    currentAngle, changeAngle, _radialThreshold, location);
                _rotations -= 360d;
            }

            // determine the angle the control should be at for our radial line to remain attached
            // to the control
            double newControlAngle = _initialControlAngle + currentAngle - _initialDragAngle.Value + _rotations;
            Logger.Debug(
                "radial interaction set new absolute position {AbsoluteDegrees} after change of {Degrees} against threshold of {Threshold} at {Location}",
                newControlAngle, changeAngle, _radialThreshold, location);
            control.ControlAngle = newControlAngle;

            return true;
        }

        private void HandleAttachMode(Vector dragVector)
        {
            // XXX configure
            if (dragVector.Length < 50d)
            {
                return;
            }

            _initialDragAngle = Vector.AngleBetween(new Vector(0d, 1d), dragVector);
            _lastUpdate = dragVector;
            Logger.Debug(
                "radial interaction started from {AbsoluteDegrees} using handle at {HandleAngle} with update threshold of {Threshold} at {Location}",
                _initialControlAngle, _initialDragAngle, _radialThreshold, DragPoint);
        }
    }
}