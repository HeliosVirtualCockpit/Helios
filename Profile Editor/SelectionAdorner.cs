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
    using GadrocsWorkshop.Helios.Controls;
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    public class SelectionAdorner : Adorner
    {
        private VisualCollection _children;

        private readonly Pen SelectionBorderPen = new Pen(Brushes.AliceBlue, 1d);
        private readonly Pen SelectionRectPen = new Pen(Brushes.Teal, 1.5d);
        private readonly Brush SelectionResizeBrush = new SolidColorBrush(Colors.Aquamarine);

        private readonly Pen SelectionRectPenNoFocus = new Pen(Brushes.DarkGray, 1.5d);
        private readonly Brush SelectionResizeBrushNoFocus = new SolidColorBrush(Colors.LightGray);

        private const double ResizeRadius = 5.0;
        private const double ResizeDiameter = ResizeRadius * 2d;

        private Thumb _topLeft;
        private Thumb _topRight;
        private Thumb _bottomLeft;
        private Thumb _bottomRight;

        private bool _drawFocus = true;

        private HeliosVisualContainerEditor _editor;

        private bool _isline = false;
        private LineDecoration _tempLine;

        public SelectionAdorner(HeliosVisualContainerEditor adornedElement)
            : base(adornedElement)
        {
            if (adornedElement.SelectedItems == null)
            {
                throw new Exception("timing bug encountered in processing selections; editor cannot be null");
            }
            _children = new VisualCollection(this);
            _editor = adornedElement;
            _editor.SelectedItems.CollectionSizeChanged += new EventHandler(SelectedItems_PropertyChanged);
            _editor.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(Editor_GotKeyboardFocus);
            _editor.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(Editor_LostKeyboardFocus);

            _drawFocus = Keyboard.FocusedElement?.Equals(_editor) ?? false;

            SelectionBorderPen.DashStyle = DashStyles.Dash;
            SelectionResizeBrush.Opacity = 0.5d;

            BuildAdornerCorner(ref _topLeft, Cursors.SizeNWSE, new DragStartedEventHandler(TopLeft_DragStarted));
            BuildAdornerCorner(ref _topRight, Cursors.SizeNESW, new DragStartedEventHandler(TopRight_DragStarted));
            BuildAdornerCorner(ref _bottomLeft, Cursors.SizeNESW, new DragStartedEventHandler(BottomLeft_DragStarted));
            BuildAdornerCorner(ref _bottomRight, Cursors.SizeNWSE, new DragStartedEventHandler(BottomRight_DragStarted));

            CheckLine();
        }

        private void Editor_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!_drawFocus)
            {
                _drawFocus = true;
                InvalidateVisual();
            }
        }

        private void Editor_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (_drawFocus)
            {
                _drawFocus = false;
                InvalidateVisual();
            }
        }

        void SelectedItems_PropertyChanged(object sender, EventArgs e)
        {
            if (CheckAccess())
            {
                InvalidateAll();
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(InvalidateAll));
            }
        }

        private void InvalidateAll()
        {
            CheckLine();

            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }

        private void CheckLine()
        {
            if (_editor.SelectedItems != null && _editor.SelectedItems.Count == 1 && _editor.SelectedItems[0] is Helios.Controls.LineDecoration)
            {
                if (!_isline)
                {
                    _children.Remove(_bottomLeft);
                    _children.Remove(_bottomRight);
                    _topRight.Cursor = Cursors.Cross;
                    _topLeft.Cursor = Cursors.Cross;
                }
                Helios.Controls.LineDecoration line = _editor.SelectedItems[0] as Helios.Controls.LineDecoration;
                _tempLine = new Helios.Controls.LineDecoration();
                _tempLine.Clone(line);
                _isline = true;
            }
            else
            {
                if (_isline)
                {
                    _children.Add(_bottomLeft);
                    _children.Add(_bottomRight);
                    _topRight.Cursor = Cursors.SizeNESW;
                    _topLeft.Cursor = Cursors.SizeNWSE;
                }
                _tempLine = null;
                _isline = false;
            }
        }

        void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor, DragStartedEventHandler handler)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb();

            // Set some arbitrary visual characteristics.
            cornerThumb.Cursor = customizedCursor;
            cornerThumb.Height = cornerThumb.Width = 10;
            cornerThumb.Opacity = 0.00;
            cornerThumb.DragStarted += handler;
            cornerThumb.DragDelta += new DragDeltaEventHandler(HandleDragDelta);
            cornerThumb.DragCompleted += new DragCompletedEventHandler(HandleDragCompleted);

            _children.Add(cornerThumb);
        }

        #region Visual Methods

        protected override int VisualChildrenCount
        {
            get
            {
                return _children.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= _children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _children[index];
        }

        protected override Size MeasureOverride(Size constraint)
        {
            Size cornerThumbSize = new Size(10, 10);

            if (!_isline)
            {
                _topLeft.Measure(cornerThumbSize);
                _topRight.Measure(cornerThumbSize);
                _bottomLeft.Measure(cornerThumbSize);
                _bottomRight.Measure(cornerThumbSize);
            }

            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect selectionRect = _editor.SelectedItems.Rectangle;
            selectionRect.Scale(_editor.ZoomFactor, _editor.ZoomFactor);

            if (_isline)
            {
                Helios.Controls.LineDecoration line = _editor.SelectedItems[0] as Helios.Controls.LineDecoration;
                _topLeft.Arrange(new Rect((line.Start.X - ResizeRadius) * _editor.ZoomFactor, (line.Start.Y - ResizeRadius) * _editor.ZoomFactor, ResizeDiameter, ResizeDiameter));
                _topRight.Arrange(new Rect((line.End.X - ResizeRadius) * _editor.ZoomFactor, (line.End.Y - ResizeRadius) * _editor.ZoomFactor, ResizeDiameter, ResizeDiameter));
            }
            else
            {
                _topLeft.Arrange(new Rect(selectionRect.Left - ResizeRadius, selectionRect.Top - ResizeRadius, ResizeDiameter, ResizeDiameter));
                _topRight.Arrange(new Rect(selectionRect.Right - ResizeRadius, selectionRect.Top - ResizeRadius, ResizeDiameter, ResizeDiameter));
                _bottomLeft.Arrange(new Rect(selectionRect.Left - ResizeRadius, selectionRect.Bottom - ResizeRadius, ResizeDiameter, ResizeDiameter));
                _bottomRight.Arrange(new Rect(selectionRect.Right - ResizeRadius, selectionRect.Bottom - ResizeRadius, ResizeDiameter, ResizeDiameter));
            }

            // Return the final size.
            return finalSize;
        }

        #endregion

        #region Resize Handlers

        void TopLeft_DragStarted(object sender, DragStartedEventArgs e)
        {
            _editor.LoadSnapTargets(true);

            if (_isline)
            {
                Helios.Controls.LineDecoration line = _editor.SelectedItems[0] as Helios.Controls.LineDecoration;
                _editor.SnapManager.Size = new Size(1, 1);
                _editor.SnapManager.Location = line.Start;
                _editor.SnapManager.Action = SnapAction.LineStart;
                _tempLine.Clone(line);
                line.IsHidden = true;
            }
            else
            {
                _editor.SnapManager.Size = _editor.SelectedItems.Rectangle.Size;
                _editor.SnapManager.Location = _editor.SelectedItems.Rectangle.TopLeft;
                _editor.SnapManager.Action = SnapAction.ResizeNW;
            }

            _editor.SnapManager.DragVector = new Vector(0, 0);
            _editor.Focus();
        }

        void TopRight_DragStarted(object sender, DragStartedEventArgs e)
        {
            _editor.LoadSnapTargets(true);

            if (_isline)
            {
                Helios.Controls.LineDecoration line = _editor.SelectedItems[0] as Helios.Controls.LineDecoration;
                _editor.SnapManager.Size = new Size(1, 1);
                _editor.SnapManager.Location = line.End;
                _editor.SnapManager.Action = SnapAction.LineEnd;
                _tempLine.Clone(line);
                line.IsHidden = true;
            }
            else
            {
                _editor.SnapManager.Action = SnapAction.ResizeNE;
                _editor.SnapManager.Size = _editor.SelectedItems.Rectangle.Size;
                _editor.SnapManager.Location = _editor.SelectedItems.Rectangle.TopLeft;
            }

            _editor.SnapManager.DragVector = new Vector(0, 0);
            _editor.Focus();
        }

        void BottomLeft_DragStarted(object sender, DragStartedEventArgs e)
        {
            _editor.LoadSnapTargets(true);
            _editor.SnapManager.Size = _editor.SelectedItems.Rectangle.Size;
            _editor.SnapManager.Location = _editor.SelectedItems.Rectangle.TopLeft;
            _editor.SnapManager.DragVector = new Vector(0, 0);
            _editor.SnapManager.Action = SnapAction.ResizeSW;
            _editor.Focus();
        }

        void BottomRight_DragStarted(object sender, DragStartedEventArgs e)
        {
            _editor.LoadSnapTargets(true);
            _editor.SnapManager.Size = _editor.SelectedItems.Rectangle.Size;
            _editor.SnapManager.Location = _editor.SelectedItems.Rectangle.TopLeft;
            _editor.SnapManager.DragVector = new Vector(0, 0);
            _editor.SnapManager.Action = SnapAction.ResizeSE;
            _editor.Focus();
        }

        private void HandleDragDelta(object sender, DragDeltaEventArgs args)
        {
            _editor.SnapManager.ForceProportions = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
            foreach(HeliosVisual visual in _editor.SelectedItems) _editor.SnapManager.ForceProportions |= visual.KeepAspectRatio;

            _editor.SnapManager.IgnoreTargets = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
            _editor.SnapManager.DragVector = new Vector(args.HorizontalChange, args.VerticalChange);

            if (_isline)
            {
                if (_editor.SnapManager.Action == SnapAction.LineStart)
                {
                    _tempLine.Start = GetLineDragPoint(_tempLine.End, _editor.SnapManager.NewLocation);
                }
                else if (_editor.SnapManager.Action == SnapAction.LineEnd)
                {
                    _tempLine.End = GetLineDragPoint(_tempLine.Start, _editor.SnapManager.NewLocation);
                }
            }

            InvalidateVisual();
        }

        private Point GetLineDragPoint(Point otherPoint, Point dragPoint)
        {
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                if (Math.Abs(dragPoint.X - otherPoint.X) < Math.Abs(dragPoint.Y - otherPoint.Y))
                {
                    return new Point(otherPoint.X, dragPoint.Y);
                }
                else
                {
                    return new Point(dragPoint.X, otherPoint.Y);
                }
            }
            else
            {
                return dragPoint;
            }
        }

        void HandleDragCompleted(object sender, DragCompletedEventArgs e)
        {
            ConfigManager.UndoManager.StartBatch();

            if (_editor.SnapManager.Action == SnapAction.LineStart || _editor.SnapManager.Action == SnapAction.LineEnd)
            {
                Helios.Controls.LineDecoration line = _editor.SelectedItems[0] as Helios.Controls.LineDecoration;
                line.Start = _tempLine.Start;
                line.End = _tempLine.End;
                line.IsHidden = false;
            }
            else
            {
                double scaleX = Math.Max(_editor.SnapManager.NewSize.Width / _editor.SelectedItems.Rectangle.Size.Width, 0d);
                double scaleY = Math.Max(_editor.SnapManager.NewSize.Height / _editor.SelectedItems.Rectangle.Size.Height, 0d);
                ScaleVisuals(_editor.SelectedItems, _editor.SelectedItems.Rectangle, _editor.SnapManager.LocationOffset, scaleX, scaleY, _editor.SnapManager.ForceProportions);
            }

            ConfigManager.UndoManager.CloseBatch();

            _editor.SnapManager.Action = SnapAction.None;

            InvalidateVisual();
        }

        private void ScaleVisuals(HeliosVisualCollection visuals, Rect selectionRectangle, Vector offset, double scaleX, double scaleY, bool forceProportions)
        {
            foreach (HeliosVisual visual in visuals)
            {
                double lScaleX = scaleX;
                double lScaleY = scaleY;

                if (visual.Rotation != HeliosVisualRotation.None & visual.Rotation != HeliosVisualRotation.ROT180)
                {
                    lScaleX = scaleY;
                    lScaleY = scaleX;
                }

                if (visual.Left - selectionRectangle.Left > 0.1)
                {
                    double locXDif = visual.Left - selectionRectangle.Left;
                    visual.Left += (locXDif * lScaleX) - locXDif;
                }
                visual.Left += offset.X;

                if (visual.Top - selectionRectangle.Top > 0.1)
                {
                    double locYDif = visual.Top - selectionRectangle.Top;
                    visual.Top += (locYDif * scaleY) - locYDif;
                }
                visual.Top += offset.Y;


                visual.Width = Math.Max(visual.Width * lScaleX, 1d);
                visual.Height = Math.Max(visual.Height * lScaleY, 1d);

                //if (!visual.KeepAspectRatio)
                //{
                //    visual.Width = Math.Max(visual.Width * lScaleX, 1d);
                //    visual.Height = Math.Max(visual.Height * lScaleY, 1d);
                //} else
                //{
                //    /// Aspect ratio is locked, so we allow the Visual to calculate the new size
                //    /// using the dimension which has changed the most.
                //    if(Math.Abs(lScaleX-1) > Math.Abs(lScaleY-1))
                //    {
                //        visual.Width = Math.Max(visual.Width * lScaleX, 1d);
                //        lScaleY = lScaleX;
                //    }
                //    else
                //    {
                //        visual.Height = Math.Max(visual.Height * lScaleY, 1d);
                //        lScaleX = lScaleY;
                //    }
                //}

                //if (forceProportions)
                //{
                visual.ScaleChildren(lScaleX, lScaleY);
                //}
            }
        }
        #endregion

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            if (_editor.SelectedItems.Count > 0)
            {
                Pen rectPen = _drawFocus ? SelectionRectPen : SelectionRectPenNoFocus;
                Brush circleBrush = _drawFocus ? SelectionResizeBrush : SelectionResizeBrushNoFocus;

                if (_isline)
                {                    
                    Point start;
                    Point end;

                    drawingContext.PushTransform(new ScaleTransform(_editor.ZoomFactor, _editor.ZoomFactor));
                    if (_editor.SnapManager.Action == SnapAction.LineStart || _editor.SnapManager.Action == SnapAction.LineEnd)
                    {
                        drawingContext.PushTransform(new TranslateTransform(_tempLine.Left, _tempLine.Top));
                        _tempLine.Renderer.Render(drawingContext);
                        drawingContext.Pop();

                        start = _tempLine.Start;
                        end = _tempLine.End;
                    }
                    else
                    {
                        Helios.Controls.LineDecoration line = _editor.SelectedItems[0] as Helios.Controls.LineDecoration;

                        start = line.Start;
                        end = line.End;
                        if (_editor.SnapManager.Action == SnapAction.Move)
                        {
                            start += _editor.SnapManager.LocationOffset;
                            end += _editor.SnapManager.LocationOffset;
                            _tempLine.Clone(line);
                            
                            drawingContext.PushTransform(new TranslateTransform(_tempLine.Left + _editor.SnapManager.LocationOffset.X, _tempLine.Top + _editor.SnapManager.LocationOffset.Y));
                            _tempLine.Renderer.Render(drawingContext);
                            drawingContext.Pop();
                        }
                    }
                    drawingContext.Pop();

                    start.X *= _editor.ZoomFactor;
                    start.Y *= _editor.ZoomFactor;
                    end.X *= _editor.ZoomFactor;
                    end.Y *= _editor.ZoomFactor;

                    drawingContext.DrawEllipse(circleBrush, rectPen, start, ResizeRadius, ResizeRadius);
                    drawingContext.DrawEllipse(circleBrush, rectPen, end, ResizeRadius, ResizeRadius);
                }
                else
                {
                    foreach (HeliosVisual visual in _editor.SelectedItems)
                    {
                        Rect visualRect = visual.DisplayRectangle;
                        visualRect.Scale(_editor.ZoomFactor, _editor.ZoomFactor);
                        drawingContext.DrawRectangle(null, SelectionBorderPen, visualRect);
                    }

                    Rect selectionRect = (_editor.SnapManager.Action != SnapAction.None && _editor.SnapManager.Action != SnapAction.Drop && _editor.SnapManager.IsValidDrag) ? _editor.SnapManager.NewRectangle : _editor.SelectedItems.Rectangle;
                    selectionRect.Scale(_editor.ZoomFactor, _editor.ZoomFactor);

                    drawingContext.DrawRectangle(null, rectPen, selectionRect);

                    drawingContext.DrawEllipse(circleBrush, rectPen, selectionRect.TopLeft, ResizeRadius, ResizeRadius);
                    drawingContext.DrawEllipse(circleBrush, rectPen, selectionRect.TopRight, ResizeRadius, ResizeRadius);
                    drawingContext.DrawEllipse(circleBrush, rectPen, selectionRect.BottomLeft, ResizeRadius, ResizeRadius);
                    drawingContext.DrawEllipse(circleBrush, rectPen, selectionRect.BottomRight, ResizeRadius, ResizeRadius);
                }
            }
        }
    }
}
