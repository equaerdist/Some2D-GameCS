using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class UserController
    {
        public static DateTime LastPressTime = DateTime.Now;
        public static void AddManagingKeys(MyFrom window, Player player)
        {
            window.KeyDown += (s, e) =>
            {
                if ((DateTime.Now - LastPressTime).TotalMilliseconds > 100)
                {
                    LastPressTime = DateTime.Now;
                    Tools.Axe temp = Tools.Axe.Y;
                    switch (e.KeyCode)
                    {
                        case System.Windows.Forms.Keys.W:
                            if (player.Velocity.Y == 0)
                            {
                                player.inAction = true;
                                player.Velocity = new Vector(player.Velocity.X, -2.5);
                                temp = Tools.Axe.Y;
                                Action<Tools.Axe, MyFrom> temporaryW = player.DoStep;
                                temporaryW.BeginInvoke(temp, window, null, null);
                            }
                            break;
                        case System.Windows.Forms.Keys.S:
                            if (player.Velocity.Y == 0)
                            {
                                player.inAction = true;
                                player.Velocity = new Vector(player.Velocity.X, 2.5);
                                temp = Tools.Axe.Y;
                                Action<Tools.Axe, MyFrom> temporaryS = player.DoStep;
                                temporaryS.BeginInvoke(temp, window, null, null);
                            }
                            break;
                        case System.Windows.Forms.Keys.A:
                            if (player.Velocity.X == 0)
                            {
                                player.inAction = true;
                                player.Velocity = new Vector(-2.5, player.Velocity.Y);
                                player.CurrentDirection = Player.Direction.Left;
                                temp = Tools.Axe.X;
                                Action<Tools.Axe, MyFrom> temporaryA = player.DoStep;
                                temporaryA.BeginInvoke(temp, window, null, null);
                            }
                            break;
                        case System.Windows.Forms.Keys.D:
                            if (player.Velocity.X == 0)
                            {
                                player.inAction = true;
                                player.Velocity = new Vector(2.5, player.Velocity.Y);
                                player.CurrentDirection = Player.Direction.Right;
                                temp = Tools.Axe.X;
                                Action<Tools.Axe, MyFrom> temporaryD = player.DoStep;
                                temporaryD.BeginInvoke(temp, window, null, null);
                            }
                            break;
                    }
                }
            };
            window.KeyDown += (s, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.ShiftKey)
                {
                    if (player.SpeedMode == Player.Mode.Slow)
                    {
                        player.inAction = true;
                        player.SpeedMode = Player.Mode.Fast;
                    }
                }
                if (e.KeyCode == System.Windows.Forms.Keys.Space)
                {
                    if ((DateTime.Now - player.CurrentWeapon.ShotLastTime).TotalMilliseconds >=
                    player.CurrentWeapon.TimeBetweenShotsInMilliseconds)
                    {
                        Action<MyFrom> b = player.MakeShoot;
                        player.CurrentWeapon.ShotLastTime = DateTime.Now;
                        b.BeginInvoke(window, null, null);
                    }
                  }
            };
            window.KeyDown += (s, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Space)
                {
                    if ((DateTime.Now - player.CurrentWeapon.ShotLastTime).TotalMilliseconds >=
                    player.CurrentWeapon.TimeBetweenShotsInMilliseconds)
                    {
                        Action<MyFrom> b = player.MakeShoot;
                        b.BeginInvoke(window, null, null);
                    }
                }
            };
            window.KeyUp += (s, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.ShiftKey) player.SpeedMode = Player.Mode.Slow;
                switch (e.KeyCode)
                {
                    case System.Windows.Forms.Keys.W:
                        player.inAction = false;
                        player.Velocity = new Vector(player.Velocity.X, 0);
                        break;
                    case System.Windows.Forms.Keys.S:
                        player.inAction = false;
                        player.Velocity = new Vector(player.Velocity.X, 0);
                        break;
                    case System.Windows.Forms.Keys.A:
                        player.inAction = false;
                        player.Velocity = new Vector(0, player.Velocity.Y);
                        break;
                    case System.Windows.Forms.Keys.D:
                        player.inAction = false;
                        player.Velocity = new Vector(0, player.Velocity.Y);
                        break;
                }
                //window.Text = player.Location.ToString();
            };
        }
    }
}
