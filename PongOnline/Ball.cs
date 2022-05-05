using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PongOnline
{
    public class Ball
    {
        public const int WIDTH = 10;
        public const int HEIGHT = 10;

        public static readonly Brush BRUSH = Brushes.Red;
        public Point location;
        public VelocityVector direction;

        public Ellipse graphics;

        Canvas canvas;

        public Bat player1;
        public Bat player2;

        public Ball(Canvas c, Point location, Timer maintimer, VelocityVector direction, Bat player1, Bat player2)
        {
            this.canvas = c;
            graphics = new Ellipse()
            {
                Width = WIDTH,
                Height = HEIGHT,
                Fill = BRUSH,
                Stroke = BRUSH,
                StrokeThickness = 2,
            };

            canvas.Children.Add(graphics);
            Canvas.SetTop(graphics, location.Y);
            Canvas.SetLeft(graphics, location.X);
            this.location = location;
            this.direction = direction;

            this.player1 = player1;
            this.player2 = player2;

            maintimer.Elapsed += new ElapsedEventHandler(Tick);
        }
        const int DEFAULT_LOCATION = 350;
        public void ModifyLocation(double ypos, double xpos)
        {
            if (xpos <= 0)
            {
                xpos = DEFAULT_LOCATION;
                ypos = DEFAULT_LOCATION;
                location = new Point(DEFAULT_LOCATION, DEFAULT_LOCATION);
                direction.location = new Point(DEFAULT_LOCATION, DEFAULT_LOCATION);
                direction.velocity = Velocities.Random();
            }
            if (xpos >= 700)
            {
                xpos = DEFAULT_LOCATION;
                ypos = DEFAULT_LOCATION;
                location = new Point(DEFAULT_LOCATION,DEFAULT_LOCATION);
                direction.location = new Point(DEFAULT_LOCATION,DEFAULT_LOCATION);
                direction.velocity = Velocities.Random();
            }

            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                Canvas.SetTop(graphics, ypos);
                Canvas.SetLeft(graphics, xpos);
            }));
        }
        public void Tick(object sender, ElapsedEventArgs e)
        {
            direction.UpdateLocation();

            Point from = new Point(direction.velocity.X > 0 ? this.location.X + WIDTH : this.location.X, this.location.Y > direction.location.Y ? this.location.Y + HEIGHT : this.location.Y); 
            Point to = new Point ( direction.velocity.X > 0 ? direction.location.X + WIDTH : direction.location.X, this.location.Y > direction.location.Y ? this.location.Y : direction.location.Y + HEIGHT );
            
            if (to.Y <= 0 || to.Y >= 455)
            {
                direction.BounceVertical();
            }
            else if (player1.Intersects(from, to) || player2.Intersects(from, to))
            {
                direction.BounceHorizontal();
            }

            location = direction.location;
            ModifyLocation(location.Y, location.X);
        }
    }
    public class VelocityVector
    {
        public Point location;
        public Velocity velocity;

        public VelocityVector(Point location, Velocity velocity)
        {
            this.location = location;
            this.velocity = velocity;
            multiplyTimeout.Elapsed += new ElapsedEventHandler(TimerTick);
        }

        public void UpdateLocation()
        {
            location = new Point(location.X + velocity.X, location.Y + velocity.Y);
        }
        Timer multiplyTimeout = new Timer(1000);
        public void TimerTick(object sender, ElapsedEventArgs e)
        {
            canMultiply = true;
            multiplyTimeout.Stop();
        }
        bool canMultiply = true;
        public void BounceHorizontal()
        {
            velocity.X *= -1;
            if (canMultiply) 
            {
                velocity *= 1.1f;
            }
            canMultiply = false;
            multiplyTimeout.Start();
        }
        public void BounceVertical()
        {
            velocity.Y *= -1;
            if (canMultiply)
            {
                velocity *= 1.1f;
            }
            canMultiply = false;
            multiplyTimeout.Start();
        }
    }
    public class Velocity
    {
        public float X;
        public float Y;

        public Velocity(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Velocity operator *(Velocity v, float m)
        {
            return new Velocity(v.X*m,v.Y*m);
        }
        public static Velocity operator /(Velocity v, float m)
        {
            return new Velocity(v.X / m, v.Y / m);
        }
    }
    public class Velocities
    {
        public static readonly Velocity TopRight = new Velocity(1.5f, 1.5f);
        public static readonly Velocity BottomRight = new Velocity(1.5f, -1.5f);
        public static readonly Velocity TopLeft = new Velocity(-1.5f, 1.5f);
        public static readonly Velocity BottomLeft = new Velocity(-1.5f, -1.5f);

        static Random r = new Random();
        public static Velocity Random()
        {
            switch (r.Next(0,4))
            {
                case 0:
                    return TopRight;
                case 1:
                    return BottomRight;
                case 2:
                    return TopLeft;
                case 3:
                    return BottomLeft;
                default:
                    throw new Exception();
            }
        }
    }
}
