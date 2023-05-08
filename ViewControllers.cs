using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game
{
    public static class ViewControllers
    {
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
                g.FillRectangle(Brushes.Black, (coord.Item2 - (int)player.AbsoluteAmountXOffset) * window.Width / 600, 
                    (coord.Item1 - (int)player.AbsoluteAmountYOffset + 10    ) * window.Height / 600, 50 * window.Width / 600, 
                    10 * window.Height / 600);
                lock (map.Object)
                {
                    if(map.Object.ContainsKey(coord) && (int)map.Object[coord].HP != 0)
                        g.FillRectangle(Brushes.Red, (coord.Item2 - (int)player.AbsoluteAmountXOffset) * window.Width / 600,
                            (coord.Item1 - (int)player.AbsoluteAmountYOffset + 10) * window.Height / 600,
                            50 * window.Width / 600 * (int)map.Object[coord].HP / 100, 10 * window.Height / 600);
                }
            };
            window.Paint += temp;
            var timer = new Timer() { Interval = 500 };
            timer.Tick += (s, e) =>
            {
                window.Paint -= temp;
                window.Invalidate();
                timer.Stop();
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
            var timer = new Timer() { Interval = 500 };

            var timeVariable = (int)Math.Sin(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            timer.Tick += (s, e) =>
            {
                timeVariable = (int)Math.Sin(timeVariable + 500);
            };
            timer.Start();
            window.Paint += (s, e) =>
            {
                var length = map.GameArea.GetLength(0);
                var g = e.Graphics;
                var startX = player.Location.X - 600 > 0 ? player.Location.X - 600 : 0;
                var startY = player.Location.Y - 600 > 0 ? player.Location.Y - 600 : 0;
                var endX = player.Location.X + 600 >= length ? length - 1 : player.Location.X + 600;
                var endY = player.Location.Y + 600 >= length ? length - 1 : player.Location.Y + 600;
                for (int i = (int)startY; i < (int)endY; i++)
                {
                    for (int j = (int)startX; j < (int)endX; j++)
                    {
                        if (Models.ContainsKey((Map.Objects)map.GameArea[i, j]) &&
                        (Map.Objects)map.GameArea[i, j] != Map.Objects.Grass &&
                        (Map.Objects)map.GameArea[i, j] != Map.Objects.Player
                        && (Map.Objects)map.GameArea[i, j] != Map.Objects.Wood &&
                         (Map.Objects)map.GameArea[i, j] != Map.Objects.Diamond)
                        {
                            g.FillEllipse(Brushes.Black, (int)(j - (float)player.AbsoluteAmountXOffset)
                                * window.Size.Width / 600,
                                 (int)(i - (float)player.AbsoluteAmountYOffset) * window.Size.Height / 600 +
                                 50 * window.Size.Height / 550, 44 * window.Size.Width / 600,
                                 10);
                            g.DrawImage(Models[(Map.Objects)map.GameArea[i, j]],
                                        (int)(j - (float)player.AbsoluteAmountXOffset) * window.Size.Width / 600,
                                       (int)(i - (float)player.AbsoluteAmountYOffset) * window.Size.Height / 600,
                                        50 * window.Size.Width / 700, 50 * window.Size.Height / 500);
                        }
                        else if ((Map.Objects)map.GameArea[i, j] == Map.Objects.Bullet)
                        {
                            var trackerSize = 10;
                            g.DrawLine(new Pen(Brushes.Red, 5 * window.Width / 600), (int)(j - (float)player.AbsoluteAmountXOffset) * window.Size.Width / 600,
                                  (int)(i - (float)player.AbsoluteAmountYOffset) * window.Size.Height / 600,
                                  (int)(j - (float)player.AbsoluteAmountXOffset + trackerSize) * window.Size.Width / 600,
                                  (int)(i - (float)player.AbsoluteAmountYOffset) * window.Size.Height / 600
                                  );
                        }
                        else if (Models.ContainsKey((Map.Objects)map.GameArea[i, j]) &&
                        (Map.Objects)map.GameArea[i, j] != Map.Objects.Grass &&
                        (Map.Objects)map.GameArea[i, j] != Map.Objects.Player)
                        {
                            g.DrawImage(Models[(Map.Objects)map.GameArea[i, j]],
                                       (int)(j - (float)player.AbsoluteAmountXOffset) * window.Size.Width / 600,
                                      (int)(i - (float)player.AbsoluteAmountYOffset) * window.Size.Height / 600,
                                       (60 + 500 * timeVariable) * window.Size.Width / 700,
                                           (50 + 20 * timeVariable) * window.Size.Height / 500);
                        }
                    }
                }
            };
        }
        public static void AddDrawMap(Map map, MyFrom window)
        {
            Bitmap grassTexture = new Bitmap(100 * window.Width / 600, 100 * window.Height / 600);
            using (var graph = Graphics.FromImage(grassTexture))
            {
                graph.DrawImage(Image.FromFile("./textures/grass.jpg"), 
                    new Rectangle(0, 0, 100 * window.Width / 600, 100 * window.Height / 600));
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
                            g.DrawImage(grassTexture, j * grassTexture.Size.Height,
                           i * grassTexture.Size.Width);
                    }
                }
            };
        }
        public static void AddDrawPlayer(Player player, MyFrom window)
        {
            window.Paint += (sender, args) =>
            {
                    var playerTexture = new Bitmap(50 * window.Size.Width / 700, 50 * window.Size.Height / 500);
                    using (var graph = Graphics.FromImage(playerTexture))
                    {
                    lock (player.CurrentTexture)
                    {
                        graph.DrawImage(player.CurrentTexture, 0, 0, 50 * window.Size.Width / 700, 50 * window.Size.Height / 500);
                    }
                    }
                    var g = args.Graphics;
                    g.DrawImage(playerTexture, ((float)player.Location.X  - (float)player.AbsoluteAmountXOffset)
                        * window.Size.Width / 600,
                         ((float)player.Location.Y -(float)player.AbsoluteAmountYOffset) * window.Size.Height / 600);
                //window.Text = $" x is {player.Location} and y is {player.AbsoluteAmountXOffset}";
            };
        }
    }
}
