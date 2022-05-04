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
        PORT
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
        public const int HEIGHT = 30;

        public static readonly Brush BRUSH = Brushes.Green;
        Rectangle graphics;
        Point location;

        Canvas canvas;
        MovementType movementType;
        Direction direction = Direction.NONE;

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
                case MovementType.PORT:
                    break;
            }
            mainTimer.Elapsed += new ElapsedEventHandler(Tick);
        }

        public void HandleKeyDown(Key key)
        {
            if (key == up)
            {
                direction = Direction.UP;

            }
            else if (key == down)
            {
                direction = Direction.DOWN;
            }
        }
        public void HandleKeyUp(Key key) //Consistency to have both methods accept a key
        {
            direction = Direction.NONE;
        }
        public void Tick(object sender, ElapsedEventArgs e)
        {
            switch (direction)
            {
                case Direction.UP:
                    location.Y-=2;
                    ModifyLocation(location.Y);
                    break;
                case Direction.DOWN:
                    location.Y+=2;
                    ModifyLocation(location.Y);
                    break;
                case Direction.NONE:
                    break;
            }
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
