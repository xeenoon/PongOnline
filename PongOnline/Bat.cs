using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PongOnline
{
    public enum MovementType
    {
        WASD,
        ARROWS,
        REMOTE
    }
    public enum Direction
    {
        UP,
        DOWN,
        NONE
    }
    public class Bat
    {
        public const int WIDTH = 10;
        public const int HEIGHT = 50;

        public static readonly Brush BRUSH = Brushes.Green;
        public Rectangle graphics;
        Point location;

        Canvas canvas;
        MovementType movementType;
        public Direction direction = Direction.NONE;

        Key up;
        Key down;

        public Bat(Canvas c, Point location, Timer mainTimer, MovementType movementType)
        {
            this.canvas = c;
            graphics = new Rectangle()
            {
                Width = WIDTH,
                Height = HEIGHT,
                Fill = BRUSH,
                Stroke = BRUSH,
                StrokeThickness = 2,
            };

            // Add to a canvas for example
            canvas.Children.Add(graphics);
            Canvas.SetTop(graphics, location.Y);
            Canvas.SetLeft(graphics, location.X);
            this.location = location;

            this.movementType = movementType;
            switch (movementType)
            {
                case MovementType.WASD:
                    up = Key.W;
                    down = Key.S;
                    break;
                case MovementType.ARROWS:
                    up = Key.Up;
                    down = Key.Down;
                    break;
                case MovementType.REMOTE:
                    break;
            }
            mainTimer.Elapsed += new ElapsedEventHandler(Tick);
        }

        // Given three collinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        bool onSegment(Point p, Point q, Point r)
        {
            if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Max(p.X, r.X) &&
                q.Y <= Math.Max(p.Y, r.Y) && q.X >= Math.Max(p.Y, r.Y))
                return true;

            return false;
        }
        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are collinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        int orientation(Point p, Point q, Point r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/
            // for details of below formula.
            double val = (q.Y - p.Y) * (r.X - q.X) -
                      (q.X - p.X) * (r.Y - q.Y);

            if (val == 0) return 0;  // collinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        bool doIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = orientation(p1, q1, p2);
            int o2 = orientation(p1, q1, q2);
            int o3 = orientation(p2, q2, p1);
            int o4 = orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are collinear and p2 lies on segment p1q1
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are collinear and q2 lies on segment p1q1
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are collinear and p1 lies on segment p2q2
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are collinear and q1 lies on segment p2q2
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        internal bool Intersects(Point from, Point to)
        {
            Point top = new Point(location.X + WIDTH / 2, location.Y);
            Point bottom = new Point(location.X + WIDTH / 2, location.Y + HEIGHT);


            return doIntersect(from,to,top,bottom);
        }

        internal bool Intersects(Ball ball)
        {
            double right = location.X + WIDTH;
            double left = location.X;

            double top = location.Y + HEIGHT;
            double bottom = location.Y;

            bool isLeftPaddle = left < 300;
            if (isLeftPaddle && right > ball.location.X && right - ball.location.X < WIDTH * 2 * Math.Abs(ball.direction.velocity.X)) //Our right side intersects with balls left side
            {
                if (top >= ball.location.Y && bottom <= ball.location.Y) //Ball has same y val as us
                {
                    return true;
                }
            }

            if (!isLeftPaddle && left < ball.location.X + Ball.WIDTH && left - ball.location.X - Ball.WIDTH < WIDTH * 2 * Math.Abs(ball.direction.velocity.X)) //Our left side intersects with balls right side
            {
                if (top >= ball.location.Y && bottom <= ball.location.Y) //Ball has same y val as us
                {
                    return true;
                }
            }

            return false;
        }
        public void Tick(object sender, ElapsedEventArgs e)
        {
            switch (direction)
            {
                case Direction.UP:
                    location.Y-=5;
                    ModifyLocation(location.Y);
                    SendNewMovement(location.Y);
                    break;
                case Direction.DOWN:
                    location.Y+=5;
                    ModifyLocation(location.Y);
                    SendNewMovement(location.Y);
                    break;
                case Direction.NONE:
                    break;
            }
        }

        private void SendNewMovement(double y)
        {
            string tosend = string.Format("1:{0}", (int)y);
            Networking.SendToAll(Encoding.ASCII.GetBytes(tosend));
        }

        public void ModifyLocation(double ypos)
        {
            if (ypos <= 0)
            {
                ypos = 0;
            }
            if (ypos >= 435)
            {
                ypos = 435;
            }
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                Canvas.SetTop(graphics, ypos);
            }));
        }
    }
}
