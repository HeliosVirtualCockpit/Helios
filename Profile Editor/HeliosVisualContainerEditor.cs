//  Copyright 2014 Craig Courtney
//  Copyright 2020 Ammo Goettsch
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

using System.Diagnostics;
using System.Windows.Controls;

namespace GadrocsWorkshop.Helios.ProfileEditor
{
    using GadrocsWorkshop.Helios.Controls.Special;
    using GadrocsWorkshop.Helios.ProfileEditor.UndoEvents;
    using GadrocsWorkshop.Helios.Windows.Controls;
    using NLog.Targets;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Xml;

    [ContentProperty("View")]
    public class HeliosVisualContainerEditor : FrameworkElement
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private bool _keyMoveBatchOpen = false;

        private readonly DrawingBrush _checkerBrush;
        private readonly Pen _borderPen = new Pen(Brushes.Black, 6d);

        // Mouse state vairables for mouse interactions
        private EditorMouseState _mouseState;     // Current mode for mouse interactions
        private Point _mouseDownPosition;   // Location (relative to this control) where mouse button was pressed
        private Vector _dragVector = new Vector(0d, 0d);  // Offset of current mouse position based off of mouse down position
        private Point _panningOffset; // the scroller offsets when we started panning

        private readonly SnapManager _snapManager = new SnapManager();

        private SelectionAdorner _selectionAdorner;
        private DragSelectionAdorner _dragSelectionAdorner;

        private readonly CalibrationPointCollectionDouble _zoomCalibration;

        private readonly HeliosVisualView _view;

