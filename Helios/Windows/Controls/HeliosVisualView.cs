//  Copyright 2014 Craig Courtney
//  Copyright 2020 Helios Contributors

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

using System.Diagnostics;
using GadrocsWorkshop.Helios.Controls.Capabilities;

namespace GadrocsWorkshop.Helios.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    [DebuggerDisplay("View for {Visual?.Name}")]
    public class HeliosVisualView : FrameworkElement
    {
        private List<HeliosVisualView> _children;
        private DateTime? _touchDownTime;

        public HeliosVisualView()
        {
            _children = new List<HeliosVisualView>();
        }

        #region Properties

        public HeliosVisual Visual
        {
            get { return (HeliosVisual)GetValue(VisualProperty); }
            set {SetValue(VisualProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Profile.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualProperty =
            DependencyProperty.Register("Visual", typeof(HeliosVisual), typeof(HeliosVisualView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnVisualChanged));

        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(HeliosVisualView), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public bool DisplayRotation
        {
            get { return (bool)GetValue(DisplayRotationProperty); }
            set { SetValue(DisplayRotationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DisplayRotation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayRotationProperty =
            DependencyProperty.Register("DisplayRotation", typeof(bool), typeof(HeliosVisualView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnDisplayRotationChanged));

        public bool IgnoreHidden
        {
            get { return (bool)GetValue(IgnoreHiddenProperty); }
            set { SetValue(IgnoreHiddenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IgnoreHidden.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IgnoreHiddenProperty =
            DependencyProperty.Register("IgnoreHidden", typeof(bool), typeof(HeliosVisualView), new PropertyMetadata(false, OnIgnoreHidden));

        private List<HeliosVisualView> Children
        {
            get { return _children; }
        }


        /// <summary>
        /// if true, this visual and all its descendants will use slower "Fant" scaling for bitmaps to increase visual quality
        /// </summary>
        public bool HighQualityBitmapScaling
        {
            get => (bool)GetValue(HighQualityBitmapScalingProperty);
            set => SetValue(HighQualityBitmapScalingProperty, value);
        }
        public static readonly DependencyProperty HighQualityBitmapScalingProperty =
            DependencyProperty.Register("HighQualityBitmapScaling", typeof(bool), typeof(HeliosVisualView), new PropertyMetadata(false, OnHighQualityBitmapScalingChanged));

        #endregion

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

        #endregion

        #region Property Responders

        private static void OnIgnoreHidden(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeliosVisualView view = d as HeliosVisualView;
            if (view != null)
            {
                if (view.IgnoreHidden)
                {
                    view.Visibility = Visibility.Visible;
                }
                else
                {
                    view.SetHidden();
                }
            }
        }

        private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is HeliosVisualView view))
            {
                return;
            }

            view.OnVisualChanged(e.OldValue as HeliosVisual);
        }

        private void OnVisualChanged(HeliosVisual oldVisual)
        {
            if (oldVisual != null)
            {
                oldVisual.Children.CollectionChanged -= VisualChildren_CollectionChanged;
                oldVisual.DisplayUpdate -= Visual_DisplayUpdate;
                oldVisual.Resized -= Visual_ResizeMove;
                oldVisual.Moved -= Visual_ResizeMove;
                oldVisual.HiddenChanged -= Visual_HiddenChanged;

                if (oldVisual is IPreviewInput oldPreview)
                {
                    PreviewMouseDown -= oldPreview.PreviewMouseDown;
                    PreviewMouseUp -= oldPreview.PreviewMouseUp;
                    PreviewTouchDown -= oldPreview.PreviewTouchDown;
                    PreviewTouchUp -= oldPreview.PreviewTouchUp;
                }
            }

            Children.Clear();

            if (Visual == null)
            {
                // no new visual to connect to
                return;
            }

            if (DisplayRotation)
            {
                if (Visual.Renderer.Dispatcher == null)
                {
                    Visual.Renderer.Dispatcher = Dispatcher;
                }

                Visual.Renderer.Refresh();
                LayoutTransform = Visual.Renderer.Transform;
            }
            else
            {
                LayoutTransform = null;
            }

            UpdateChildren();
            Visual.Children.CollectionChanged += VisualChildren_CollectionChanged;
            Visual.DisplayUpdate += Visual_DisplayUpdate;
            Visual.Resized += Visual_ResizeMove;
            Visual.Moved += Visual_ResizeMove;
            Visual.HiddenChanged += Visual_HiddenChanged;

            if (!IgnoreHidden)
            {
                Visibility = Visual.IsHidden ? Visibility.Hidden : Visibility.Visible;
            }

            if (Visual is IPreviewInput newPreview)
            {
                PreviewMouseDown += newPreview.PreviewMouseDown;
                PreviewMouseUp += newPreview.PreviewMouseUp;
                PreviewTouchDown += newPreview.PreviewTouchDown;
                PreviewTouchUp += newPreview.PreviewTouchUp;
            }
        }

        protected static void OnDisplayRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeliosVisualView view = d as HeliosVisualView;
            if (view != null)
            {
                if (view.DisplayRotation)
                {
                    view.Visual.Renderer.Refresh();
                    view.LayoutTransform = view.Visual.Renderer.Transform;
                    view.InvalidateVisual();
                }
                else
                {
                    view.LayoutTransform = null;
                }
            }
        }

        private void Visual_HiddenChanged(object sender, EventArgs e)
        {
            if (CheckAccess())
            {
                SetHidden();
            }
            else
            {
                Dispatcher.BeginInvoke((Action)SetHidden);
            }
        }

        private static void OnHighQualityBitmapScalingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // change the bitmap scaling mode
            RenderOptions.SetBitmapScalingMode(d, ((bool)e.NewValue) ? BitmapScalingMode.HighQuality : BitmapScalingMode.Linear);

            // cascade change to all descendants
            ((HeliosVisualView)d)._children.ForEach(child => child.HighQualityBitmapScaling = (bool)e.NewValue);
        }

        private void SetHidden()
        {
            if (Visual != null && !IgnoreHidden)
            {
                Visibility = Visual.IsHidden ? Visibility.Hidden : Visibility.Visible;
                CascadeRedraw();
            }
        }

        private void CascadeRedraw()
        {
            if (Visual.IsVisible)
            {
                InvalidateVisual();

                foreach (HeliosVisualView child in _children)
                {
                    child.CascadeRedraw();
                }
            }
        }

        private void Visual_ResizeMove(object sender, EventArgs e)
        {
            if (CheckAccess())
            {
                OnResize();
            }
            else
            {
                Dispatcher.BeginInvoke(new Action(OnResize));
            }
        }

        private void OnResize()
        {
            LayoutTransform = DisplayRotation ? Visual.Renderer.Transform : null;

            InvalidateMeasure();
            InvalidateArrange();
            UpdateLayout();
            InvalidateVisual();

            if (!(VisualParent is FrameworkElement parent))
            {
                return;
            }

            parent.InvalidateMeasure();
            parent.InvalidateArrange();
        }

        private void VisualChildren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)UpdateChildren);
        }

        private void Visual_DisplayUpdate(object sender, EventArgs e)
        {
            if (Visual.IsVisible)
            {
                if (CheckAccess())
                {
                    InvalidateVisual();
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(InvalidateVisual), null);
                }
            }
        }

        private void UpdateChildren()
        {
            if (Visual != null)
            {
                int i = 0;
                for (i = Children.Count-1; i >= 0; i--)
                {
                    HeliosVisualView view = Children[i] as HeliosVisualView;
                    if (!Visual.Children.Contains(view.Visual))
                    {
                        RemoveVisualChild(view);
                        Children.RemoveAt(i);
                    }
                }

                for (i = 0; i < Visual.Children.Count; i++)
                {
                    int viewIndex = IndexOfChildView(Visual.Children[i]);
                    HeliosVisualView view;
                    if (viewIndex == -1)
                    {
                        view = new HeliosVisualView
                        {
                            HighQualityBitmapScaling = HighQualityBitmapScaling, 
                            Visual = Visual.Children[i]
                        };
                        Children.Add(view);
                        AddVisualChild(view);
                    }
                    else
                    {
                        view = Children[viewIndex];
                        RemoveVisualChild(view);
                        if (viewIndex != i)
                        {
                            Children.RemoveAt(viewIndex);
                            if (i >= Children.Count)
                            {
                                Children.Add(view);
                            }
                            else
                            {
                                Children.Insert(i, view);
                            }
                        }
                        AddVisualChild(view);
                    }
                }
            }

            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }

        private int IndexOfChildView(HeliosVisual visual)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                HeliosVisualView view = Children[i] as HeliosVisualView;
                if (view != null && view.Visual == visual)
                {
                    return i;
                }
            }
            return -1;
        }

        #endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (Visual != null)
            {
                Visual.Renderer.Render(drawingContext, RenderSize);
            }
        }

        #region Layout Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            Size resultSize = new Size(1, 1);

            if (Visual == null)
            {
                return resultSize;
            }

            resultSize.Width = double.IsPositiveInfinity(availableSize.Width) ? Math.Max(1d, Visual.Width * ZoomFactor) : availableSize.Width;
            resultSize.Height = double.IsPositiveInfinity(availableSize.Height) ? Math.Max(1d, Visual.Height * ZoomFactor) : availableSize.Height;

            double scale = Math.Min(resultSize.Height / Math.Max(1, Visual.Height), resultSize.Width / Math.Max(1, Visual.Width));

            resultSize.Width = Visual.Width * scale;
            resultSize.Height = Visual.Height * scale;

            foreach (HeliosVisualView child in Children)
            {
                child.Measure(new Size(Math.Max(1, child.Visual.DisplayRectangle.Width) * scale, Math.Max(1, child.Visual.DisplayRectangle.Height) * scale));
            }

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Visual == null)
            {
                return finalSize;
            }

            double scaleX = finalSize.Width / Math.Max(1, Visual.Width);
            double scaleY = finalSize.Height / Math.Max(1, Visual.Height);

            foreach (HeliosVisualView child in Children)
            {
                Rect childRect = new Rect(child.Visual.DisplayRectangle.Left * scaleX, child.Visual.DisplayRectangle.Top * scaleY, child.Visual.DisplayRectangle.Width * scaleX, child.Visual.DisplayRectangle.Height * scaleY);
                child.Arrange(childRect);
            }

            return finalSize;
        }

        #endregion

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (Visual != null && Visual.HitTest(hitTestParameters.HitPoint))
            {
                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            }
            else
            {
                return null;
            }
        }

        public HeliosVisualView GetViewerForVisual(HeliosVisual visual)
        {
            foreach (HeliosVisualView view in Children)
            {
                if (view.Visual == visual)
                {
                    return view;
                }
            }
            return null;
        }

        private bool SuppressMouseClick()
        {
            //  Some touchscreens produce a mouse event after a touch event which results in Control Center giving the 
            //  appearance of a double touch.  The SuppressMouseClick - when enabled - is armed on the touch event
            //  and will ignore a subsequent mouse event for a defined period to avoid this problem.
            //
            //  The delay period is implemented currently as a global for all touchscreens within the Control Center
            //  preferences so that it does not need to be stored in each one of the user's profiles.  If this turns
            //  out to be too crude, then code in Monitor.cs & MonitorPorpertyEditor.xaml will need to be uncommented
            //  to allow a per-monitor approach to be adopted.

            if ((Visual == null) ||
                (Visual.Monitor == null) ||
                (Visual.Monitor.SuppressMouseAfterTouchDuration < 1) ||
                (!_touchDownTime.HasValue))
            {
                return false;
            }
            TimeSpan since = DateTime.Now - _touchDownTime.Value;
            if (since.TotalMilliseconds > Visual.Monitor.SuppressMouseAfterTouchDuration)
            {
                return false;
            }
            return true;
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (SuppressMouseClick())
            {
                e.Handled = true;
                return;
            }

            Point location = e.GetPosition(this);
            Visual.MouseDown(location);
            CaptureMouse();
            e.Handled = true;
        }

        protected override void OnTouchDown(TouchEventArgs e)
        {
            if (this.IsEnabled)
            {
                _touchDownTime = DateTime.Now;
                Point location = e.GetTouchPoint(this).Position;
                Visual.MouseDown(location);
                CaptureTouch(e.TouchDevice);
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.MouseDevice.Captured == this)
            {
                Point location = e.GetPosition(this);
                Visual.MouseDrag(location);
                Visual.MouseDragWithArgs(location, e);
                e.Handled = true;
            }
        }

        protected override void OnTouchMove(TouchEventArgs e)
        {
            if (e.TouchDevice.Captured == this)
            {
                Point location = e.GetTouchPoint(this).Position;
                Visual.MouseDrag(location);
                e.Handled = true;
            }
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.MouseDevice.Captured == this)
            {
                if (SuppressMouseClick())
                {
                    e.Handled = true;
                    return;
                }
                Point location = e.GetPosition(this);
                Visual.MouseUp(location);
                Visual.MouseUpWithArgs(location, e);
                e.MouseDevice.Capture(null);
                ReleaseMouseCapture();
                e.Handled = true;
            }
        }

        protected override void OnTouchUp(TouchEventArgs e)
        {
            if (e.TouchDevice.Captured == this)
            {
                Point location = e.GetTouchPoint(this).Position;
                Visual.MouseUp(location);
                ReleaseTouchCapture(e.TouchDevice);
                e.Handled = true;
            }
        }

        /// <summary>
        /// previewing mouse wheel events to avoid consuming mouse wheel event
        /// if ancestor or descendant control should be scrolling or otherwise processing it
        /// instead
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            if (IsEnabled && (Visual?.CanConsumeMouseWheel ?? false))
            {
                // we want to process this mouse wheel interaction, but we have no default
                // handler for preview mouse wheel, so we just short circuit it
                ProcessMouseWheel(e);
            }

            // based on https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/
            if (e.Handled || ScrollViewerHelper.MouseWheelEventsOnStack.Contains(e))
            {
                // infinite loop prevented
                return;
            }

            // re-originate as bubbling preview mouse wheel event at the original source
            MouseWheelEventArgs previewEventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = UIElement.PreviewMouseWheelEvent,
                Source = this
            };
            UIElement originalSource = e.OriginalSource as UIElement;
            ScrollViewerHelper.MouseWheelEventsOnStack.Add(previewEventArg);
            originalSource?.RaiseEvent(previewEventArg);
            ScrollViewerHelper.MouseWheelEventsOnStack.Remove(previewEventArg);

            // at this point if no one else handled the event in our children, we do our job

            if (previewEventArg.Handled)
            {
                // already handled
                e.Handled = true;
                return;
            }

            // punt mouse wheel event upwards because we can't use it
            e.Handled = true;
            MouseWheelEventArgs eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
            {
                RoutedEvent = MouseWheelEvent,
                Source = this
            };

            // HeliosVisualView is not in the logical tree, so we navigate up the visual tree
            (VisualParent as UIElement)?.RaiseEvent(eventArg);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            // we won't normally receive this event if these conditions are not true, but check anyway in
            // case of synthetic event
            if (IsEnabled && Visual != null && Visual.CanConsumeMouseWheel)
            {
                ProcessMouseWheel(e);
            }
        }

        private void ProcessMouseWheel(MouseWheelEventArgs e)
        {
            int delta = e.Delta;
            Visual.MouseWheel(delta);
            Visual.MouseWheelWithArgs(delta, e);
            e.Handled = true;
        }
    }
}
