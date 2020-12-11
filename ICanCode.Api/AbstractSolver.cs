using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Web;
using WebSocketSharp;

namespace ICanCode.Api
{
    public abstract class AbstractSolver
    {
        public event Action<Board, string> BoardUpdated;
        protected int goldCollected;
        protected int turnCounter = 0;
        private const string ResponsePrefix = "board=";
        protected int boxesMoved = 6;
        public Board Board { get; set; }
        public Board FullBoard { get; set; }
        protected string prevState = null;
        public Board PrevBoard = new Board(20);
        public Board PrevFullBoard = new Board(Constants.FullBoardSize);
        BinaryFormatter formatter = new BinaryFormatter();
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="server">server http address including email and code</param>
        public AbstractSolver(string server)
        {
            // Console.OutputEncoding = Encoding.UTF8;
            ServerUrl = server;
            if (Directory.Exists(Constants.CacheFolder))
            {
                var sortedFiles = new DirectoryInfo(Constants.CacheFolder).GetFiles()
                    .OrderBy(f => f.LastWriteTime)
                    .Reverse()
                    .ToList();
                if (sortedFiles.Any())
                {
                    using (FileStream fs = new FileStream(sortedFiles.First().FullName, FileMode.OpenOrCreate))
                    {
                        FullBoard = (Board)formatter.Deserialize(fs);
                    }
                }
            }
            if (FullBoard == null)
            {
                FullBoard = new Board(Constants.FullBoardSize);
            }
            
        }

        public string ServerUrl { get; private set; }


        /// <summary>
        /// Set this property to true to finish playing
        /// </summary>
        public bool ShouldExit { get; protected set; }

