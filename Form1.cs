using System;
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
            var map = new Map(Image.FromFile("./textures/grass.jpg"));
            Action newThread = map.Initialize;
            newThread.BeginInvoke(null, null);
            //map.Initialize();
            var weapon = new Weapon(400, 200, 20, 300);
            var physicForWeapon = new Physics(weapon, new Vector(4, 0), 0.5, 10);
            weapon.Physic= physicForWeapon;
            var player = new Player(map, weapon)
            {
                Location = new Vector(ClientSize.Width / 2, ClientSize.Height / 2),
                Velocity = new Vector(),
                CurrentTexture = Image.FromFile("./textures/character.png")
            };

           
            player.StateChanged += () =>
            {
                this.Invalidate();
            };
            ViewControllers.AddDrawMap(map, this);
            ViewControllers.AddDrawMapEnviroment(map, this, player);
            ViewControllers.AddDrawPlayer(player, this);
            UserController.AddManagingKeys(this, player);
            ViewControllers.AddProcessedTextures(player, this);
        }
    }
}
