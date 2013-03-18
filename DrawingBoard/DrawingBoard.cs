using System;
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
       
        Stroke _stroke;
        StylusPointCollection _erasePoints;
        StrokeCollection _allErasedStrokes = new StrokeCollection();
        InkPresenter _presenter;
        private Nullable<StylusPoint> _lastPoint = null;

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


        public DrawingBoard(InkPresenter Ink)
        {
            _presenter = Ink;
            _presenter.MouseLeftButtonDown += new MouseButtonEventHandler(ink_MouseLeftButtonDown);
            _presenter.MouseLeftButtonUp += new MouseButtonEventHandler(ink_MouseLeftButtonUp);
            _presenter.MouseMove += new MouseEventHandler(ink_MouseMove);
            _presenter.MouseLeave += new MouseEventHandler(ink_MouseLeave);
           
            //defaults some properties so drawing will work
            InkMode = PenMode.Pen;
            MainColor = Colors.Black;
            OutlineColor = Colors.Black;
            BrushWidth = 2;
            BrushHeight = 2;
        }



        void ink_MouseLeave(object sender, MouseEventArgs e)
        {
            ResetPen();
        }

        private void ResetPen()
        {
            _stroke = null;
            _presenter.ReleaseMouseCapture();
        }


        void ink_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = e.GetPosition(sender as UIElement);
            if (pos.X < 0 || pos.Y < 0 || pos.X > _presenter.Width || pos.Y > _presenter.Height)
            {
                ResetPen();
                return;
            }

            if (InkMode == PenMode.Pen && _stroke != null)
            {
                var sp = e.StylusDevice.GetStylusPoints(_presenter);
                _stroke.StylusPoints.Add(sp);
            }
            //orig
            //if (InkMode == PenMode.erase && _erasePoints != null)
            //{
            //    _erasePoints.Add(e.StylusDevice.GetStylusPoints(_presenter));
            //    StrokeCollection hitStrokes = _presenter.Strokes.HitTest(_erasePoints);
            //    if (hitStrokes.Count > 0)
            //    {
            //        foreach (Stroke hitStroke in hitStrokes)
            //        {
            //            _allErasedStrokes.Add(hitStroke);
            //            _presenter.Strokes.Remove(hitStroke);
            //        }
            //    }
            //}

            //test
            if (InkMode == PenMode.Erase && _lastPoint != null)
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

        void ink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _stroke = null;
            _presenter.ReleaseMouseCapture();
        }

        void ink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _presenter.CaptureMouse();
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

        #region Crap
        //public void MouseLeftButtonDown(MouseButtonEventArgs e)
        //{
        //    ink.CaptureMouse();
        //    if (InkMode == PenMode.pen)
        //    {
        //        stroke = new Stroke();
        //        stroke.DrawingAttributes.Color = MainColor;
        //        stroke.DrawingAttributes.Height = BrushHeight;
        //        stroke.DrawingAttributes.Width = BrushWidth;
        //        stroke.DrawingAttributes.OutlineColor = OutlineColor;
        //        stroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(ink));
        //        ink.Strokes.Add(stroke);
        //    }
        //    else
        //    {
        //        ErasePoints = e.StylusDevice.GetStylusPoints(ink);
        //    }
        //}

        //public void MouseLeftButtonUp()
        //{
        //    stroke = null;
        //    ink.ReleaseMouseCapture();
        //}

        //public void MouseLeave()
        //{
        //    stroke = null;
        //    ink.ReleaseMouseCapture();
        //}

        //public void MouseMove(MouseEventArgs e)
        //{
        //    if (InkMode == PenMode.pen && stroke != null)
        //    {
        //        stroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(ink));
        //    }

        //    if (InkMode == PenMode.erase && ErasePoints != null)
        //    {
        //        ErasePoints.Add(e.StylusDevice.GetStylusPoints(ink));
        //        StrokeCollection hitStrokes = ink.Strokes.HitTest(ErasePoints);
        //        if (hitStrokes.Count > 0)
        //        {
        //            foreach (Stroke hitStroke in hitStrokes)
        //            {
        //                allErasedStrokes.Add(hitStroke);
        //                ink.Strokes.Remove(hitStroke);
        //            }
        //        }

        //    }
        //}

        #endregion

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