        public void Play()
        {
            string url = GetWebSocketUrl(this.ServerUrl);

            var socket = new WebSocket(url);
            socket.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;

            socket.OnMessage += Socket_OnMessage;
            socket.Connect();

            while (!ShouldExit && socket.ReadyState != WebSocketState.Closed)
            {
                Thread.Sleep(50);
            }
        }
        private void Socket_OnMessage(object sender, MessageEventArgs e)
        {
            if (!ShouldExit)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                var response = e.Data;

                if (!response.StartsWith(ResponsePrefix))
                {
                    Console.WriteLine("Something strange is happening on the server... Response:\n{0}", response);
                    ShouldExit = true;
                }
                else
                {
                    var boardString = response.Substring(ResponsePrefix.Length);
                    var board = new Board(boardString);
                    Board = board;
                    if (FullBoard.CurrentLevel == 0)
                    {
                        FullBoard.CurrentLevel = board.CurrentLevel;
                    }

                    if (FullBoard.CurrentLevel != board.CurrentLevel)
                    {
                        FullBoard = new Board(Constants.FullBoardSize);
                        if (Directory.Exists(Constants.CacheFolder))
                        {
                            var levelCacheFile = $"{Constants.CacheFolder}/{board.CurrentLevel}.dat";
                            if(File.Exists(levelCacheFile))
                            {
                                using (FileStream fs = new FileStream(levelCacheFile, FileMode.OpenOrCreate))
                                {
                                    FullBoard = (Board)formatter.Deserialize(fs);
                                }
                            }
                        }
                        FullBoard.CurrentLevel = board.CurrentLevel;
                    }
                    for (int layer = 0; layer < FullBoard.Field.GetLength(0); layer++)
                    {
                        for (int x = 0; x < FullBoard.Size; x++)
                        {
                            for (int y = 0; y < FullBoard.Size; y++)
                            {
                                if (FullBoard.IsAt(layer, x, y,
                                    Element.ROBO_OTHER, Element.ROBO_OTHER_FLYING, Element.ROBO_OTHER_LASER, Element.ROBO_OTHER_FALLING,
                                    Element.LASER_DOWN, Element.LASER_UP, Element.LASER_LEFT, Element.LASER_RIGHT, Element.ZOMBIE_DIE, Element.FEMALE_ZOMBIE, Element.MALE_ZOMBIE,
                                    Element.AIM, Element.PATH, Element.GOLD))
                                {
                                    FullBoard.Field[layer,x,y] = (char)Element.EMPTY;
                                }
                            }
                        }
                    }
                    for (int layer = 0; layer < board.Field.GetLength(0);layer++)
                    {
                        for (int x = 0; x < board.Size; x++)
                        {
                            for (int y = 0; y < board.Size; y++)
                            {
                                var absolutePoint = new Point(x, y).RelativeToAbsolute(board.Size, board.Offset);
                                FullBoard.Field[layer, absolutePoint.X, absolutePoint.Y] = board.Field[layer, x, y];
                            }
                        }
                    }

                    if (board.HeroPosition.Y <= 1)
                    {
                        for (int x = 0; x < board.Size; x++)
                        {
                            var absolutePoint = new Point(x, -1).RelativeToAbsolute(board.Size, board.Offset);
                            FullBoard.Field[0, absolutePoint.X, absolutePoint.Y] = (char)Element.WALL_FRONT;
                        }
                    }

                    if (board.HeroPosition.X >= board.Size-2)
                    {
                        for (int y = 0; y < board.Size; y++)
                        {
                            var absolutePoint = new Point(board.Size, y).RelativeToAbsolute(board.Size, board.Offset);
                            FullBoard.Field[0, absolutePoint.X, absolutePoint.Y] = (char)Element.WALL_FRONT;
                        }
                    }
                    FullBoard.HeroPosition = board.HeroPosition.RelativeToAbsolute(board.Size, board.Offset);

                    if (!Directory.Exists(Constants.CacheFolder))
                    {
                        Directory.CreateDirectory(Constants.CacheFolder);
                    }
                    
                    using (FileStream fs = new FileStream($"Cache/{FullBoard.CurrentLevel}.dat", FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, FullBoard);
                    }
                    //Just print current state (gameBoard) to console
                    /*Console.Clear();
                    Console.SetCursorPosition(0, 0);*/
                    var action = "";
                    var time = "";
                    try
                    {
                        turnCounter++;
                        action = WhatToDo(board).ToString();
                        time = $"Execution Time: {watch.ElapsedMilliseconds} ms";
                    }
                    catch { }
                    if (action.Contains("ACT(3)"))
                    {
                        Static.TurnsWithoutFire = 0;
                        Static.PerkCooldownDeathRay = 0;
                        Static.PerkCooldownUnstopableLaser = 0;
                    }
                    else
                    {
                        Static.TurnsWithoutFire++;
                    }
                    var str = new StringBuilder();
                    
                    str.AppendLine(time);
                    str.AppendLine("Answer: " + action);
                    str.AppendLine("Gold collected: " + goldCollected);
                    str.AppendLine("Turns without fire: " + Static.TurnsWithoutFire);
                    str.AppendLine("DeathRay Perk Cooldown: " + Static.PerkCooldownDeathRay);
                    str.AppendLine("Unlimited Fire Perk Cooldown: " + Static.PerkCooldownUnlimitedFire);
                    str.AppendLine("Unstopable Laser Perk Cooldown: " + Static.PerkCooldownUnstopableLaser);
                    str.AppendLine(board.ToString());
                    str.AppendLine(FullBoard.ToString());
                    if (PrevBoard != null)
                        str.AppendLine(PrevFullBoard.ToString());
                    if (!action.IsNullOrEmpty())
                    {
                        prevState = str.ToString();
                    }

                    for (int layer = 0; layer < board.Field.GetLength(0); layer++)
                    {
                        for (int x = 0; x < board.Size; x++)
                        {
                            for (int y = 0; y < board.Size; y++)
                            {
                                PrevBoard.Field[layer, x, y] = board.Field[layer, x, y];
                            }
                        }
                    }

                    for (int layer = 0; layer < FullBoard.Field.GetLength(0); layer++)
                    {
                        for (int x = 0; x < FullBoard.Size; x++)
                        {
                            for (int y = 0; y < FullBoard.Size; y++)
                            {
                                PrevFullBoard.Field[layer, x, y] = FullBoard.Field[layer, x, y];
                            }
                        }
                    }

                    /*Console.WriteLine(str.ToString());
                    Console.SetCursorPosition(0, 0);*/
                    ((WebSocket)sender).Send(action);
                    BoardUpdated?.Invoke(board, str.ToString());
                    
                }
            }
        }

        public static string GetWebSocketUrl(string serverUrl)
        {
            Uri uri = new Uri(serverUrl);

            var server = $"{uri.Host}:{uri.Port}";
            var userName = uri.Segments.Last();
            var code = HttpUtility.ParseQueryString(uri.Query).Get("code");

            return GetWebSocketUrl(userName, code, server);
        }

        private static string GetWebSocketUrl(string userName, string code, string server)
        {
            return string.Format("wss://{0}/codenjoy-contest/ws?user={1}&code={2}",
                            server,
                            Uri.EscapeDataString(userName),
                            code);
        }

        public abstract Command WhatToDo(Board board);

        /// <summary>
        /// Starts client shutdown.
        /// </summary>
        public void InitiateExit()
        {
            ShouldExit = true;
        }
    }
}
