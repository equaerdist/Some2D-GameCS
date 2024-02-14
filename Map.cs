using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Map
    {
        public IDictionary<Map.Objects, Map.Objects[]> DroppedItems { get; }
        public Dictionary<Tuple<int, int>, Object> Object = new Dictionary<Tuple<int, int>, Object> { };
        public enum Objects { Grass, Player, Tree, VioletStone, BrownStone, GreyStone, Bullet, Wood, Diamond}
        public byte[,] GameArea { get; private set; }
        public bool IsOnline { get; set; }
        public Image MapTexture { get; set; }
        public Map( Image texture)
        {
            MapTexture = texture;
            DroppedItems = new Dictionary<Map.Objects, Map.Objects[]>()
            {
                {Map.Objects.Tree, new[]{Map.Objects.Wood} },
                {Map.Objects.VioletStone, new[]{Map.Objects.Diamond} },
            };
            Object[Tuple.Create(300, 300)] = new Object() { HP = 100 };
        }
        public void SetNewLocationFromNet(MoveInfo info)
        {
            GameArea[(int)info.OldLocation.Y, (int)info.OldLocation.X] = (byte)Map.Objects.Grass;
            GameArea[(int)info.NewLocation.Y, (int)info.NewLocation.X] = (byte)info.Object;
            if (Object.ContainsKey(Tuple.Create((int)info.OldLocation.Y, (int)info.OldLocation.X))) {
                var hp = Object[Tuple.Create((int)info.OldLocation.Y, (int)info.OldLocation.X)].HP;
                Object.Remove(Tuple.Create((int)info.OldLocation.Y, (int)info.OldLocation.X));
                Object[Tuple.Create((int)info.NewLocation.Y, (int)info.NewLocation.X)] = new Object()
                { HP = hp };
            }
            else if(!Object.ContainsKey(Tuple.Create((int)info.OldLocation.Y, (int)info.OldLocation.X))
                && info.Object == Map.Objects.Player)
            {
                Object[Tuple.Create((int)info.NewLocation.Y, (int)info.NewLocation.X)] = new Object()
                { HP = 100 };
            }
        }
        public void SetNewHpFromNet(HpInfo info)
        {
            if (GameArea[(int)info.Location.Y, (int)info.Location.X] != (byte)Map.Objects.Grass)
            {
                if (Object.ContainsKey(Tuple.Create((int)info.Location.Y, (int)info.Location.X)))
                {
                    if (info.objectName.HP == 0)
                    {
                        Object.Remove(Tuple.Create((int)info.Location.Y, (int)info.Location.X));
                    }
                    else
                    {
                        Object[Tuple.Create((int)info.Location.Y, (int)info.Location.X)] = new Object
                        { HP = info.objectName.HP };
                    }
                }
            }
        }
        public void SetNewPosition(Vector oldLocation, Vector newLocation, Map.Objects obj)
        {
            GameArea[(int)oldLocation.Y, (int)oldLocation.X] = (byte)Map.Objects.Grass;
            GameArea[(int)newLocation.Y, (int)newLocation.X] = (byte)obj;
            if (IsOnline)
            {
                ServerController.StateChanged(obj, newLocation, oldLocation);
            }
        }
        public bool DifferHp(Tuple<int, int> location, Object newHp)
        {
            if (newHp.HP == 0) { Object.Remove(location);
            if(IsOnline)
                {
                    ServerController.StateChanged(
                        new Vector(x: location.Item2, y: location.Item1), newHp);
                }
                SpawnRandomDrop(location); return false; }
            else { Object[location] = newHp;
                if (IsOnline)
                {
                    ServerController.StateChanged(
                        new Vector(x: location.Item2, y: location.Item1), newHp);
                }
                return true; }
            
        }
        public void SpawnRandomDrop(Tuple<int, int> coord)
        {
            if (DroppedItems.ContainsKey((Map.Objects)GameArea[coord.Item1, coord.Item2])) {
                var r = new Random();
                var destroyedObject = (Map.Objects)GameArea[coord.Item1, coord.Item2];
                var spawn = DroppedItems[destroyedObject][r.Next(0, DroppedItems[destroyedObject].Length)];
                GameArea[coord.Item1, coord.Item2] = (byte)spawn;
                if(IsOnline)
                    ServerController.StateChanged(spawn, new Vector(x: coord.Item2, y: coord.Item1)
                   , new Vector());
            }
            else
            {
                GameArea[coord.Item1, coord.Item2] = (byte)Map.Objects.Grass;
                if(IsOnline)
                    ServerController.StateChanged(Map.Objects.Grass, new Vector(x: coord.Item2, y: coord.Item1)
                    , new Vector());
            }
        }
        public void AddNewPlayer(Player player)
        {
            GameArea[(int)player.Location.Y, (int)player.Location.X] =(byte)Map.Objects.Player;
            Object[Tuple.Create((int)player.Location.Y, (int)player.Location.X)] = new Object()
            { HP = player.HP };
        }
        public void MultiplyMap(Map map)
        {
            GameArea = map.GameArea;
            Object = map.Object;
        }
        public void StateChanged(Location coordinates)
        {
            if(GameArea != null)
            {
                try
                {
                    GameArea[(int)coordinates.Item1.Y, (int)coordinates.Item1.X] = (byte)Objects.Grass;
                    GameArea[(int)coordinates.Item2.Y, (int)coordinates.Item2.X] = (byte)Objects.Player;
                    if (IsOnline)
                    {
                        ServerController.StateChanged(Map.Objects.Player, coordinates.Item2, coordinates.Item1);
                    }
                }
                catch { return; }
            }
        }
        public void Initialize(Vector playerPos)
        {
            Object[Tuple.Create((int)playerPos.Y, (int)playerPos.X)] = new Object() { HP = 100 };
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
