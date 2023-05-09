using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Game
{
    public static class ViewControllers
    {
        private readonly static int ceilToPixel = 1;
        private readonly static int defaultWindowWidth = 600;
        private readonly static int defaultWindowHeight = 600;
        private static double offsetForDrop = Math.Sin(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
        private static Timer UpdateDropOffset;
        static ViewControllers()
        {
            UpdateDropOffset = new Timer() { Interval = 1 };
            UpdateDropOffset.Tick += (s, e) =>
            { offsetForDrop = Math.Sin(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond); };
            UpdateDropOffset.Start();
        }
        public static void CountNewXOffset(Player player, MyFrom window)
        {
            if (player.Location.X - 200 - player.AbsoluteAmountXOffset < 0 ||
                            player.Location.X + 200 - player.AbsoluteAmountXOffset >= window.Width / ViewControllers.ScaleCoefficent)
                player.AbsoluteAmountXOffset = player.Location.X + 200 - window.Width / ViewControllers.ScaleCoefficent;
        }
        public static void CountNewYOffset(Player player, MyFrom window)
        {
            if (player.Location.Y - 200 - player.AbsoluteAmountYOffset < 0 ||
                            player.Location.Y - player.AbsoluteAmountYOffset + 200 >= window.Height / ViewControllers.ScaleCoefficent)
                player.AbsoluteAmountYOffset = player.Location.Y + 200 - window.Height / ViewControllers.ScaleCoefficent;
        }
        public static Bitmap RenderOnOtherThread(Map map, MyFrom window, Player player)
        {
            var result = Tools.CountVisibleFar(player, window, map);
            var bitmap = new Bitmap(window.Width, window.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                for (int i = result.startY; i < result.endY; i++)
                {
                    for (int j = result.startX; j < result.endX; j++)
                    {
                        if (Models.ContainsKey((Map.Objects)map.GameArea[i, j])
                        && (Map.Objects)map.GameArea[i, j] != Map.Objects.Grass
                        && (Map.Objects)map.GameArea[i, j] != Map.Objects.Player
                        && (Map.Objects)map.GameArea[i, j] != Map.Objects.Wood
                        && (Map.Objects)map.GameArea[i, j] != Map.Objects.Diamond)
                        {

                            g.FillEllipse(Brushes.Black, (int)(j - (float)player.AbsoluteAmountXOffset) * ScaleCoefficent,
                             (int)(i - (float)player.AbsoluteAmountYOffset + 45) * ScaleCoefficent,
                             50 * ScaleCoefficent,
                             10 * ScaleCoefficent);
                            g.DrawImage(Models[(Map.Objects)map.GameArea[i, j]],
                                        (int)(j - (float)player.AbsoluteAmountXOffset) * ScaleCoefficent,
                                       (int)(i - (float)player.AbsoluteAmountYOffset) * ScaleCoefficent,
                                        50 * ScaleCoefficent,
                                        50 * ScaleCoefficent);

                        }
                        else if ((Map.Objects)map.GameArea[i, j] == Map.Objects.Bullet)
                        {
                            var trackerSize = 14;

                            g.DrawLine(new Pen(Brushes.Red, 4 * ScaleCoefficent),
                              (int)(j - (float)player.AbsoluteAmountXOffset) * ScaleCoefficent,
                                (int)(i - (float)player.AbsoluteAmountYOffset) * ScaleCoefficent,
                                (int)(j - (float)player.AbsoluteAmountXOffset + trackerSize) * ScaleCoefficent,
                                (int)(i - (float)player.AbsoluteAmountYOffset) * ScaleCoefficent);

                        }
                        else if (Models.ContainsKey((Map.Objects)map.GameArea[i, j]) &&
                        (Map.Objects)map.GameArea[i, j] != Map.Objects.Grass &&
                        (Map.Objects)map.GameArea[i, j] != Map.Objects.Player)
                        {

                            g.DrawImage(Models[(Map.Objects)map.GameArea[i, j]],
                                    (int)(j - (float)player.AbsoluteAmountXOffset) * ScaleCoefficent,
                                   (int)(i - (float)player.AbsoluteAmountYOffset) * ScaleCoefficent,
                                    (60 + 2 * (float)offsetForDrop) * ScaleCoefficent,
                                        (50 + 2 * (float)offsetForDrop) * ScaleCoefficent);
                        }
                    }
                }
            }
            return bitmap;
        }
        public static int ScaleCoefficent { get; set; } = 1;
        public  enum ProccessedElements { Heart};
        ///<summary> 
        ///    Метод для добавления текстур предварительно обработав их по размеру
        ///</summary>
        public static readonly Dictionary<ProccessedElements, Bitmap> ProcessedTextures = new Dictionary<ProccessedElements, Bitmap>
        { };
        public static void AddProcessedTextures(Player player, MyFrom window)
        {
            var temp = new Bitmap(20, 20);
            using (var g = Graphics.FromImage(temp))
            {
                g.DrawImage(Image.FromFile("./textures/heart.png"), 0,0,20,20);
            }
            ProcessedTextures[ProccessedElements.Heart] = temp;
        }
        public static readonly Queue<Tuple<PaintEventHandler, Timer>> ActiveTimers = new Queue<Tuple<PaintEventHandler, Timer>>();
        public static void ShowMapObjectsHp(MyFrom window, Player player, Map map, Tuple<int, int> coord)
        {
            PaintEventHandler temp = (s, a) =>
            {
                var g = a.Graphics;
                g.FillRectangle(Brushes.Black, (coord.Item2 - (int)player.AbsoluteAmountXOffset) * ScaleCoefficent, 
                    (coord.Item1 - (int)player.AbsoluteAmountYOffset + 10    ) * ScaleCoefficent, 50 * ScaleCoefficent, 
                    10 * ScaleCoefficent);
                lock (map.Object)
                {
                    if(map.Object.ContainsKey(coord) && (int)map.Object[coord].HP != 0)
                        g.FillRectangle(Brushes.Red, (coord.Item2 - (int)player.AbsoluteAmountXOffset) * ScaleCoefficent,
                            (coord.Item1 - (int)player.AbsoluteAmountYOffset + 10) * ScaleCoefficent,
                            50 * ScaleCoefficent * (int)map.Object[coord].HP / 100, 10 * ScaleCoefficent);
                }
            };
            window.Paint += temp;
            var timer = new Timer() { Interval = 500 };
            timer.Tick += (s, e) =>
            {
                window.Paint -= temp;
                window.Invalidate();
                timer.Dispose();
            };
            if (ActiveTimers.Count > 0) {
                var temporary = ActiveTimers.Dequeue();
                window.Paint -= temporary.Item1;
                temporary.Item2.Dispose();
             }
            ActiveTimers.Enqueue(Tuple.Create(temp, timer));
            timer.Start();
        }
        public static readonly Dictionary<Map.Objects, Image> Models = new Dictionary<Map.Objects, Image>()
        { {Map.Objects.VioletStone, Image.FromFile("./textures/stone1.png")},
            {Map.Objects.BrownStone, Image.FromFile("./textures/stone2.png")},
            {Map.Objects.GreyStone, Image.FromFile("./textures/stone3.png")},
            {Map.Objects.Tree, Image.FromFile("./textures/tree.png")},
            {Map.Objects.Player, Image.FromFile("./textures/character.png") },
            {Map.Objects.Wood, Image.FromFile("./textures/wood.png") },
             {Map.Objects.Diamond, Image.FromFile("./textures/diamond.png") }
        };
        public static void AddDrawMapEnviroment(Map map, MyFrom window, Player player)
        {
            window.Paint +=  (s, e) =>
            {
                var g = e.Graphics;
                var result = RenderOnOtherThread(map, window, player);
                g.DrawImage(result, 0, 0, window.Width, window.Height);
            };
        }
        public static void AddDrawMap(Map map, MyFrom window)
        {
            Bitmap grassTexture = new Bitmap(100, 100);
            using (var graph = Graphics.FromImage(grassTexture))
            {
                graph.DrawImage(Image.FromFile("./textures/grass.jpg"), 
                    new Rectangle(0, 0, 100, 100));
            }
            var amountY = (int)Math.Floor((double)window.Size.Height / grassTexture.Size.Height) + 1;
            var amountX = (int)Math.Floor((double)window.Size.Width / grassTexture.Size.Width) + 1;
            window.SizeChanged += (send, arg) =>
            {
                amountY = (int)Math.Floor((double)window.Size.Height / grassTexture.Size.Height) + 1;
                amountX = (int)Math.Floor((double)window.Size.Width / grassTexture.Size.Width) + 1;
            };
            window.Paint += (sender, args) =>
            {
                var g = args.Graphics;
                for(int i=0;  i< amountY;i++)
                {
                    for(int j=0; j < amountX;j++)
                    {
                            g.DrawImage(grassTexture, j * grassTexture.Size.Width,
                           i * grassTexture.Size.Height);
                    }
                }
            };
        }
        public static void AddDrawPlayer(Player player, MyFrom window)
        {
            window.Paint += (sender, args) =>
            {
                    var playerTexture = new Bitmap(50 * ScaleCoefficent, 50 * ScaleCoefficent);
                    using (var graph = Graphics.FromImage(playerTexture))
                    {
                    lock (player.CurrentTexture)
                    {
                        graph.DrawImage(player.CurrentTexture, 0, 0, 50 * ScaleCoefficent, 50 * ScaleCoefficent);
                    }
                    }
                    var g = args.Graphics;
                    g.DrawImage(playerTexture, ((float)player.Location.X  - (float)player.AbsoluteAmountXOffset)
                        * ScaleCoefficent,
                         ((float)player.Location.Y -(float)player.AbsoluteAmountYOffset) * ScaleCoefficent);
                //window.Text = $" x is {player.Location} and y is {player.AbsoluteAmountXOffset}";
            };
        }
    }
}
