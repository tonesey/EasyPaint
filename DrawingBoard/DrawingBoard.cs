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
        InkPresenter _ink;

        public enum PenMode { pen, erase };

        public InkPresenter Ink
        {
            get { return _ink; }
        }

        //props
        public PenMode InkMode { get; set; }
        public Color MainColor { get; set; }
        public Color OutlineColor { get; set; }
        public int BrushWidth { get; set; }
        public int BrushHeight { get; set; }


        public DrawingBoard(InkPresenter Ink)
        {
            _ink = Ink;
            _ink.MouseLeftButtonDown += new MouseButtonEventHandler(ink_MouseLeftButtonDown);
            _ink.MouseLeftButtonUp += new MouseButtonEventHandler(ink_MouseLeftButtonUp);
            _ink.MouseMove += new MouseEventHandler(ink_MouseMove);
            _ink.MouseLeave += new MouseEventHandler(ink_MouseLeave);
            //defaults some properties so drawing will work
            InkMode = PenMode.pen;
            MainColor = Colors.Black;
            OutlineColor = Colors.Black;
            BrushWidth = 2;
            BrushHeight = 2;
        }



        void ink_MouseLeave(object sender, MouseEventArgs e)
        {
            _stroke = null;
            _ink.ReleaseMouseCapture();
        }

        void ink_MouseMove(object sender, MouseEventArgs e)
        {
            if (InkMode == PenMode.pen && _stroke != null)
            {
                _stroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(_ink));
            }

            if (InkMode == PenMode.erase && _erasePoints != null)
            {
                _erasePoints.Add(e.StylusDevice.GetStylusPoints(_ink));
                StrokeCollection hitStrokes = _ink.Strokes.HitTest(_erasePoints);
                if (hitStrokes.Count > 0)
                {
                    foreach (Stroke hitStroke in hitStrokes)
                    {
                        _allErasedStrokes.Add(hitStroke);
                        _ink.Strokes.Remove(hitStroke);
                    }
                }
            }
        }


       
        void ink_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _stroke = null;
            _ink.ReleaseMouseCapture();
        }

        void ink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ink.CaptureMouse();
            if (InkMode == PenMode.pen)
            {
                _stroke = new Stroke();
                _stroke.DrawingAttributes.Color = MainColor;
                _stroke.DrawingAttributes.Height = BrushHeight;
                _stroke.DrawingAttributes.Width = BrushWidth;
                _stroke.DrawingAttributes.OutlineColor = OutlineColor;
                _stroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(_ink));
                _ink.Strokes.Add(_stroke);
            }
            else
            {
                _erasePoints = e.StylusDevice.GetStylusPoints(_ink);
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

        public void undoLast(PenMode inkMode)
        {
            if (inkMode == PenMode.pen && _ink.Strokes.Count > 0)
            {
                _ink.Strokes.RemoveAt(_ink.Strokes.Count - 1);
            }
            else if (inkMode == PenMode.erase && _allErasedStrokes.Count > 0)
            {
                _ink.Strokes.Add(_allErasedStrokes[_allErasedStrokes.Count - 1]);
                _allErasedStrokes.RemoveAt(_allErasedStrokes.Count - 1);
            }

        }

    }
}
