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

namespace GadrocsWorkshop.Helios.ProfileEditor
{
    using GadrocsWorkshop.Helios.ProfileEditor.UndoEvents;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    class VisualContainerDropAdvisor : IDropTargetAdvisor
    {
        private HeliosVisualContainerEditor _target;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();


        public System.Windows.UIElement TargetUI
        {
            get
            {
                return _target;
            }
            set
            {
                _target = value as HeliosVisualContainerEditor;
            }
        }

        public bool ApplyMouseOffset
        {
            get { return true; }
        }

        public bool IsValidDataObject(IDataObject dataObject, Point dropPoint)
        {
            bool valid = dataObject.GetDataPresent("Helios.Visual");
            if (valid)
            {
                HeliosVisual item = dataObject.GetData("Helios.Visual") as HeliosVisual;

                _target.LoadSnapTargets(false);
                _target.SnapManager.Action = SnapAction.Drop;
                _target.SnapManager.Size = item.DisplayRectangle.Size;
                _target.SnapManager.DragVector = new Vector(0, 0);

            }
            return valid;
        }

        public Point GetVisualFeedbackLocation(Point location)
        {
            _target.SnapManager.IgnoreTargets = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            _target.SnapManager.Location = location;
            return _target.SnapManager.NewLocation;
        }

        public void OnDropCompleted(IDataObject obj, Point dragPoint)
        {
            HeliosVisual item = obj.GetData("Helios.Visual") as HeliosVisual;

            if (item == null)
            {
                return;
            }
            if (item.IsUnique && _target.VisualContainer.Children.Where(visual => visual.TypeIdentifier == item.TypeIdentifier)
                   .Select(visual => visual.TypeIdentifier)
                   .Count() > 0)
            {
                Logger.Debug($"Dropping second instance of unique type {item.TypeIdentifier} refused.");
                return;
            }
            if (Math.Abs(_target.ZoomFactor) < 0.0001)
            {
                return;
            }
            Point scaledPoint = new Point(dragPoint.X / _target.ZoomFactor, dragPoint.Y / _target.ZoomFactor); 
            _target.SnapManager.Location = scaledPoint;

            item.Left = Math.Max(0d, _target.SnapManager.NewLocation.X);
            item.Top = Math.Max(0d, _target.SnapManager.NewLocation.Y);
            item.Name = _target.VisualContainer.Children.GetUniqueName(item);
            _target.VisualContainer.Children.Add(item);
            _target.SelectedItems.Clear();
            _target.SelectedItems.Add(item);
            _target.Focus();

            ConfigManager.UndoManager.AddUndoItem(new ControlAddUndoEvent(_target.VisualContainer, item));
        }

        public UIElement GetVisualFeedback(IDataObject obj)
        {
            HeliosVisual item = obj.GetData("Helios.Visual") as HeliosVisual;

            if (item == null)
            {
                return null;
            }

            HeliosVisualView view = new HeliosVisualView();
            view.Visual = item;
            view.Width = item.Width * _target.ZoomFactor;
            view.Height = item.Height * _target.ZoomFactor;
            return view;            
        }

        public UIElement GetTargetTopContainer()
        {
            return _target;
        }
    }
}
