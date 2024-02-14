using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;

namespace Game
{
    public interface IInterfaceState
    {
         bool CanIManage();
    }
    class GameState :IInterfaceState
    {
        public bool CanIManage() { return true; }
    }
    public class Interface
    {
        private DateTime LastPressWASDTime = DateTime.Now;
        private DateTime LastPressInventoryTime = DateTime.Now;
        private DateTime LastCallInterface = DateTime.Now;
        private DateTime LastCallScale = DateTime.Now;
        public IInterfaceState State;
        public readonly MyFrom Window;
        public readonly Player Player;
        public readonly Server Server;
        public Label Menu;
        public bool MenuVisible;

        public bool IsVisible { get; private set; } = false;
        public readonly IDictionary<string, PaintEventHandler> Functions = new Dictionary<string, PaintEventHandler>();
        private void AddPlayerManagingKeys()
        {

            Window.KeyDown += (s, e) =>
            {
                if (State.CanIManage())
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
                }
            };
            Window.KeyDown += (s, e) =>
            {
                if (State.CanIManage())
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
                }
            };
            Window.KeyDown += (s, e) =>
            {
                if (State.CanIManage())
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
                }
            };


            Window.KeyUp += (s, e) =>
            {
                if (State.CanIManage())
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
                }
                //window.Text = player.Location.ToString();
            };
        }
        private void CallMenu()
        {
            if (MenuVisible) { MenuVisible = false; Window.Controls.Remove(Menu); Menu?.Dispose(); }
            else
            {
                MenuVisible = true;
                var label = new Label()
                {
                    Size = new Size(Window.Width / 2, Window.Height / 2),
                    Location = new Point(Window.Width / 4, Window.Height / 4)
                };
                var table = new TableLayoutPanel() { Dock = DockStyle.Fill };
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                for (int i = 0; i < 2; i++)
                {
                    table.RowStyles.Add(new RowStyle(SizeType.Absolute, label.Height / 3));
                }
                Menu = label;
                var button = new Button() { Dock = DockStyle.Fill, Text = "Играть по сети" ,
                    FlatStyle = FlatStyle.Flat, BackColor = Color.PeachPuff};
                button.Click += (s, e) =>
                {
                    MenuVisible = false; Window.Controls.Remove(Menu); Menu?.Dispose();
                    var loadScreen = new Label() { Width = Window.Width, Height = Window.Height };
                    loadScreen.BackColor = Color.Khaki;
                    loadScreen.Text = "Подождите, пытаемся соединиться с сервером...";
                    loadScreen.Font = new Font("Times New Roman", 25, FontStyle.Bold);
                    Window.Controls.Add(loadScreen);
                    Player.IsOnline = true;
                    Player.CurrentMap.IsOnline = true;
                    loadScreen.BringToFront();
                    var task = new Task((async () =>
                    {
                        var text = await Server.Initialize(new InitializeInfo(Player.CurrentMap, Player));
                        Window.BeginInvoke(new Action(() => loadScreen.Text = text));
                        await Task.Delay(3000);
                        Window.BeginInvoke(new Action(() => { Window.Controls.Remove(loadScreen); loadScreen.Dispose(); }));
                    }));
                    task.Start();
                };
                if (Player.IsOnline) button.Enabled = false;
                table.Controls.Add(button, 0, 0);
                label.Controls.Add(table);
                label.BackColor = Color.LightSkyBlue;
                Window.Controls.Add(label);
                label.BringToFront();
            }
        }
        private void AddGameInterfacepManagingKeys()
        {
            Window.KeyDown += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.N:
                            if ((DateTime.Now - LastPressInventoryTime).TotalMilliseconds > 300)
                            {
                                LastPressInventoryTime = DateTime.Now;
                                CallMenu();
                            }
                        break;
                }

            };
           Window.KeyDown += (s, e) =>
            {
                if (State.CanIManage())
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
                }
            };
        }
        private void HideOrShow()
        {
            if (State.CanIManage())
            {
                if (IsVisible)
                {
                    IsVisible = false;
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
        }
        public Interface(ViewControllers.ProccessedElements HpTexture, MyFrom window, Player Player, Server server)
        {
            this.State = new GameState();
            this.Window = window;
            this.Player = Player;
            Server = server;
            Functions["ShowPlayerHp"] = (sender, args) =>
            {
                var g = args.Graphics;
                g.FillRectangle(Brushes.Black, 10, 10, 100, 15);
                g.FillRectangle(Brushes.Red, 10, 10, (float) (100 * Player.HP / 100), 15);
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
