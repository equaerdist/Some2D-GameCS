using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Map
    {
        public readonly Dictionary<Tuple<int, int>, Object> Object = new Dictionary<Tuple<int, int>, Object> { };
        public enum Objects { Grass, Player, Tree, VioletStone, BrownStone, GreyStone, Bullet}
        public byte[,] GameArea { get; private set; }
        public Image MapTexture { get; set; }
        public Map( Image texture)
        {
            MapTexture = texture;
        }
        public void StateChanged(Location coordinates)
        {
            if(GameArea != null)
            {
                try
                {
                    GameArea[(int)coordinates.Item1.Y, (int)coordinates.Item1.X] = (byte)Objects.Grass;
                    GameArea[(int)coordinates.Item2.Y, (int)coordinates.Item2.X] = (byte)Objects.Player;
                }
                catch { return; }
            }
        }
        public void Initialize()
        {
            var r = new Random();
            GameArea = new byte[1800, 1800];
            for (int i = 0; i < GameArea.GetLength(0); i++)
            {
                for (int j = 0; j < GameArea.GetLength(1); j++)
                {
                    if (i % 275 == 0 && j % 435 == 0) { GameArea[i, j] = (byte)r.Next(2, 6); Object[Tuple.Create(i, j)] = new Object() { HP = 100 }; }
                }
            }
        }
    }
}
