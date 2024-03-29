﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Game;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Drawing.Image;
using Timer = System.Windows.Forms.Timer;

namespace Game
{
    public partial class MyFrom : Form
    {
        public Timer timer { get; set; }
        public MyFrom()
        {
            KeyPreview= true;
            DoubleBuffered= true;
            Size = new Size(600, 600);
            var map = new Map(Image.FromFile("./textures/grass.jpg"));
            Action<Vector> newThread = map.Initialize;
            newThread.BeginInvoke(new Vector(ClientSize.Width / 2, ClientSize.Height / 2), null, null);
            //map.Initialize();
            var weapon = new Weapon(400, 100, 20, 300);
            var physicForWeapon = new Physics(weapon, new Vector(4, 0), 0.5, 10);
            weapon.Physic= physicForWeapon;
           

            var player = new Player(map, weapon, 0.2)
            {
                Location = new Vector(ClientSize.Width / 2, ClientSize.Height / 2),
                Velocity = new Vector(),
                CurrentTexture = Image.FromFile("./textures/character.png")

            };
            map.Object[Tuple.Create((int)player.Location.X, (int)player.Location.Y)] = new Object() { HP = ((int)player.HP + 20.00) * 3 };
            player.StateChanged += () =>
            {
                this.Invalidate();
            };

            var inventory = new Inventory(player);
            player.PlayerInventory = inventory;
            ViewControllers.AddDrawMap(map, this);
            ViewControllers.AddDrawMapEnviroment(map, this, player);
            ViewControllers.AddDrawPlayer(player, this);
            ViewControllers.AddProcessedTextures(player, this);
            this.SizeChanged += (s, e) =>
            {
                ViewControllers.CountNewYOffset(player, this);
                ViewControllers.CountNewXOffset(player, this);
                Invalidate();
            };
            var server = new Server(map);
            var playerInterface = new Interface(
                ViewControllers.ProccessedElements.Heart, 
                this, 
                player,
                server
                );
            ServerController.ConnectedServer = server;
        }
    }
}
