using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public abstract class Info
    {
        public abstract string Type { get; set; }
    }
    public class InitializeInfo :Info
    {
        public override string Type { get; set; } = "initializeiNFO";
        public Map Map { get; set; }
        public bool Initialized { get; set; }
        public Player[] Players;
        public InitializeInfo(Map map, Player player)
        {
            Map = map;
            this.Players = new[] { player };
        }
    }
    public class MoveInfo :Info
    {
        public override string Type { get; set; } = "MoveiNFO";
        public Map.Objects Object { get; set; }
        public Vector NewLocation { get; set; }
        public Vector OldLocation { get; set; }
        public MoveInfo(Map.Objects @object, Vector newLocation, Vector oldLocation)
        {
            Object = @object;
            NewLocation = newLocation;
            OldLocation = oldLocation;
        }
    }
    public class HpInfo :Info
    {
        public override string Type { get; set; } = "HpINFO";
        public Vector Location { get; set; }
        public Object objectName { get; set; }
        public HpInfo(Vector location, Object objectName)
        {
            Location = location;
            this.objectName = objectName;
        }
    }
}
