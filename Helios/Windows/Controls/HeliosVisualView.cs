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

using GadrocsWorkshop.Helios.Controls.Capabilities;
using GadrocsWorkshop.Helios.Controls.RendererCapabilities;
using NLog;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace GadrocsWorkshop.Helios.Windows.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    [DebuggerDisplay("View for {Visual?.Name} @{GetHashCode()}")]
    public class HeliosVisualView : FrameworkElement
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private DateTime? _touchDownTime;

        /// <summary>
        /// if not null, this is a windows control that the renderer for this visual has presented to us for inclusion in the tree
        /// </summary>
        private FrameworkElement _rendererElement;

        /// <summary>
        /// if true, then the HeliosVisual handles windows mouse events <see cref="IWindowsMouseInput">directly </see>, and we will not translate them or automatically capture the mouse
        /// </summary>
        private bool _controlHandlesMouse;

        public HeliosVisualView()
        {
            Children = new List<HeliosVisualView>();
        }

        #region Properties

        public HeliosVisual Visual
        {
            get { return (HeliosVisual)GetValue(VisualProperty); }
            set {SetValue(VisualProperty, value); }
        }

        public static readonly DependencyProperty VisualProperty =
            DependencyProperty.Register("Visual", typeof(HeliosVisual), typeof(HeliosVisualView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsRender, OnVisualChanged));

        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(HeliosVisualView), new FrameworkPropertyMetadata(1d, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure));

        public bool DisplayRotation
        {
            get { return (bool)GetValue(DisplayRotationProperty); }
            set { SetValue(DisplayRotationProperty, value); }
        }

        public static readonly DependencyProperty DisplayRotationProperty =
            DependencyProperty.Register("DisplayRotation", typeof(bool), typeof(HeliosVisualView), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentArrange | FrameworkPropertyMetadataOptions.AffectsParentMeasure, OnDisplayRotationChanged));

        public bool IgnoreHidden
        {
            get { return (bool)GetValue(IgnoreHiddenProperty); }
            set { SetValue(IgnoreHiddenProperty, value); }
        }

        public static readonly DependencyProperty IgnoreHiddenProperty =
            DependencyProperty.Register("IgnoreHidden", typeof(bool), typeof(HeliosVisualView), new PropertyMetadata(false, OnIgnoreHidden));

        private List<HeliosVisualView> Children { get; }

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

        /// <summary>
        /// WARNING: this has to already reflect the new state when AddVisualChild is called, but not if RemoveVisualChild is called, because
        /// of how .NET works
        /// </summary>
        protected override int VisualChildrenCount => Children.Count + ((_rendererElement != null) ? 1 : 0);

        /// <summary>
        /// WARNING: this has to already reflect the new state when AddVisualChild is called, but not if RemoveVisualChild is called, because
        /// of how .NET works
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            if (_rendererElement != null)
            {
                if (index == 0)
                {
                    return _rendererElement;
                }

                // otherwise interpret as index into Children
                index--;
            }
            
            if (index < 0 || index >= Children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return Children[index];
        }

        #endregion

        #region Property Responders

        private static void OnIgnoreHidden(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is HeliosVisualView view))
            {
                return;
            }

            if (view.IgnoreHidden)
            {
                view.Visibility = Visibility.Visible;
            }
            else
            {
                view.SetHidden();
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
                // clean up event handling
                oldVisual.Children.CollectionChanged -= VisualChildren_CollectionChanged;
                oldVisual.DisplayUpdate -= Visual_DisplayUpdate;
                oldVisual.Resized -= Visual_ResizeMove;
                oldVisual.Moved -= Visual_ResizeMove;
                oldVisual.HiddenChanged -= Visual_HiddenChanged;
            }

            // ReSharper disable once ConvertIfStatementToSwitchStatement

            if (oldVisual is IWindowsPreviewInput oldPreview)
                {
                    PreviewMouseDown -= oldPreview.PreviewMouseDown;
                    PreviewMouseUp -= oldPreview.PreviewMouseUp;
                    PreviewTouchDown -= oldPreview.PreviewTouchDown;
                    PreviewTouchUp -= oldPreview.PreviewTouchUp;
                }

                if (oldVisual is IWindowsMouseInput oldMouseInput)
                {
                    // native Windows API mouse events, in addition to Helios simple mouse/touch events
                    MouseDown -= oldMouseInput.MouseDown;
                    MouseUp -= oldMouseInput.MouseUp;
                    MouseMove -= oldMouseInput.MouseMove;
                    GotMouseCapture -= oldMouseInput.GotMouseCapture;
                    LostMouseCapture -= oldMouseInput.LostMouseCapture;
                    MouseEnter -= oldMouseInput.MouseEnter;
                    MouseLeave -= oldMouseInput.MouseLeave;
                    _controlHandlesMouse = false;
                }

            
            if (oldVisual is IWindowsManipulationAware oldManipulationHandlers)
            {
                ManipulationStarting -= oldManipulationHandlers.ManipulationStarting;
                ManipulationStarted -= oldManipulationHandlers.ManipulationStarted;
                ManipulationBoundaryFeedback -= oldManipulationHandlers.ManipulationBoundaryFeedback;
                ManipulationDelta -= oldManipulationHandlers.ManipulationDelta;
                ManipulationInertiaStarting -= oldManipulationHandlers.ManipulationInertiaStarting;
                ManipulationCompleted -= oldManipulationHandlers.ManipulationCompleted;
                IsManipulationEnabled = false;
            }

            // tear down the old tree
            if (_rendererElement != null)
            {
                RemoveVisualChild(_rendererElement);
                _rendererElement = null;
            }
            Children.Clear();

            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (Visual == null)
            {
                // view is now not drawing anything
                return;
            }

            if (Visual is IWindowsManipulationAware manipulationAware)
            {
                IsManipulationEnabled = true;
                ManipulationStarting += manipulationAware.ManipulationStarting;
                ManipulationStarted += manipulationAware.ManipulationStarted;
                ManipulationBoundaryFeedback += manipulationAware.ManipulationBoundaryFeedback;
                ManipulationDelta += manipulationAware.ManipulationDelta;
                ManipulationInertiaStarting += manipulationAware.ManipulationInertiaStarting;
                ManipulationCompleted += manipulationAware.ManipulationCompleted;
            }


            if (Visual.Renderer is IWindowsControl windowsControl)
            {
                if (_rendererElement == null)
                {
                    // no currently installed element
                    InstallWindowsControl(windowsControl);
                }
            }

            if (DisplayRotation && Visual.Renderer != null)
            {
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

            if (Visual is IWindowsPreviewInput newPreview)
            {
                PreviewMouseDown += newPreview.PreviewMouseDown;
                PreviewMouseUp += newPreview.PreviewMouseUp;
                PreviewTouchDown += newPreview.PreviewTouchDown;
                PreviewTouchUp += newPreview.PreviewTouchUp;
            }

            if (Visual is IWindowsMouseInput newMouseInput)
            {
                // native Windows API mouse events, in addition to Helios simple mouse/touch events
                MouseDown += newMouseInput.MouseDown;
                MouseUp += newMouseInput.MouseUp;
                MouseMove += newMouseInput.MouseMove;
                GotMouseCapture += newMouseInput.GotMouseCapture;
                LostMouseCapture += newMouseInput.LostMouseCapture;
                MouseEnter += newMouseInput.MouseEnter;
                MouseLeave += newMouseInput.MouseLeave;
                _controlHandlesMouse = true;
            }
        }

        protected static void OnDisplayRotationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is HeliosVisualView view))
            {
                return;
            }

            if (view.DisplayRotation)
            {
                Logger.Debug("Invalidating Helios control due to rotation change");
                view.Visual.Renderer.Refresh();
                view.LayoutTransform = view.Visual.Renderer.Transform;
                view.InvalidateVisual();
            }
            else
            {
                view.LayoutTransform = null;
            }
        }

        private void Visual_HiddenChanged(object sender, EventArgs e)
        {
            SetHidden();
        }

        private static void OnHighQualityBitmapScalingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // change the bitmap scaling mode
            RenderOptions.SetBitmapScalingMode(d, ((bool)e.NewValue) ? BitmapScalingMode.HighQuality : BitmapScalingMode.Linear);

            // cascade change to all descendants
            ((HeliosVisualView)d).Children.ForEach(child => child.HighQualityBitmapScaling = (bool)e.NewValue);
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
            if (!Visual.IsVisible)
            {
                return;
            }

            // this window is out of date
            Logger.Debug("Invalidating Helios control due to forced redraw cascade");
            InvalidateVisual();

            // directly rendered windows control is out of date, if any
            _rendererElement?.InvalidateVisual();

            // let each Helios child invalidate their visuals also
            foreach (HeliosVisualView child in Children)
            {
                child.CascadeRedraw();
            }
        }

        private void Visual_ResizeMove(object sender, EventArgs e)
        {
            OnResize();
        }

        private void OnResize()
        {
            LayoutTransform = DisplayRotation ? Visual.Renderer.Transform : null;

            Logger.Debug("Invalidating Helios control due to resize");
            InvalidateMeasure();
            InvalidateArrange();
            UpdateLayout();
            InvalidateVisual();

            if (!(VisualParent is FrameworkElement parent))
            {
                return;
            }

            Logger.Debug("Invalidating Helios control's parents due to resize");
            parent.InvalidateMeasure();
            parent.InvalidateArrange();
        }

        private void VisualChildren_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // can't modify anything during a visual tree event, so we must call this indirect
            Dispatcher.BeginInvoke((Action)UpdateChildren);
        }

        private void Visual_DisplayUpdate(object sender, EventArgs e)
        {
            if (Visual.IsVisible || Visual.DesignMode)
            {
                // Logger.Debug("Invalidating Helios control due to requested display update");
                InvalidateVisual();
            }
        }

        private void UpdateChildren()
        {
            Logger.Debug("Updating entire Helios control to check for child changes");
            if (Visual == null)
            {
                // we don't event need to invalidate (older code was still calling those)
                return;
            }

            int i;
            for (i = Children.Count-1; i >= 0; i--)
            {
                HeliosVisualView view = Children[i];
                if (!Visual.Children.Contains(view.Visual))
                {
                    Logger.Debug("Helios control removing stale visual child {HeliosVisualView}", view);
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
                    Logger.Debug("Helios control adding new visual child {HeliosVisualView}", view);
                    Children.Add(view);
                    AddVisualChild(view);
                }
                else
                {
                    view = Children[viewIndex];
                    Logger.Debug("Helios control temporarily removing visual child {HeliosVisualView}", view);
                    RemoveVisualChild(view);
                    if (viewIndex != i)
                    {
                        Logger.Debug("Helios control sorting visual child {HeliosVisualView} to its correct position {Index}", view, i);
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
                    Logger.Debug("Helios control restoring visual child {HeliosVisualView}", view);
                    AddVisualChild(view);
                }
            }

            Logger.Debug("Invalidating Helios control due to child updates");
            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }

        private void InstallWindowsControl(IWindowsControl windowsControl)
        {
            FrameworkElement rendererElement = windowsControl.CreateNewFrameworkElement();
            if (rendererElement == null)
            {
                // we don't always create a live element, for example in design mode
                return;
            }

            // have to do this indirect to avoid editing visual tree in this stack
            Dispatcher.BeginInvoke(new Action<FrameworkElement>(InstallRendererElement), rendererElement);
        }

        private void InstallRendererElement(FrameworkElement rendererElement)
        {
            Logger.Debug("Installed Windows child control into Helios control");
            _rendererElement = rendererElement;
            AddVisualChild(_rendererElement);
            _rendererElement.InvalidateVisual();

            Logger.Debug("Invalidating Helios control due to newly installed Windows child control");
            InvalidateMeasure();
            InvalidateArrange();
            InvalidateVisual();
        }

        private int IndexOfChildView(HeliosVisual visual)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i] is HeliosVisualView view && view.Visual == visual)
                {
                    return i;
                }
            }
            return -1;
        }

