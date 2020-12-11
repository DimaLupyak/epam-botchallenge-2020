using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICanCode.Api;
using ICanCode.Client;

namespace ICanCode.GuiClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string ServerUrl = "https://epam-botchallenge.com/codenjoy-contest/board/player/uda5i8hzlz8khmmvcv1s?code=7996357795976478622";
        YourSolver bot;

        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 20; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition());
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            Start();
        }

        void Start()
        {
            //Console.SetWindowSize(Console.LargestWindowWidth - 3, Console.LargestWindowHeight - 3);

            // creating custom AI client
            
            bot = new YourSolver(ServerUrl);

            // starting thread with playing game
            (new Thread(bot.Play)).Start();

            bot.BoardUpdated += UpdateBoardInvoke;

        }

        private void UpdateBoardInvoke(Board board, string str)
        {
            Dispatcher.BeginInvoke(
                new ThreadStart(() => UpdateBoard(board, str)));
        }


        private void UpdateBoard(Board board, string str)
        {
            text.Text = str;
            var layers = new[] {Layers.LAYER1, Layers.LAYER2, Layers.LAYER3, Layers.LAYER4, Layers.LAYER5};
            grid.Children.Clear();


            foreach (var layer in layers)
            {
                for (int x = 0; x < board.Size; x++)
                {
                    for (int y = 0; y < board.Size;  y++)
                    {
                        var myButton = new Button();
                        myButton.BorderBrush = Brushes.Transparent;
                        myButton.BorderThickness = new Thickness(0);
                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri($"Images/{GetElementName(board.Field[layer, x, y])}.png", UriKind.Relative);
                        bitmap.EndInit();
                        myButton.Background = new ImageBrush(bitmap);
                        Grid.SetRow(myButton, y);
                        Grid.SetColumn(myButton, x);
                        grid.Children.Add(myButton);
                    }
                }
            }
        }

        private string GetElementName(char c)
        {
            switch (c)
            {
                case 'S':
                    return "START";
                case 'E':
                    return "EXIT";
                case '.':
                    return "FLOOR";
                case '-':
                    return "EMPTY";
                case 'Z':
                    return "ZOMBIE_START";
                case 'O':
                    return "HOLE";
                case 'B':
                    return "BOX";
                case '$':
                    return "GOLD";
                case 'l':
                    return "UNSTOPPABLE_LASER_PERK";
                case 'r':
                    return "DEATH_RAY_PERK";
                case 'f':
                    return "UNLIMITED_FIRE_PERK";
                case '☺':
                    return "ROBO";
                case 'o':
                    return "ROBO_FALLING";
                case '*':
                    return "ROBO_FLYING";
                case '☻':
                    return "ROBO_LASER";
                case '♀':
                    return "FEMALE_ZOMBIE";
                case '♂':
                    return "MALE_ZOMBIE";
                case '✝':
                    return "ZOMBIE_DIE";
                case 'X':
                    return "ROBO_OTHER";
                case 'x':
                    return "ROBO_OTHER_FALLING";
                case '^':
                    return "ROBO_OTHER_FLYING";
                case '&':
                    return "ROBO_OTHER_LASER";
                case '˂':
                    return "LASER_MACHINE_CHARGING_LEFT";
                case '˃':
                    return "LASER_MACHINE_CHARGING_RIGHT";
                case '˄':
                    return "LASER_MACHINE_CHARGING_UP";
                case '˅':
                    return "LASER_MACHINE_CHARGING_DOWN";
                case '◄':
                    return "LASER_MACHINE_READY_LEFT";
                case '►':
                    return "LASER_MACHINE_READY_RIGHT";
                case '▲':
                    return "LASER_MACHINE_READY_UP";
                case '▼':
                    return "LASER_MACHINE_READY_DOWN";
                case '←':
                    return "LASER_LEFT";
                case '→':
                    return "LASER_RIGHT";
                case '↑':
                    return "LASER_UP";
                case '↓':
                    return "LASER_DOWN";
                case '╔':
                    return "ANGLE_IN_LEFT";
                case '═':
                    return "WALL_FRONT";
                case '┐':
                    return "ANGLE_IN_RIGHT";
                case '│':
                    return "WALL_RIGHT";
                case '┘':
                    return "ANGLE_BACK_RIGHT";
                case '─':
                    return "WALL_BACK";
                case '└':
                    return "ANGLE_BACK_LEFT";
                case '║':
                    return "WALL_LEFT";
                case '┌':
                    return "WALL_BACK_ANGLE_LEFT";
                case '╗':
                    return "WALL_BACK_ANGLE_RIGHT";
                case '╝':
                    return "ANGLE_OUT_RIGHT";
                case '╚':
                    return "ANGLE_OUT_LEFT";
                case 't':
                    return "PATH";
                case 'a':
                    return "AIM";
                case 'd':
                    return "DANGER";
                case 's':
                    return "NO-STEP";
                case 'j':
                    return "NO-JUMP";
                case '~':
                    return "ROBO_FIRE";
                case '1':
                    return "1";
                case '2':
                    return "2";
                case '3':
                    return "3";
                case '4':
                    return "4";
                case '5':
                    return "5"; 
            }

            return "EMPTY";
        }
    }
}
