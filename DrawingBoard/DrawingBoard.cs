﻿using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SimzzDev
{
    public class DrawingBoard
    {
        //WINRT
        //http://code.msdn.microsoft.com/InkPen-sample-in-CSharp-189ce853

        Stroke _stroke;
        StylusPointCollection _erasePoints;
        StrokeCollection _allErasedStrokes = new StrokeCollection();
        InkPresenter _presenter;
        private Nullable<StylusPoint> _lastPoint = null;
        private bool _useOverlay = false;
        private bool _dan_mode = false;

        public enum PenMode { Pen, Erase };

        public InkPresenter Ink
        {
            get { return _presenter; }
        }

        //props
        public PenMode InkMode { get; set; }
        public Color MainColor { get; set; }
        public Color OutlineColor { get; set; }
        public int BrushWidth { get; set; }
        public int BrushHeight { get; set; }

        public ImageSource MyImage { get; set; }

        public DrawingBoard(InkPresenter Ink, bool useOverlay)
        {
            _presenter = Ink;

            if (!_useOverlay)
            {
                AssignHandlers();
            }

            //defaults some properties so drawing will work
            InkMode = PenMode.Pen;
            MainColor = Colors.Black;
            OutlineColor = Colors.Black;
            BrushWidth = 2;
            BrushHeight = 2;
        }


        private void AssignHandlers()
        {
            UnAssignHandlers();

            if (!_dan_mode)
            {
                _presenter.MouseLeftButtonDown += new MouseButtonEventHandler(Ink_MouseLeftButtonDown);
                _presenter.MouseLeftButtonUp += new MouseButtonEventHandler(Ink_MouseLeftButtonUp);
                _presenter.MouseMove += new MouseEventHandler(Ink_MouseMove);
                _presenter.MouseLeave += new MouseEventHandler(Ink_MouseLeave);
            }
            else
            {
                _presenter.MouseLeftButtonDown += new MouseButtonEventHandler(Ink_MouseLeftButtonDown_D);
                _presenter.MouseLeftButtonUp += new MouseButtonEventHandler(Ink_MouseLeftButtonUp_D);
                _presenter.MouseMove += new MouseEventHandler(Ink_MouseMove_D);
            }

            _presenter.LayoutUpdated += _presenter_LayoutUpdated;
            _presenter.ManipulationStarted += _presenter_ManipulationStarted;
            _presenter.ManipulationCompleted += _presenter_ManipulationCompleted;
        }

        private void UnAssignHandlers()
        {
            if (!_dan_mode)
            {
                _presenter.MouseLeftButtonDown -= new MouseButtonEventHandler(Ink_MouseLeftButtonDown);
                _presenter.MouseLeftButtonUp -= new MouseButtonEventHandler(Ink_MouseLeftButtonUp);
                _presenter.MouseMove -= new MouseEventHandler(Ink_MouseMove);
                _presenter.MouseLeave -= new MouseEventHandler(Ink_MouseLeave);
            }
            else
            {
                _presenter.MouseLeftButtonDown -= new MouseButtonEventHandler(Ink_MouseLeftButtonDown_D);
                _presenter.MouseLeftButtonUp -= new MouseButtonEventHandler(Ink_MouseLeftButtonUp_D);
                _presenter.MouseMove -= new MouseEventHandler(Ink_MouseMove_D);
            }

            _presenter.LayoutUpdated -= _presenter_LayoutUpdated;
            _presenter.ManipulationStarted -= _presenter_ManipulationStarted;
            _presenter.ManipulationCompleted -= _presenter_ManipulationCompleted;
        }

        void _presenter_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            //Debug.WriteLine("_presenter_ManipulationCompleted");
            //if (MyImage == null) return;
            //try
            //{
            //    Image img = new Image();
            //    img.Source = MyImage;
            //    img.Width = _presenter.Width;
            //    img.Height = _presenter.Height;
            //    Canvas.SetLeft(img, 0);
            //    Canvas.SetTop(img, 0);
            //    _presenter.Children.Add(img);
            //}
            //catch (Exception ex)
            //{
            //}
        }

        void _presenter_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
        }

        void _presenter_LayoutUpdated(object sender, EventArgs e)
        {
            //            ColorMatrix cm = new ColorMatrix();
            //cm.Matrix33 = 0.55f;
            //ImageAttributes ia = new ImageAttributes();
            //ia.SetColorMatrix(cm);
            //canvas.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel

        }

        private void ResetPen()
        {
            _stroke = null;
            _presenter.ReleaseMouseCapture();
        }

        public void Ink_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetPen();
        }

        //private void _presenter_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (_stroke != null)
        //    {
        //        _stroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(_presenter));
        //        borderInk.Children.Add(new System.Windows.Shapes.Rectangle()
        //        {
        //            Width = rectangleSize,
        //            Height = rectangleSize,
        //            Fill = lBrush
        //        });
        //        Canvas.SetLeft((System.Windows.Shapes.Rectangle)borderInk.Children[borderInk.Children.Count - 1], _stroke.StylusPoints[_stroke.StylusPoints.Count - 1].X);
        //        Canvas.SetTop((System.Windows.Shapes.Rectangle)borderInk.Children[borderInk.Children.Count - 1], _stroke.StylusPoints[_stroke.StylusPoints.Count - 1].Y);
        //    }
        //}

        #region standard
        public void Ink_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(sender as UIElement);
            if (pos.X < 0 || pos.Y < 0 || pos.X > _presenter.Width || pos.Y > _presenter.Height)
            {
                ResetPen();
                return;
            }
            if (InkMode == PenMode.Pen)
            {
                if (_stroke != null)
                {
                    StylusPointCollection sp = e.StylusDevice.GetStylusPoints(_presenter);
                    _stroke.StylusPoints.Add(sp);
                }
                return;
            }
            else if (InkMode == PenMode.Erase)
            {
                if (_lastPoint != null)
                {
                    StylusPointCollection pointErasePoints = e.StylusDevice.GetStylusPoints(_presenter);
                    pointErasePoints.Insert(0, _lastPoint.Value);
                    //Compare collected stylus points with the ink presenter strokes and store the intersecting strokes.
                    StrokeCollection hitStrokes = _presenter.Strokes.HitTest(pointErasePoints);
                    if (hitStrokes.Count > 0)
                    {
                        foreach (Stroke hitStroke in hitStrokes)
                        {
                            //For each intersecting stroke, split the stroke into two while removing the intersecting points.
                            ProcessPointErase(hitStroke, pointErasePoints);
                        }
                    }
                    _lastPoint = pointErasePoints[pointErasePoints.Count - 1];
                }
            }
        }

        public void Ink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _stroke = null;
            //  _presenter.ReleaseMouseCapture();
        }

        public void Ink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //_presenter.CaptureMouse();
            if (InkMode == PenMode.Pen)
            {
                _stroke = new Stroke();
                _stroke.DrawingAttributes.Color = MainColor;
                _stroke.DrawingAttributes.Height = BrushHeight;
                _stroke.DrawingAttributes.Width = BrushWidth;
                _stroke.DrawingAttributes.OutlineColor = OutlineColor;
                _stroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(_presenter));
                _presenter.Strokes.Add(_stroke);
            }
            else
            {
                //_erasePoints = e.StylusDevice.GetStylusPoints(_presenter);
                StylusPointCollection pointErasePoints = e.StylusDevice.GetStylusPoints(_presenter);
                //Store the last point in the stylus point collection.
                _lastPoint = pointErasePoints[pointErasePoints.Count - 1];
            }
        }

        void ProcessPointErase(Stroke stroke, StylusPointCollection pointErasePoints)
        {
            Stroke splitStroke1, splitStroke2, hitTestStroke;

            // Determine first split stroke.
            splitStroke1 = new Stroke();
            hitTestStroke = new Stroke();
            hitTestStroke.StylusPoints.Add(stroke.StylusPoints);
            hitTestStroke.DrawingAttributes = stroke.DrawingAttributes;
            //Iterate through the stroke from index 0 and add each stylus point to splitstroke1 until 
            //a stylus point that intersects with the input stylus point collection is reached.
            while (true)
            {
                StylusPoint sp = hitTestStroke.StylusPoints[0];
                hitTestStroke.StylusPoints.RemoveAt(0);
                if (!hitTestStroke.HitTest(pointErasePoints)) break;
                splitStroke1.StylusPoints.Add(sp);
            }

            // Determine second split stroke.
            splitStroke2 = new Stroke();
            hitTestStroke = new Stroke();
            hitTestStroke.StylusPoints.Add(stroke.StylusPoints);
            hitTestStroke.DrawingAttributes = stroke.DrawingAttributes;
            while (true)
            {
                StylusPoint sp = hitTestStroke.StylusPoints[hitTestStroke.StylusPoints.Count - 1];
                hitTestStroke.StylusPoints.RemoveAt(hitTestStroke.StylusPoints.Count - 1);
                if (!hitTestStroke.HitTest(pointErasePoints)) break;
                splitStroke2.StylusPoints.Insert(0, sp);
            }

            // Replace stroke with splitstroke1 and splitstroke2.
            if (splitStroke1.StylusPoints.Count > 1)
            {
                splitStroke1.DrawingAttributes = stroke.DrawingAttributes;
                _presenter.Strokes.Add(splitStroke1);
            }
            if (splitStroke2.StylusPoints.Count > 1)
            {
                splitStroke2.DrawingAttributes = stroke.DrawingAttributes;
                _presenter.Strokes.Add(splitStroke2);
            }
            _presenter.Strokes.Remove(stroke);
        }
        #endregion

        #region from dan
        private void Ink_MouseMove_D(object sender, MouseEventArgs e)
        {
            if (_stroke != null)
            {
                _stroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(_presenter));
                //borderInk.Children.Add(new System.Windows.Shapes.Rectangle()
                //{
                //    Width = rectangleSize,
                //    Height = rectangleSize,
                //    Fill = lBrush
                //});
                //Canvas.SetLeft((System.Windows.Shapes.Rectangle)borderInk.Children[borderInk.Children.Count - 1], _stroke.StylusPoints[_stroke.StylusPoints.Count - 1].X);
                //Canvas.SetTop((System.Windows.Shapes.Rectangle)borderInk.Children[borderInk.Children.Count - 1], _stroke.StylusPoints[_stroke.StylusPoints.Count - 1].Y);
            }
        }

        private void Ink_MouseLeftButtonDown_D(object sender, MouseButtonEventArgs e)
        {
            _presenter.CaptureMouse();
            _stroke = new Stroke();
            _stroke.DrawingAttributes.Color = MainColor;
            _stroke.DrawingAttributes.Height = BrushHeight;
            _stroke.DrawingAttributes.Width = BrushWidth;
            _stroke.DrawingAttributes.OutlineColor = OutlineColor;
            _stroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(_presenter));
            _presenter.Strokes.Add(_stroke);

            //borderInk.Children.Add(new System.Windows.Shapes.Rectangle()
            //{
            //    Width = rectangleSize,
            //    Height = rectangleSize,
            //    Fill = lBrush
            //});
            //Canvas.SetLeft((System.Windows.Shapes.Rectangle)borderInk.Children[borderInk.Children.Count - 1], _stroke.StylusPoints[_stroke.StylusPoints.Count - 1].X);
            //Canvas.SetTop((System.Windows.Shapes.Rectangle)borderInk.Children[borderInk.Children.Count - 1], _stroke.StylusPoints[_stroke.StylusPoints.Count - 1].Y);
        }

        private void Ink_MouseLeftButtonUp_D(object sender, MouseButtonEventArgs e)
        {
            if (_stroke != null)
            {
                _stroke.StylusPoints.Add(GetStylusPoint(e.GetPosition(_presenter)));
            }
            _stroke = null;
        }

        #endregion



        private StylusPoint GetStylusPoint(Point position)
        {
            return new StylusPoint(position.X, position.Y);
        }

        public void UndoLast(PenMode inkMode)
        {
            if (inkMode == PenMode.Pen && _presenter.Strokes.Count > 0)
            {
                _presenter.Strokes.RemoveAt(_presenter.Strokes.Count - 1);
            }
            else if (inkMode == PenMode.Erase && _allErasedStrokes.Count > 0)
            {
                _presenter.Strokes.Add(_allErasedStrokes[_allErasedStrokes.Count - 1]);
                _allErasedStrokes.RemoveAt(_allErasedStrokes.Count - 1);
            }

        }

        public void Clear()
        {
            _presenter.Strokes.Clear();
        }
    }
}