#endregion

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            Visual?.Renderer?.Render(drawingContext, RenderSize);
        }

#region Layout Methods

        protected override Size MeasureOverride(Size availableSize)
        {
            Logger.Debug("Helios control measuring for {Size}", availableSize);
            Size resultSize = new Size(1, 1);

            if (Visual == null)
            {
                Logger.Debug("Helios control has no visual, so we will ask for one pixel");
                return resultSize;
            }

            resultSize.Width = double.IsPositiveInfinity(availableSize.Width) ? Math.Max(1d, Visual.Width * ZoomFactor) : availableSize.Width;
            resultSize.Height = double.IsPositiveInfinity(availableSize.Height) ? Math.Max(1d, Visual.Height * ZoomFactor) : availableSize.Height;

            double scale = Math.Min(resultSize.Height / Math.Max(1, Visual.Height), resultSize.Width / Math.Max(1, Visual.Width));

            resultSize.Width = Visual.Width * scale;
            resultSize.Height = Visual.Height * scale;

            _rendererElement?.Measure(availableSize);
            foreach (HeliosVisualView child in Children)
            {
                child.Measure(new Size(Math.Max(1, child.Visual.DisplayRectangle.Width) * scale, Math.Max(1, child.Visual.DisplayRectangle.Height) * scale));
            }

            return resultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Logger.Debug("Helios control arranging for {Size}", finalSize);
            if (Visual == null)
            {
                Logger.Debug("Helios control has no visual, so we will acccept {Size}", finalSize);
                return finalSize;
            }

            double scaleX = finalSize.Width / Math.Max(1, Visual.Width);
            double scaleY = finalSize.Height / Math.Max(1, Visual.Height);

            _rendererElement?.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
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
            return null;
        }

        public HeliosVisualView GetViewerForVisual(HeliosVisual visual) => Children.FirstOrDefault(view => view.Visual == visual);

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
            return !(since.TotalMilliseconds > Visual.Monitor.SuppressMouseAfterTouchDuration);
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_controlHandlesMouse)
            {
                // use normal windows handling via events
                return;
            }

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
            if (!IsEnabled)
            {
                return;
            }

            _touchDownTime = DateTime.Now;
            Point location = e.GetTouchPoint(this).Position;
            Visual.MouseDown(location);
            CaptureTouch(e.TouchDevice);
            e.Handled = true;
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (_controlHandlesMouse)
            {
                // use normal windows handling via events
                return;
            }

            if (e.MouseDevice.Captured == this)
            {
                Point location = e.GetPosition(this);
                Visual.MouseDrag(location);
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
            if (_controlHandlesMouse)
            {
                // use normal windows handling via events
                return;
            }

            if (e.MouseDevice.Captured == this)
            {
                if (SuppressMouseClick())
                {
                    e.Handled = true;
                    return;
                }
                Point location = e.GetPosition(this);
                Visual.MouseUp(location);
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
            e.Handled = true;
        }
    }
}
