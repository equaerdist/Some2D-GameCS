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
        public IDictionary<Map.Objects, Map.Objects[]> DroppedItems { get; }
        public readonly Dictionary<Tuple<int, int>, Object> Object = new Dictionary<Tuple<int, int>, Object> { };
        public enum Objects { Grass, Player, Tree, VioletStone, BrownStone, GreyStone, Bullet, Wood, Diamond}
        public byte[,] GameArea { get; private set; }
        public Image MapTexture { get; set; }
        public Map( Image texture)
        {
            MapTexture = texture;
            DroppedItems = new Dictionary<Map.Objects, Map.Objects[]>()
            {
                {Map.Objects.Tree, new[]{Map.Objects.Wood} },
                {Map.Objects.VioletStone, new[]{Map.Objects.Diamond} },
            };
        }
        public void SpawnRandomDrop(Tuple<int, int> coord)
        {
            if (DroppedItems.ContainsKey((Map.Objects)GameArea[coord.Item1, coord.Item2])) {
                var r = new Random();
                var destroyedObject = (Map.Objects)GameArea[coord.Item1, coord.Item2];
                GameArea[coord.Item1, coord.Item2] = (byte)DroppedItems[destroyedObject][r.Next(0, DroppedItems[destroyedObject].Length)];
            }
            else
            {
                GameArea[coord.Item1, coord.Item2] = (byte)Map.Objects.Grass;
            }
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
            GameArea = new byte[3600, 3600];
            for (int i = 0; i < GameArea.GetLength(0); i++)
            {
                for (int j = 0; j < GameArea.GetLength(1); j++)
                {
                    if (i % 300 ==0 && j % 400 == 0 && j != 0 && i != 0) 
                    { GameArea[i, j] = (byte)r.Next(2, 6); Object[Tuple.Create(i, j)] = new Object() { HP = 100 }; }
                    else if( i % 350 == 0 && j % 455 == 0 && j != 0 && i != 0)
                    { GameArea[i, j] = (byte)r.Next(2, 6); Object[Tuple.Create(i, j)] = new Object() { HP = 100 }; }
                    else if (i % 10 == 0 && j % 20 == 0 && j != 0 && i != 0 && j / 100 < 0 && i / 200 < 0)
                    { GameArea[i, j] = (byte)r.Next(2, 6); Object[Tuple.Create(i, j)] = new Object() { HP = 100 }; }
                }
            }
        }
    }
}
