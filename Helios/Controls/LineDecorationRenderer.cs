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

namespace GadrocsWorkshop.Helios.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public class LineDecorationRenderer : HeliosVisualRenderer
    {
        private PathGeometry _path;
        private Pen _pathPen;

        protected override void OnRender(DrawingContext drawingContext)
        {
            /// TODO: Effects on a line (both straight and curved need to be implemented at some point 
            //if (!NeedsEffect || (Visual as LineDecoration).Curve)
            //{
                drawingContext.DrawGeometry(null, _pathPen, _path);
            //}
            //else
            //{
            //    DrawingVisual visual = new DrawingVisual();
            //    DrawingContext tempDrawingContext = visual.RenderOpen();
            //    drawingContext.DrawGeometry(null, _pathPen, _path);
            //    tempDrawingContext.Close();
            //    LineDecoration line = Visual as LineDecoration;
            //    RenderVisual(drawingContext, visual, new Rect(0, 0, Math.Max(line.Rectangle.Width, line.Thickness), Math.Max(line.Rectangle.Height, line.Thickness)));
            //}
        }

        protected override void OnRefresh()
        {
            LineDecoration lineDecoration = Visual as LineDecoration;
            if (lineDecoration != null)
            {
                _pathPen = new Pen(new SolidColorBrush(lineDecoration.LineColor), lineDecoration.Thickness);
                _path = new PathGeometry();

                Point startPoint = new Point(lineDecoration.Start.X - lineDecoration.Left, lineDecoration.Start.Y - lineDecoration.Top);
                Point endPoint = new Point(lineDecoration.End.X - lineDecoration.Left, lineDecoration.End.Y - lineDecoration.Top);

                if (lineDecoration.Curve)
                {
                    // Create a vector representing the direction of the line
                    Vector v1 = lineDecoration.End - lineDecoration.Start;
                    double length = v1.Length;

                    // Normalize so it can be used to construct control points
                    v1.Normalize();

                    // Create a matrix to rotate arm perpendicular
                    Matrix m1 = new Matrix();
                    m1.Rotate(-90);

                    Point curvePoint1 = (startPoint + (v1 * (length * lineDecoration.CurveStart))) + ((v1 * (length * lineDecoration.CurveDepth)) * m1);
                    Point curvePoint2 = (endPoint - (v1 * (length * lineDecoration.CurveStart))) + ((v1 * (length * lineDecoration.CurveDepth)) * m1);

                    //_path.Figures.Add(QuadraticBezierFromIntersection(startPoint, curvePoint1, endPoint));
                    _path.Figures.Add(BezierFromIntersection(startPoint,curvePoint1,curvePoint2,endPoint));
                }
                else
                {
                    LineGeometry line = new LineGeometry(startPoint, endPoint);
                    _path.AddGeometry(line);
                }                
            }
        }

        static void bez3pts1(double x0, double y0, double x3, double y3, double x2, double y2, out double x1, out double y1)
        {
            // find chord lengths
            double c1 = Math.Sqrt((x3 - x0) * (x3 - x0) + (y3 - y0) * (y3 - y0));
            double c2 = Math.Sqrt((x3 - x2) * (x3 - x2) + (y3 - y2) * (y3 - y2));
            // guess "best" t
            double t = c1 / (c1 + c2);
            // quadratic Bezier is B(t) = (1-t)^2*P0 + 2*t*(1-t)*P1 + t^2*P2
            // solving gives P1 = [B(t) - (1-t)^2*P0 - t^2*P2] / [2*t*(1-t)] where P3 is B(t)
            x1 = (x3 - (1 - t) * (1 - t) * x0 - t * t * x2) / (2 * t * (1 - t));
            y1 = (y3 - (1 - t) * (1 - t) * y0 - t * t * y2) / (2 * t * (1 - t));
        }

        // pass in a PathFigure and it will append a QuadraticBezierSegment connecting the previous point to int1 and endPt
        static public PathFigure QuadraticBezierFromIntersection(Point startPt, Point int1, Point endPt)
        {
            double x1, y1;
            bez3pts1(startPt.X, startPt.Y, int1.X, int1.Y, endPt.X, endPt.Y, out x1, out y1);
            PathFigure p = new PathFigure { StartPoint = startPt };
            p.Segments.Add(new QuadraticBezierSegment { Point1 = new Point(x1, y1), Point2 = endPt });
            return p;
        }

        // linear equation solver utility for ai + bj = c and di + ej = f
        static void solvexy(double a, double b, double c, double d, double e, double f, out double i, out double j)
        {
            j = (c - a / d * f) / (b - a * e / d);
            i = (c - (b * j)) / a;
        }

        // basis functions
        static double b0(double t) { return Math.Pow(1 - t, 3); }
        static double b1(double t) { return t * (1 - t) * (1 - t) * 3; }
        static double b2(double t) { return (1 - t) * t * t * 3; }
        static double b3(double t) { return Math.Pow(t, 3); }

        static void bez4pts1(double x0, double y0, double x4, double y4, double x5, double y5, double x3, double y3, out double x1, out double y1, out double x2, out double y2)
        {
            // find chord lengths
            double c1 = Math.Sqrt((x4 - x0) * (x4 - x0) + (y4 - y0) * (y4 - y0));
            double c2 = Math.Sqrt((x5 - x4) * (x5 - x4) + (y5 - y4) * (y5 - y4));
            double c3 = Math.Sqrt((x3 - x5) * (x3 - x5) + (y3 - y5) * (y3 - y5));
            // guess "best" t
            double t1 = c1 / (c1 + c2 + c3);
            double t2 = (c1 + c2) / (c1 + c2 + c3);
            // transform x1 and x2
            solvexy(b1(t1), b2(t1), x4 - (x0 * b0(t1)) - (x3 * b3(t1)), b1(t2), b2(t2), x5 - (x0 * b0(t2)) - (x3 * b3(t2)), out x1, out x2);
            // transform y1 and y2
            solvexy(b1(t1), b2(t1), y4 - (y0 * b0(t1)) - (y3 * b3(t1)), b1(t2), b2(t2), y5 - (y0 * b0(t2)) - (y3 * b3(t2)), out y1, out y2);
        }

        static public PathFigure BezierFromIntersection(Point startPt, Point int1, Point int2, Point endPt)
        {
            double x1, y1, x2, y2;
            bez4pts1(startPt.X, startPt.Y, int1.X, int1.Y, int2.X, int2.Y, endPt.X, endPt.Y, out x1, out y1, out x2, out y2);
            PathFigure p = new PathFigure { StartPoint = startPt };
            p.Segments.Add(new BezierSegment { Point1 = new Point(x1, y1), Point2 = new Point(x2, y2), Point3 = endPt });
            return p;
        }


    }
}
