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
        public readonly MyFrom Window;
        public readonly Player Player;
        public bool IsVisible { get; private set; } = false;
        public readonly IDictionary<string, PaintEventHandler> Functions = new Dictionary<string, PaintEventHandler>();
        public void HideOrShow()
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
                var offset = 10 * Window.Width / 600;
                for (int i = -1; i < (int)Player.HP; i++)
                {
                    g.DrawImage(ViewControllers.ProcessedTextures[HpTexture], offset,
                         10 * Window.Size.Height / 600, 20 * Window.Width / 600, 20 * Window.Height / 600);
                    offset += 20 * Window.Width / 600;
                }
            };
            Functions["ShowPlayerEnergy"] = (sender, args) =>
            {
                var g = args.Graphics;
               g.FillRectangle(Brushes.Black, 10 * Window.Width / 600, 30 * window.Height / 600, 100 * window.Width/ 600,
                   15 * window.Height / 600);
                g.FillRectangle(Brushes.Blue, 10 * Window.Width / 600, 
                    30 * window.Height / 600, 100 * window.Width / 600 * Player.Energy / 100,
                   15 * window.Height / 600);
            };
        }
    }
}
