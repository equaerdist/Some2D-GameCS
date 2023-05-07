using Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class Inventory
    {
        public class Comparator : IComparer<Tuple<int, Map.Objects>>
        {
            public int Compare(Tuple<int, Map.Objects> x, Tuple<int, Map.Objects> y)
            {
                return x.Item2.CompareTo(y.Item2);
            }
        }
        private List<Tuple<int, Map.Objects>> objects = new List<Tuple<int, Map.Objects>>();
        public Inventory(Player player)
        {
            Owner= player;
        }
        public int Count { get { return objects.Count; } }
        public Player Owner { get; private set; }
        public void Add(Tuple<int, Map.Objects> objectItem)
        {
            for(int i=0; i <objects.Count; i++)
            {
                if (objects[i].Item2 == objectItem.Item2) 
                    objects[i] = Tuple.Create(objects[i].Item1 + objectItem.Item1, objectItem.Item2);
            }
            objects.Sort(new Comparator());
        }
        public void Remove(Tuple<int, Map.Objects> objectItem)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].Item2 == objectItem.Item2)
                {
                    if (objects[i].Item1 > objectItem.Item1)
                        objects[i] = Tuple.Create(objects[i].Item1 - objectItem.Item1, objects[i].Item2);
                    else
                        objects.RemoveAt(i);
                }
            }
            objects.Sort(new Comparator());
        }
    }
}