        static HeliosVisualContainerEditor()
        {
            Type ownerType = typeof(HeliosVisualContainerEditor);

            DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Delete, Delete_Executed, Delete_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Stop, Stop_Executed, Stop_CanExecute));

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.MoveForward, MoveForward_Executed, Move_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.MoveBack, MoveBack_Executed, Move_CanExecute));

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.AlignTop, AlignTop_Executed, Align_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.AlignBottom, AlignBottom_Executed, Align_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.AlignLeft, AlignLeft_Executed, Align_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.AlignRight, AlignRight_Executed, Align_CanExecute));

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.AlignHorizontalCenter, AlignHorizontalCenter_Executed, Align_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.AlignVerticalCenter, AlignVerticalCenter_Executed, Align_CanExecute));

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.DistributeHorizontalCenter, DistributeHorizontalCenter_Executed, Distribute_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.DistributeVerticalCenter, DistributeVerticalCenter_Executed, Distribute_CanExecute));

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.SpaceHorizontal, SpaceHorizontal_Executed, Distribute_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.SpaceVertical, SpaceVertical_Executed, Distribute_CanExecute));

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ProfileEditorCommands.SaveTemplate, SaveTemplate_Executed, SaveTemplate_CanExecute));

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Copy, Copy_Executed, Copy_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Cut, Cut_Executed, Cut_CanExecute));
            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Paste, Paste_Executed, Paste_CanExecute));

            CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.SelectAll, SelectAll_Executed));
        }

        public HeliosVisualContainerEditor()
        {
            Focusable = true;

            _view = new HeliosVisualView();
            _view.IgnoreHidden = true;
            _view.DisplayRotation = false;
            AddVisualChild(_view);

            SelectedItems = new HeliosVisualCollection();
            _zoomCalibration = new CalibrationPointCollectionDouble(-4d, 0.25d, 4d, 4d)
            {
                new CalibrationPointDouble(0d, 1d)
            };
            SelectedItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);

            DrawingGroup checkerGroup = new DrawingGroup();
            checkerGroup.Children.Add(new GeometryDrawing(Brushes.White, null, new RectangleGeometry(new Rect(0, 0, 100, 100))));

            DrawingGroup grayGroup = new DrawingGroup();
            grayGroup.Children.Add(new GeometryDrawing(Brushes.LightGray, null, new RectangleGeometry(new Rect(0, 0, 50, 50))));
            grayGroup.Children.Add(new GeometryDrawing(Brushes.LightGray, null, new RectangleGeometry(new Rect(50, 50, 50, 50))));

            checkerGroup.Children.Add(grayGroup);
            checkerGroup.Freeze();

            _checkerBrush = new DrawingBrush(checkerGroup);
            _checkerBrush.Viewport = new Rect(0, 0, 10, 10);
            _checkerBrush.ViewportUnits = BrushMappingMode.Absolute;
            _checkerBrush.TileMode = TileMode.Tile;
            _checkerBrush.Freeze();
        }

        #region Visual Methods

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _view;
        }

        #endregion

        #region Properties

        public HeliosVisualContainer VisualContainer
        {
            get { return (HeliosVisualContainer)GetValue(VisualContainerProperty); }
            set { SetValue(VisualContainerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisualContainer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisualContainerProperty =
            DependencyProperty.Register("VisualContainer", typeof(HeliosVisualContainer), typeof(HeliosVisualContainerEditor), new PropertyMetadata(null));

        public HeliosVisualCollection SelectedItems
        {
            get { return (HeliosVisualCollection)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(HeliosVisualCollection), typeof(HeliosVisualContainerEditor), new PropertyMetadata(null));

        public double ZoomLevel
        {
            get { return (double)GetValue(ZoomLevelProperty); }
            set { SetValue(ZoomLevelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomLevel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomLevelProperty =
            DependencyProperty.Register("ZoomLevel", typeof(double), typeof(HeliosVisualContainerEditor), new UIPropertyMetadata(0d));

        public double ZoomFactor
        {
            get { return (double)GetValue(ZoomFactorProperty); }
            set { SetValue(ZoomFactorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomFactor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomFactorProperty =
            DependencyProperty.Register("ZoomFactor", typeof(double), typeof(HeliosVisualContainerEditor), new FrameworkPropertyMetadata(1d));

        public SnapManager SnapManager
        {
            get { return _snapManager; }
        }

        public bool PreviewMode
        {
            get { return (bool)GetValue(PreviewModeProperty); }
            set { SetValue(PreviewModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PreviewMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PreviewModeProperty =
            DependencyProperty.Register("PreviewMode", typeof(bool), typeof(HeliosVisualContainerEditor), new PropertyMetadata(false));

        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == SelectedItemsProperty)
            {
                HeliosVisualCollection oldItems = e.OldValue as HeliosVisualCollection;
                if (oldItems != null)
                {
                    oldItems.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
                }

                HeliosVisualCollection newItems = e.NewValue as HeliosVisualCollection;
                if (newItems != null)
                {
                    newItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedItems_CollectionChanged);
                    if (newItems.Count > 0)
                    {
                        GetSelectionAdorner();
                    }
                }
                else
                {
                    RemoveSelectionAdorner();
                }
            }
            else if (e.Property == VisualContainerProperty)
            {
                HeliosVisualContainer oldContainer = e.OldValue as HeliosVisualContainer;
                if (oldContainer != null)
                {
                    oldContainer.DesignMode = false;
                    oldContainer.Children.CollectionChanged -= Children_CollectionChanged;
                }

                _view.Visual = VisualContainer;
                VisualContainer.DesignMode = true;
                VisualContainer.Children.CollectionChanged += Children_CollectionChanged;
            }
            else if (e.Property == PreviewModeProperty)
            {
                if (PreviewMode)
                {
                    RemoveSelectionAdorner();
                }
                else if (SelectedItems.Count > 0)
                {
                    GetSelectionAdorner();
                }


            }
            else if (e.Property == ZoomLevelProperty)
            {
                ZoomFactor = _zoomCalibration.Interpolate(ZoomLevel);
                _view.ZoomFactor = ZoomFactor;

                SelectionAdorner adorner = GetSelectionAdorner(false);
                if (adorner != null)
                {
                    InvalidateVisual();
                }
            }
            base.OnPropertyChanged(e);
        }

        void Children_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (HeliosVisual item in e.OldItems)
                {
                    if (SelectedItems.Contains(item) && !(e.NewItems != null && e.NewItems.Contains(item)))
                    {
                        SelectedItems.Remove(item);
                    }
                }
            }
        }

        void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SelectionAdorner adorner = GetSelectionAdorner();
            if (adorner != null)
            {
                adorner.InvalidateVisual();
            }            
        }

        private SelectionAdorner GetSelectionAdorner()
        {
            return GetSelectionAdorner(true);
        }

        private SelectionAdorner GetSelectionAdorner(bool create)
        {
            if (_selectionAdorner == null && create)
            {
                _selectionAdorner = new SelectionAdorner(this);
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    adornerLayer.Add(_selectionAdorner);
                }
                else
                {
                    _selectionAdorner = null;
                }
            }
            return _selectionAdorner;
        }

        private void RemoveSelectionAdorner()
        {
            if (_selectionAdorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                adornerLayer.Remove(_selectionAdorner);
                _selectionAdorner = null;
            }
        }

        private DragSelectionAdorner GetDragSelectionAdorner()
        {
            if (_dragSelectionAdorner == null)
            {
                _dragSelectionAdorner = new DragSelectionAdorner(this);
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                adornerLayer.Add(_dragSelectionAdorner);
            }
            return _dragSelectionAdorner;
        }

        private void RemoveDragSelectionAdorner()
        {
            if (_dragSelectionAdorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
                adornerLayer.Remove(_dragSelectionAdorner);
                _dragSelectionAdorner = null;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _view.Measure(availableSize);
            return _view.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _view.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (SelectedItems.Count > 0)
            {
                GetSelectionAdorner();
            }
            drawingContext.DrawRoundedRectangle(_checkerBrush, _borderPen, new Rect(-2, -2, RenderSize.Width + 4, RenderSize.Height + 4), 4d, 4d);
        }

        public HeliosVisualView GetViewerForVisual(HeliosVisual visual)
        {
            return _view.GetViewerForVisual(visual);
        }

        #region Mouse Control

        /// <summary>
        /// Called when the snap manager needs to be configured with appropriate visuals.
        /// </summary>
        /// <param name="excludeSelected">True if we should exclude currently selected items.</param>
        public void LoadSnapTargets(bool excludeSelected)
        {
            SnapManager.Bounds = new Rect(0, 0, VisualContainer.Width, VisualContainer.Height);
            SnapManager.Targets.Clear();
            foreach (HeliosVisual panel in VisualContainer.Children)
            {
                if ((!excludeSelected || !SelectedItems.Contains(panel)) && panel.IsSnapTarget)
                {
                    SnapManager.Targets.Add(new SnapTarget(panel));
                }
            }
        }

        /// <summary>
        /// intercept all left mouse clicks because we contain a lot of controls that would otherwise
        /// process these events, but we need to consider them "passive" for purposes of editing in the Profile Editor
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (PreviewMode)
            {
                return;
            }

            Focus();

            _mouseDownPosition = e.GetPosition(Window.GetWindow(this));
            _snapManager.DragVector = new Vector(0, 0);
            _snapManager.Action = SnapAction.None;

            Point resizePosition = e.GetPosition(this);

            Rect selectionRectangle = SelectedItems.Rectangle;
            selectionRectangle.Scale(ZoomFactor, ZoomFactor);

            if (e.ClickCount == 2)
            {
                foreach (HeliosVisual visual in VisualContainer.Children.Reverse())
                {
                    Rect visualRect = visual.DisplayRectangle;
                    visualRect.Scale(ZoomFactor, ZoomFactor);

                    if (!visual.IsLocked  && !visual.IsHidden && visualRect.Contains(resizePosition))
                    {
                        Helios.Controls.HeliosPanel panel = visual as Helios.Controls.HeliosPanel;
                        if (panel != null)
                        {
                            if (ProfileEditorCommands.OpenProfileItem.CanExecute(panel, this))
                            {
                                ProfileEditorCommands.OpenProfileItem.Execute(panel, this);
                                e.Handled = true;
                                break;
                            }
                        }
                    }
                }
                return;
            }

            if (_mouseState == EditorMouseState.Idle && SelectedItems.Count > 0 && selectionRectangle.Contains(resizePosition) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                LoadSnapTargets(true);
                _snapManager.Location = SelectedItems.Rectangle.Location;
                _snapManager.Size = SelectedItems.Rectangle.Size;
                _snapManager.Action = SnapAction.Move;
                _mouseState = EditorMouseState.Move;
                Cursor = Cursors.SizeAll;
            }

            if (_mouseState == EditorMouseState.Idle)
            {
                bool controlClicked = false;

                foreach (HeliosVisual visual in VisualContainer.Children.Reverse())
                {
                    Rect visualRect = visual.DisplayRectangle;
                    visualRect.Scale(ZoomFactor, ZoomFactor);

                    if (!visual.IsLocked && !visual.IsHidden && visualRect.Contains(resizePosition))
                    {
                        controlClicked = true;
                        if (!SelectedItems.Contains(visual))
                        {
                            if (Keyboard.Modifiers != ModifierKeys.Control)
                            {
                                SelectedItems.Clear();
                            }

                            SelectedItems.Add(visual);
                            break;
                        }
                        else if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            SelectedItems.Remove(visual);
                        }
                    }
                }

                if (!controlClicked)
                {
                    _mouseState = EditorMouseState.SelectStart;
                    DragSelectionAdorner adorner = GetDragSelectionAdorner();
                    adorner.StartLocation = e.GetPosition(this);
                    if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        SelectedItems.Clear();
                    }
                }
            }

            e.Handled = true;
            CaptureMouse();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (PreviewMode || e.ChangedButton != MouseButton.Middle || _mouseState != EditorMouseState.Idle)
            {
                base.OnPreviewMouseDown(e);
                return;
            }

            _mouseState = EditorMouseState.Panning;

            // we don't need this location, but keep it for debugging
            _mouseDownPosition = e.GetPosition(Window.GetWindow(this));

            // note the initial offsets of the scroller we will pan
            ScrollViewer scroller = FindPanningScroller();
            if (scroller == null)
            {
                _panningOffset.X = 0;
                _panningOffset.Y = 0;
            }
            else
            {
                _panningOffset.X = scroller.HorizontalOffset;
                _panningOffset.Y = scroller.VerticalOffset;
            }

            // REVISIT set an adorner and clear it when we are done panning

            e.Handled = true;
            CaptureMouse();
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (PreviewMode)
            {
                return;
            }

            Point currentPosition = e.GetPosition(Window.GetWindow(this));
            _dragVector = currentPosition - _mouseDownPosition;

            switch (_mouseState)
            {
                case EditorMouseState.SelectStart:
                {
                    if (e.LeftButton == MouseButtonState.Pressed &&
                        (Math.Abs(_dragVector.X) > SystemParameters.MinimumHorizontalDragDistance ||
                         Math.Abs(_dragVector.Y) > SystemParameters.MinimumVerticalDragDistance) &&
                         !Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        SelectedItems.Clear();
                        _mouseState = EditorMouseState.Selecting;
                    }
                    break;
                }
                case EditorMouseState.Move:
                    _snapManager.ForceProportions = Keyboard.Modifiers.HasFlag(ModifierKeys.Shift);
                    _snapManager.IgnoreTargets = Keyboard.Modifiers.HasFlag(ModifierKeys.Control);
                    _snapManager.DragVector = _dragVector;
                    GetSelectionAdorner().InvalidateVisual();                    
                    break;

                case EditorMouseState.Selecting:
                    GetDragSelectionAdorner().DragVector = _dragVector;
                    break;

                case EditorMouseState.Panning:
                    HandleMiddleMouseButtonPanning(e, currentPosition);
                    break;
            }

            e.Handled = true;
        }

        private void HandleMiddleMouseButtonPanning(MouseEventArgs e, Point currentPosition)
        {
            _ = e;
            ScrollViewer scroller = FindPanningScroller();
            if (scroller == null)
            {
                return;
            }
            scroller.ScrollToHorizontalOffset(_panningOffset.X + _mouseDownPosition.X - currentPosition.X);
            scroller.ScrollToVerticalOffset(_panningOffset.Y + _mouseDownPosition.Y - currentPosition.Y);
        }

        private ScrollViewer FindPanningScroller()
        {
            // find a scrollviewer that can move horizontally and vertically, at least if shrunk enough
            ScrollViewer scroller = VisualAncestors
                .OfType<ScrollViewer>()
                .Where(v => (v.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto) ||
                            (v.HorizontalScrollBarVisibility == ScrollBarVisibility.Visible))
                .FirstOrDefault(v => (v.VerticalScrollBarVisibility == ScrollBarVisibility.Auto) ||
                                     (v.VerticalScrollBarVisibility == ScrollBarVisibility.Visible));
            return scroller;
        }

        private IEnumerable<UIElement> VisualAncestors
        {
            get
            {
                UIElement parent = Parent as UIElement;
                while (parent != null)
                {
                    yield return parent;
                    parent = (parent as FrameworkElement)?.Parent as UIElement;
                }
            }
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (PreviewMode || (e.ChangedButton != MouseButton.Left && e.ChangedButton != MouseButton.Middle))
            {
                base.OnPreviewMouseUp(e);
                return;
            }

            switch (_mouseState)
            {
                case EditorMouseState.SelectStart:
                    if (SelectedItems.Count > 0)
                    {
                        SelectedItems.Clear();
                    }
                    break;

                case EditorMouseState.Move:
                    if (_snapManager.Action != SnapAction.None)
                    {
                        if (_snapManager.IsValidDrag)
                        {
                            ConfigManager.UndoManager.StartBatch();
                            foreach (HeliosVisual panel in SelectedItems)
                            {
                                panel.Left += _snapManager.LocationOffset.X;
                                panel.Top += _snapManager.LocationOffset.Y;
                            }
                            ConfigManager.UndoManager.CloseBatch();
                        }
                    }
                    _snapManager.Action = SnapAction.None;
                    GetSelectionAdorner().InvalidateVisual();
                    Cursor = null;
                    break;

                case EditorMouseState.Selecting:
                    DragSelectionAdorner dragAdorner = GetDragSelectionAdorner();
                    foreach (HeliosVisual visual in VisualContainer.Children.Reverse())
                    {
                        Rect visualRect = visual.DisplayRectangle;
                        visualRect.Scale(ZoomFactor, ZoomFactor);
                        if (!visual.IsLocked && dragAdorner.Rectangle.IntersectsWith(visualRect) && !SelectedItems.Contains(
                            visual))
                        {
                            SelectedItems.Add(visual);
                        }
                    }
                    break;
            }

            _mouseState = EditorMouseState.Idle;
            RemoveDragSelectionAdorner();
            ReleaseMouseCapture();
        }

        // XXX investigate why this is unused
        private void ScaleVisuals(HeliosVisualCollection visuals, Rect selectionRectangle, Vector offset, double scaleX, double scaleY)
        {
            foreach (HeliosVisual visual in visuals)
            {
                if (visual.Left > selectionRectangle.Left)
                {
                    double locXDif = visual.Left - selectionRectangle.Left;
                    visual.Left = (locXDif * scaleX) - locXDif;
                }
                else
                {
                    visual.Left += offset.X;
                }

                if (visual.Top > selectionRectangle.Top)
                {
                    double locYDif = visual.Top - selectionRectangle.Top;
                    visual.Top += (locYDif * scaleY) - locYDif;
                }
                else
                {
                    visual.Top += offset.Y;
                }

                visual.Width = Math.Max(visual.Width * scaleX, 1d);
                visual.Height = Math.Max(visual.Height * scaleY, 1d);

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    visual.ScaleChildren(scaleX, scaleY);
                }
            }
        }

#endregion

        protected override void OnKeyDown(KeyEventArgs e)
        {
            int xMove = 0;
            int yMove = 0;

            switch (e.Key)
            {
                case Key.Left:
                    xMove = -1;
                    break;
                case Key.Right:
                    xMove = 1;
                    break;
                case Key.Up:
                    yMove = -1;
                    break;
                case Key.Down:
                    yMove = 1;
                    break;
                default:
                    base.OnKeyDown(e);
                    return;
            }

            if (!_keyMoveBatchOpen)
            {
                ConfigManager.UndoManager.StartBatch();
                _keyMoveBatchOpen = true;
            }

            foreach (HeliosVisual control in SelectedItems)
            {
                control.Left = ClampValue(0d, VisualContainer.Width - control.DisplayRectangle.Width, control.Left + xMove);
                control.Top = ClampValue(0d, VisualContainer.Height - control.DisplayRectangle.Height, control.Top + yMove);
            }

            e.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (_keyMoveBatchOpen)
            {
                ConfigManager.UndoManager.CloseBatch();
                _keyMoveBatchOpen = false;
                e.Handled = true;
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        private double ClampValue(double min, double max, double value)
        {
            double clampedValue = Math.Max(min, value);
            clampedValue = Math.Min(max, clampedValue);
            return clampedValue;
        }

#region Command Handlers

        private static void Delete_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                e.Handled = true;
                e.CanExecute = editor.SelectedItems != null && editor.SelectedItems.Count > 0;
            }
        }

        private static void Move_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            e.CanExecute = editor != null && editor.SelectedItems != null && editor.SelectedItems.Count > 0;
        }

        private static void Align_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            e.CanExecute = editor != null && editor.SelectedItems != null && editor.SelectedItems.Count > 1;
        }

        private static void Distribute_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            e.CanExecute = editor != null && editor.SelectedItems != null && editor.SelectedItems.Count > 2;
        }

        private static void Delete_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                DeleteSelection(editor);
            }
        }

        private static void DeleteSelection(HeliosVisualContainerEditor editor)
        {
            List<HeliosVisual> removedControls = new List<HeliosVisual>();
            List<int> removedIndexes = new List<int>();

            foreach (HeliosVisual control in editor.SelectedItems)
            {
                removedControls.Add(control);
                removedIndexes.Add(editor.VisualContainer.Children.IndexOf(control));
                if(control is EffectColorAdjuster eFC)
                {
                    eFC.DeleteEffect();
                }
            }

            foreach (HeliosVisual control in removedControls)
            {
                editor.VisualContainer.Children.Remove(control);
                CloseDoucments(control, editor);
            }

            ConfigManager.UndoManager.AddUndoItem(new ControlDeleteUndoEvent(editor.VisualContainer, removedControls, removedIndexes));

            editor.SelectedItems.Clear();
        }

        private static void CloseDoucments(HeliosVisual visual, HeliosVisualContainerEditor editor)
        {
            ProfileEditorCommands.CloseProfileItem.Execute(visual, editor);
            foreach (HeliosVisual child in visual.Children)
            {
                CloseDoucments(child, editor);
            }
        }

        private static void MoveForward_Executed(object target, ExecutedRoutedEventArgs e)
        {
            (target as HeliosVisualContainerEditor)?.MoveSelectedItemsForward();

        }

        private static void MoveBack_Executed(object target, ExecutedRoutedEventArgs e)
        {
            (target as HeliosVisualContainerEditor)?.MoveSelectedItemsBack();
        }

        public void MoveSelectedItemsForward()
        {
            ConfigManager.UndoManager.StartBatch();
            foreach (HeliosVisual control in VisualContainer.Children.Reverse())
            {
                if (SelectedItems.Contains(control))
                {
                    ConfigManager.UndoManager.AddUndoItem(new DisplayOrderUndoItem(VisualContainer, control, true));
                    VisualContainer.Children.MoveUp(control);
                }
            }
            ConfigManager.UndoManager.CloseBatch();
        }

        public void MoveSelectedItemsBack()
        {
            ConfigManager.UndoManager.StartBatch();
            for (int i = 0; i < VisualContainer.Children.Count; i++)
            {
                HeliosVisual control = VisualContainer.Children[i];
                if (SelectedItems.Contains(control))
                {
                    ConfigManager.UndoManager.AddUndoItem(new DisplayOrderUndoItem(VisualContainer, control, false));
                    VisualContainer.Children.MoveDown(control);
                }
            }

            ConfigManager.UndoManager.CloseBatch();
        }

        public void MoveSelectedItemsForwardFully()
        {
            ConfigManager.UndoManager.StartBatch();
            int lastChild = VisualContainer.Children.Count - 1;
            int newPosition = lastChild;
            for (int i = lastChild; i >= 0; i--)
            {
                HeliosVisual control = VisualContainer.Children[i];
                if (!SelectedItems.Contains(control))
                {
                    continue;
                }

                if (i != newPosition)
                {
                    // perform the move
                    MoveVisualUndoItem undoItem = new MoveVisualUndoItem(VisualContainer, control, i, newPosition);
                    ConfigManager.UndoManager.AddUndoItem(undoItem);
                    undoItem.Do();
                }

                // next item goes ahead of here
                newPosition--;
            }
            ConfigManager.UndoManager.CloseBatch();
        }

        public void MoveSelectedItemsBackFully()
        {
            ConfigManager.UndoManager.StartBatch();
            int newPosition = 0;
            for (int i = 0; i < VisualContainer.Children.Count; i++)
            {
                HeliosVisual control = VisualContainer.Children[i];
                if(!SelectedItems.Contains(control))
                {
                    continue;
                }

                if (i != newPosition)
                {
                    // perform the move
                    MoveVisualUndoItem undoItem = new MoveVisualUndoItem(VisualContainer, control, i, newPosition);
                    ConfigManager.UndoManager.AddUndoItem(undoItem);
                    undoItem.Do();
                }

                // next item goes after this one
                newPosition++;
            }

            ConfigManager.UndoManager.CloseBatch();
        }


        private static void AlignTop_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;

            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();
                double top = editor.SelectedItems[0].Top;
                for (int i = 1; i < editor.SelectedItems.Count; i++)
                {
                    editor.SelectedItems[i].Top = top;
                }
                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void AlignBottom_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();
                double bottom = editor.SelectedItems[0].Top + editor.SelectedItems[0].Height;
                for (int i = 1; i < editor.SelectedItems.Count; i++)
                {
                    editor.SelectedItems[i].Top = bottom - editor.SelectedItems[i].Height;
                }
                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void AlignRight_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();
                double right = editor.SelectedItems[0].Left + editor.SelectedItems[0].Width;
                for (int i = 1; i < editor.SelectedItems.Count; i++)
                {
                    editor.SelectedItems[i].Left = right - editor.SelectedItems[i].Width;
                }
                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void AlignLeft_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();
                double left = editor.SelectedItems[0].Left;
                for (int i = 1; i < editor.SelectedItems.Count; i++)
                {
                    editor.SelectedItems[i].Left = left;
                }
                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void AlignHorizontalCenter_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();
                double center = editor.SelectedItems[0].Top + (editor.SelectedItems[0].Height / 2d);
                for (int i = 1; i < editor.SelectedItems.Count; i++)
                {
                    editor.SelectedItems[i].Top = center - (editor.SelectedItems[i].Height / 2d);
                }
                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void AlignVerticalCenter_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();
                double center = editor.SelectedItems[0].Left + (editor.SelectedItems[0].Width / 2d);
                for (int i = 1; i < editor.SelectedItems.Count; i++)
                {
                    editor.SelectedItems[i].Left = center - (editor.SelectedItems[i].Width / 2d);
                }
                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void DistributeHorizontalCenter_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();

                List<HeliosVisual> sortedControls = (from control in editor.SelectedItems orderby control.Left select control).ToList();

                HeliosVisual leftControl = sortedControls[0];
                HeliosVisual rightControl = sortedControls[sortedControls.Count - 1];
                double leftCenter = leftControl.Left + (leftControl.Width / 2d);
                double rightCenter = rightControl.Left + (rightControl.Width / 2d);
                double spacing = (rightCenter - leftCenter) / (double)(sortedControls.Count - 1);
                double currentCenter = leftCenter;

                for (int i = 1; i < sortedControls.Count - 1; i++)
                {
                    currentCenter += spacing;
                    sortedControls[i].Left = currentCenter - (sortedControls[i].Width / 2d);
                }

                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void DistributeVerticalCenter_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();

                List<HeliosVisual> sortedControls = (from control in editor.SelectedItems orderby control.Top select control).ToList();

                HeliosVisual topControl = sortedControls[0];
                HeliosVisual bottomControl = sortedControls[sortedControls.Count - 1];
                double topCenter = topControl.Top + (topControl.Height / 2d);
                double bottomCenter = bottomControl.Top + (bottomControl.Height / 2d);
                double spacing = (bottomCenter - topCenter) / (double)(sortedControls.Count - 1);
                double currentCenter = topCenter;

                for (int i = 1; i < sortedControls.Count - 1; i++)
                {
                    currentCenter += spacing;
                    sortedControls[i].Top = currentCenter - (sortedControls[i].Height / 2d);
                }

                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void SpaceHorizontal_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();

                List<HeliosVisual> sortedControls = (from control in editor.SelectedItems orderby control.Left select control).ToList();

                HeliosVisual leftControl = sortedControls[0];
                HeliosVisual rightControl = sortedControls[sortedControls.Count - 1];

                double totalWidth = rightControl.Left + rightControl.Width - leftControl.Left;

                double controlsWidth = 0;
                foreach(HeliosVisual visual in sortedControls)
                {
                    controlsWidth += visual.Width;
                }

                double spacing = (totalWidth - controlsWidth) / (double)(sortedControls.Count - 1);
                double currentLeft = leftControl.Left + leftControl.Width;

                for (int i = 1; i < sortedControls.Count - 1; i++)
                {
                    currentLeft += spacing;
                    sortedControls[i].Left = currentLeft;
                    currentLeft += sortedControls[i].Width;
                }

                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void SpaceVertical_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                ConfigManager.UndoManager.StartBatch();

                List<HeliosVisual> sortedControls = (from control in editor.SelectedItems orderby control.Top select control).ToList();

                HeliosVisual topControl = sortedControls[0];
                HeliosVisual bottomControl = sortedControls[sortedControls.Count - 1];

                double totalHeight = bottomControl.Top + bottomControl.Height - topControl.Top;

                double controlsHeight = 0;
                foreach (HeliosVisual visual in sortedControls)
                {
                    controlsHeight += visual.Height;
                }

                double spacing = (totalHeight - controlsHeight) / (double)(sortedControls.Count - 1);
                double currentTop = topControl.Top + topControl.Height;

                for (int i = 1; i < sortedControls.Count - 1; i++)
                {
                    currentTop += spacing;
                    sortedControls[i].Top = currentTop;
                    currentTop += sortedControls[i].Height;
                }

                ConfigManager.UndoManager.CloseBatch();
            }
        }

        private static void Stop_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            e.CanExecute = editor != null && (editor._mouseState != EditorMouseState.Idle || editor.SelectedItems.Count > 0);
        }
        
        private static void Stop_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            e.Handled = false;
            if (editor != null)
            {
                switch (editor._mouseState)
                {
                    case EditorMouseState.Idle:
                        editor.SelectedItems.Clear();
                        e.Handled = true;
                        break;
                    case EditorMouseState.Move:
                        editor._mouseState = EditorMouseState.Idle;
                        editor.SnapManager.Action = SnapAction.None;
                        editor.GetSelectionAdorner(false).InvalidateVisual();
                        editor.ReleaseMouseCapture();
                        editor.Cursor = null;
                        e.Handled = true;
                        break;
                    case EditorMouseState.Dropping:
                        editor._mouseState = EditorMouseState.Idle;
                        e.Handled = true;
                        break;
                }
            }
        }

        private static void Cut_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            e.CanExecute = editor != null && (editor.SelectedItems.Count > 0);
        }

        private static void Cut_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                if (editor.SelectedItems.Count > 0)
                {
                    CopySelection(editor.VisualContainer, editor.SelectedItems);
                    DeleteSelection(editor);
                }
            }
        }

        private static void Copy_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            e.CanExecute = editor != null && (editor.SelectedItems.Count > 0);
        }

        private static void Copy_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                if (editor.SelectedItems.Count > 0)
                {
                    CopySelection(editor.VisualContainer, editor.SelectedItems);
                }
            }
        }

        private static void Paste_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData("Helios.Visuals") || Clipboard.ContainsText();
        }

        private static void Paste_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;

            if (editor == null) 
            {
                // no editor selected to receive the paste operation
                return;
            }

            if (!(Clipboard.ContainsData("Helios.Visuals") || Clipboard.ContainsText()))
            {
                // nothing we can decode
                return;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.CloseInput = true;

            StringReader reader = new StringReader(Clipboard.GetText());
            XmlReader xmlReader = XmlReader.Create(reader, settings);

            try
            {
                HeliosSerializer serializer = new HeliosSerializer(editor.Dispatcher);

                ConfigManager.UndoManager.StartBatch();

                xmlReader.ReadStartElement("HeliosCopyBuffer");
                string copyRoot = xmlReader.ReadElementString("CopyRoot");

                HeliosVisualCollection newControls = new HeliosVisualCollection();
                foreach (string progress in serializer.DeserializeControls(newControls, xmlReader))
                {
                    ConfigManager.LogManager.LogDebug($"Paste operation {progress}");
                }

                List<HeliosVisual> localObjects = new List<HeliosVisual>();
                foreach (HeliosVisual control in newControls)
                {
                    if(!(control.IsUnique && editor.VisualContainer.Children.Where(visual => visual.TypeIdentifier == control.TypeIdentifier)
                   .Select(visual => visual.TypeIdentifier)
                   .Count() > 0)){
                        localObjects.Add(control);
                    }
                }

                // load all these now, instead of streaming enumeration (just for ease of debugging really)
                IList<HeliosBinding> bindings = serializer
                    .DeserializeBindings(editor.VisualContainer, copyRoot, localObjects, xmlReader).ToList();

                xmlReader.ReadEndElement();
                newControls.Clear();

                if (localObjects.Count > 0)
                {
                    editor.SelectedItems.Clear();
                    foreach (HeliosVisual control in localObjects)
                    {
                        if (control.Left + control.Width > editor.VisualContainer.Width)
                        {
                            control.Left = Math.Max(0d, editor.VisualContainer.Width - control.DisplayRectangle.Width);
                        }

                        if (control.Top + control.Height > editor.VisualContainer.Height)
                        {
                            control.Top = Math.Max(0d, editor.VisualContainer.Height - control.DisplayRectangle.Height);
                        }

                        control.Name = editor.VisualContainer.Children.GetUniqueName(control);
                        editor.VisualContainer.Children.Add(control);

                        ConfigManager.UndoManager.AddUndoItem(new ControlAddUndoEvent(editor.VisualContainer, control));

                        editor.SelectedItems.Add(control);
                    }

                    foreach (HeliosBinding binding in bindings)
                    {
                        /// Revisit: The following test guarding the binding is part of the ugly Reset Monitors /  Cut/Paste
                        /// functionality.  In this case the test is to avoid the accummulation of identical bindings
                        /// when *paste* is performed on a compositevisual.  CompositeVisual has already got in and created
                        /// bindings for this compositevisual so before we re-add the bindings from the cut/copied visual,
                        /// we check to see if identical bindings are already there, and if they are, we skip this Add for 
                        /// the original binding.
                        /// 
                        if (!(binding.Trigger.Source.OutputBindings.Any(OutputBinding => OutputBinding.Description == binding.Description) 
                            && binding.Action.Target.InputBindings.Any(InputBinding => InputBinding.Description == binding.Description)))
                        {
                            ConfigManager.UndoManager.AddUndoItem(new BindingAddUndoEvent(binding));
                            binding.Trigger.Source.OutputBindings.Add(binding);
                            binding.Action.Target.InputBindings.Add(binding);
                        }
                        else
                        {
                            Logger.Debug($"Duplicate Binding for {binding.Description} detected and suppressed");

    }
}
                }

                ConfigManager.UndoManager.CloseBatch();
            }
            catch (Exception)
            {
                // failed paste operation, probably due to bad format
                Logger.Error("Failed to paste from clipboard.  Contents must be XML in the HeliosCopyBuffer format.");
                ConfigManager.UndoManager.UndoBatch();
            }
            finally
            {
                xmlReader.Close();
            }
        }

        private static void CopySelection(HeliosVisualContainer root, HeliosVisualCollection controls)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            // Loop through controls and add them in the proper visual order for serialization to the copy buffer.
            HeliosVisualCollection copiedControls = new HeliosVisualCollection();
            foreach (HeliosVisual visual in root.Children)
            {
                if (controls.Contains(visual))
                {
                    copiedControls.Add(visual);
                }
            }

            StringBuilder sb = new StringBuilder();
            XmlWriter xmlWriter = XmlWriter.Create(sb, settings);
            xmlWriter.WriteStartElement("HeliosCopyBuffer");

            xmlWriter.WriteElementString("CopyRoot", HeliosSerializer.GetVisualPath(root));

            HeliosSerializer serializer = new HeliosSerializer(null);
            serializer.SerializeControls(copiedControls, xmlWriter);
            HeliosBindingCollection serializedBindings = new HeliosBindingCollection();

            xmlWriter.WriteStartElement("Bindings");
            foreach (HeliosVisual control in copiedControls)
            {
                SerializeBindings(serializer, control, xmlWriter, serializedBindings);
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            xmlWriter.Close();

            SetClipboard("Helios.Visuals", sb.ToString());
        }

        private static void SerializeBindings(HeliosSerializer serializer, HeliosVisual control, XmlWriter xmlWriter, HeliosBindingCollection serializedBindings)
        {
            serializer.SerializeBindings(control.InputBindings, xmlWriter, serializedBindings);
            serializer.SerializeBindings(control.OutputBindings, xmlWriter, serializedBindings);
            foreach (HeliosVisual child in control.Children)
            {
                SerializeBindings(serializer, child, xmlWriter, serializedBindings);
            }
        }

        private static void SetClipboard(string dataType, string data)
        {
            DataObject copyBuffer = new DataObject();
            copyBuffer.SetData(dataType, "Y");
            copyBuffer.SetText(data);

            try
            {
                Clipboard.SetDataObject(copyBuffer);
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                System.Threading.Thread.Sleep(0);
                try
                {
                    Clipboard.SetDataObject(copyBuffer);
                }
                catch (System.Runtime.InteropServices.COMException e)
                {
                    ConfigManager.LogManager.LogError("Error writing to clipboard.", e);
                }
            }
        }

        private static void SaveTemplate_CanExecute(object target, CanExecuteRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            e.CanExecute = editor != null && editor.SelectedItems != null && editor.SelectedItems.Count == 1;
        }

        private static void SaveTemplate_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                if (editor.SelectedItems.Count > 0)
                {
                    HeliosVisual control = editor.SelectedItems[0];
                    SaveTemplateDialog dialog = new SaveTemplateDialog();
                    dialog.TemplateName = control.Name;
                    dialog.TemplateCategory = "User Templates";
                    dialog.Owner = Window.GetWindow(editor);

                    bool? results = dialog.ShowDialog();
                    if (results == true)
                    {
                        HeliosTemplate template = new HeliosTemplate(control);
                        template.Name = dialog.TemplateName;
                        template.Category = dialog.TemplateCategory;

                        string templateKey = ConfigManager.TemplateManager.UserTemplates.GetKeyForItem(template);
                        if (ConfigManager.TemplateManager.UserTemplates.ContainsKey(templateKey))
                        {
                            if (MessageBox.Show(Window.GetWindow(editor), "A template already exists with that name.  Do you want to overwrite the existing template?", "Overwrite Template", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                            {
                                ConfigManager.TemplateManager.UserTemplates.RemoveKey(templateKey);
                                ConfigManager.TemplateManager.UserTemplates.Add(template);
                            }
                        }
                        else
                        {
                            ConfigManager.TemplateManager.UserTemplates.Add(template);
                        }
                    }
                }
            }
        }

        private static void SelectAll_Executed(object target, ExecutedRoutedEventArgs e)
        {
            HeliosVisualContainerEditor editor = target as HeliosVisualContainerEditor;
            if (editor != null)
            {
                foreach (HeliosVisual visual in editor.VisualContainer.Children)
                {
                    if (!editor.SelectedItems.Contains(visual))
                    {
                        editor.SelectedItems.Add(visual);
                    }
                }
            }
        }

        private class MoveVisualUndoItem: IUndoItem
        {
            private readonly HeliosVisualContainer _visualContainer;
            private readonly HeliosVisual _control;
            private readonly int _oldPosition;
            private readonly int _newPosition;

            public MoveVisualUndoItem(HeliosVisualContainer visualContainer, HeliosVisual control, int oldPosition, int newPosition)
            {
                _visualContainer = visualContainer;
                _control = control;
                _oldPosition = oldPosition;
                _newPosition = newPosition;
            }

            public void Undo()
            {
                Debug.Assert(ReferenceEquals(_control, _visualContainer.Children[_newPosition]));
                _visualContainer.Children.Move(_newPosition, _oldPosition);
                Debug.Assert(ReferenceEquals(_control, _visualContainer.Children[_oldPosition]));
            }

            public void Do()
            {
                Debug.Assert(ReferenceEquals(_control, _visualContainer.Children[_oldPosition]));
                _visualContainer.Children.Move(_oldPosition, _newPosition);
                Debug.Assert(ReferenceEquals(_control, _visualContainer.Children[_newPosition]));
            }
        }

        #endregion
    }
}
