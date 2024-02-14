using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class ServerController
    {
        public static Server ConnectedServer { get; set; }
        public static void StateChanged(Map.Objects gameObj, Vector newLoc, Vector oldLoc)
        {
            ConnectedServer?.SendInfo(new MoveInfo(Map.Objects.Player,
                                    newLoc, oldLoc));
        }
        public static void StateChanged(Vector loc, Object objName)
        {
            ConnectedServer?.SendInfo(new HpInfo(loc, objName));
        }
    }
}
