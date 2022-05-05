using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PongOnline
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class PongGameWindow : Window
    {
        public static PongGameWindow Singleton;

        Timer t = new Timer();
        Bat player1;
        Bat player2;

        Ball ball;
        public PongGameWindow()
        {
            InitializeComponent();
            t.Interval = 2;
            this.PreviewKeyDown += new KeyEventHandler(HandleKeyDown);
            this.PreviewKeyUp   += new KeyEventHandler(HandleKeyUp);

            player1 = new Bat(MainCanvas,new Point(50,50), t, MovementType.WASD);
            player2 = new Bat(MainCanvas,new Point(630,50), t, MovementType.ARROWS);

            ball = new Ball(MainCanvas,new Point(300,300), t, new VelocityVector(new Point(300,300), Velocities.Random()), player1, player2);

            Singleton = this;
            t.Start();
        }
        public bool isKeyDown;
        public void HandleKeyDown(object sender, KeyEventArgs e)
        {
         //   if (!isKeyDown)
         //   {
         //       isKeyDown = true;
                player1.HandleKeyDown(e.Key);
                player2.HandleKeyDown(e.Key);
        //    }
        }
        public void HandleKeyUp(object sender, KeyEventArgs e)
        {
       //     if (isKeyDown)
         //   {
                player1.HandleKeyUp(e.Key);
                player2.HandleKeyUp(e.Key);
                isKeyDown = !isKeyDown;
           // }
        }
    }
}
