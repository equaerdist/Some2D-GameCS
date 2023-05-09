using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Game
{
    public class Interface
    {
        private  DateTime LastPressWASDTime = DateTime.Now;
        private  DateTime LastPressInventoryTime = DateTime.Now;
        private  DateTime LastCallInterface = DateTime.Now;
        private  DateTime LastCallScale = DateTime.Now;
        public readonly MyFrom Window;
        public readonly Player Player;
        public bool IsVisible { get; private set; } = false;
        public readonly IDictionary<string, PaintEventHandler> Functions = new Dictionary<string, PaintEventHandler>();
        private  void AddPlayerManagingKeys()
        {
            Window.KeyDown += (s, e) =>
            {
                if ((DateTime.Now - LastPressWASDTime).TotalMilliseconds > 100)
                {
                    LastPressWASDTime = DateTime.Now;
                    Tools.Axe temp = Tools.Axe.Y;
                    switch (e.KeyCode)
                    {
                        case System.Windows.Forms.Keys.W:
                            if (Player.Velocity.Y == 0)
                            {
                                Player.inAction = true;
                                Player.Velocity = new Vector(Player.Velocity.X, -2.5);
                                temp = Tools.Axe.Y;
                                Action<Tools.Axe, MyFrom> temporaryW = Player.DoStep;
                                temporaryW.BeginInvoke(temp, Window, null, null);
                            }
                            break;
                        case System.Windows.Forms.Keys.S:
                            if (Player.Velocity.Y == 0)
                            {
                                Player.inAction = true;
                                Player.Velocity = new Vector(Player.Velocity.X, 2.5);
                                temp = Tools.Axe.Y;
                                Action<Tools.Axe, MyFrom> temporaryS = Player.DoStep;
                                temporaryS.BeginInvoke(temp, Window, null, null);
                            }
                            break;
                        case System.Windows.Forms.Keys.A:
                            if (Player.Velocity.X == 0)
                            {
                                Player.inAction = true;
                                Player.Velocity = new Vector(-2.5, Player.Velocity.Y);
                                Player.CurrentDirection = Player.Direction.Left;
                                temp = Tools.Axe.X;
                                Action<Tools.Axe, MyFrom> temporaryA = Player.DoStep;
                                temporaryA.BeginInvoke(temp, Window, null, null);
                            }
                            break;
                        case System.Windows.Forms.Keys.D:
                            if (Player.Velocity.X == 0)
                            {
                                Player.inAction = true;
                                Player.Velocity = new Vector(2.5, Player.Velocity.Y);
                                Player.CurrentDirection = Player.Direction.Right;
                                temp = Tools.Axe.X;
                                Action<Tools.Axe, MyFrom> temporaryD = Player.DoStep;
                                temporaryD.BeginInvoke(temp, Window, null, null);
                            }
                            break;
                    }
                }
            };
            Window.KeyDown += (s, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.ShiftKey)
                {
                    if (Player.SpeedMode == Player.Mode.Slow && Player.Energy > 0)
                    {
                        Player.inAction = true;
                        Player.SpeedMode = Player.Mode.Fast;
                    }
                }
                if (e.KeyCode == System.Windows.Forms.Keys.Space)
                {
                    if ((DateTime.Now - Player.CurrentWeapon.ShotLastTime).TotalMilliseconds >=
                    Player.CurrentWeapon.TimeBetweenShotsInMilliseconds)
                    {
                        Action<MyFrom> b = Player.MakeShoot;
                        Player.CurrentWeapon.ShotLastTime = DateTime.Now;
                        b.BeginInvoke(Window, null, null);
                    }
                }
            };
            Window.KeyDown += (s, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.Space)
                {
                    if ((DateTime.Now - Player.CurrentWeapon.ShotLastTime).TotalMilliseconds >=
                    Player.CurrentWeapon.TimeBetweenShotsInMilliseconds)
                    {
                        Action<MyFrom> b = Player.MakeShoot;
                        b.BeginInvoke(Window, null, null);
                    }
                }
                else if (e.KeyCode == System.Windows.Forms.Keys.Up && e.Control)
                {
                    if ((DateTime.Now - LastCallScale).TotalMilliseconds >=
                   300)
                    {
                        LastCallScale = DateTime.Now;
                        ViewControllers.ScaleCoefficent += 1;
                    }
                }
            };


            Window.KeyUp += (s, e) =>
            {
                if (e.KeyCode == System.Windows.Forms.Keys.ShiftKey) Player.SpeedMode = Player.Mode.Slow;
                else if (e.KeyCode == System.Windows.Forms.Keys.Down && e.Control)
                {
                    if ((DateTime.Now - LastCallScale).TotalMilliseconds >=
                   300)
                    {
                        LastCallScale = DateTime.Now;
                        ViewControllers.ScaleCoefficent -= 1;
                    }
                }
                switch (e.KeyCode)
                {
                    case System.Windows.Forms.Keys.W:
                        Player.inAction = false;
                        Player.Velocity = new Vector(Player.Velocity.X, 0);
                        break;
                    case System.Windows.Forms.Keys.S:
                        Player.inAction = false;
                        Player.Velocity = new Vector(Player.Velocity.X, 0);
                        break;
                    case System.Windows.Forms.Keys.A:
                        Player.inAction = false;
                        Player.Velocity = new Vector(0, Player.Velocity.Y);
                        break;
                    case System.Windows.Forms.Keys.D:
                        Player.inAction = false;
                        Player.Velocity = new Vector(0, Player.Velocity.Y);
                        break;
                }
                //window.Text = player.Location.ToString();
            };
        }
        private  void AddGameInterfacepManagingKeys()
        {
           Window.KeyDown += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case System.Windows.Forms.Keys.I:
                        if ((DateTime.Now - LastPressInventoryTime).TotalMilliseconds > 300)
                        {
                            LastPressInventoryTime = DateTime.Now;
                           Player.PlayerInventory.Show(Window);
                        }
                        break;
                    case System.Windows.Forms.Keys.F10:
                        if ((DateTime.Now - LastCallInterface).Milliseconds > 300)
                        {
                            HideOrShow();
                            LastCallInterface = DateTime.Now;
                        }
                        break;
                }
            };
        }
        private void HideOrShow()
        {
            if (IsVisible) { 
                IsVisible= false;
                foreach (var key in Functions)
                {
                    Window.Paint -= key.Value;
                }
                Window.Invalidate();
            }
            else
            {
                IsVisible = true;
                foreach (var key in Functions)
                {
                    Window.Paint += key.Value;
                }
                Window.Invalidate();
            }
        }
        public Interface(ViewControllers.ProccessedElements HpTexture, MyFrom window, Player Player)
        {
            this.Window = window;
            this.Player = Player;
            Functions["ShowPlayerHp"] = (sender, args) =>
            {
                var g = args.Graphics;
                var offset = 10;
                for (int i = -1; i < (int)Player.HP; i++)
                {
                    g.DrawImage(ViewControllers.ProcessedTextures[HpTexture], offset,
                         10 , 20 , 20);
                    offset += 20;
                }
            };
            Functions["ShowPlayerEnergy"] = (sender, args) =>
            {
                var g = args.Graphics;
               g.FillRectangle(Brushes.Black, 10, 30, 100,15);
               g.FillRectangle(Brushes.Blue, 10, 30, 100 * Player.Energy / 100,15);
            };
            AddGameInterfacepManagingKeys();
            AddPlayerManagingKeys();
            HideOrShow();

        }
    }
}
