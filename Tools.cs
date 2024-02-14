using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class Tools
    {
        public enum Axe { Y, X};
        public static (int startX, int startY, int  endX, int endY) CountVisibleFar(Player player, MyFrom window, Map map)
        {
            var currentLocation = player.Location;
            var length = map.GameArea.GetLength(0);
            var startX = currentLocation.X - window.Width / ViewControllers.ScaleCoefficent > 0 ?
               currentLocation.X - window.Width / ViewControllers.ScaleCoefficent : 0;

            var startY = currentLocation.Y - window.Height / ViewControllers.ScaleCoefficent > 0 ?
            currentLocation.Y - window.Height / ViewControllers.ScaleCoefficent : 0;

            var endX = currentLocation.X + window.Width / ViewControllers.ScaleCoefficent >= length ? length  :
            currentLocation.X + window.Width / ViewControllers.ScaleCoefficent;

            var endY = currentLocation.Y + window.Height / ViewControllers.ScaleCoefficent >= length ? length  :
            currentLocation.Y + window.Height / ViewControllers.ScaleCoefficent;
            return ((int)startX, (int)startY, (int)endX, (int)endY);
        }
    }
}
