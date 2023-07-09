using Game;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        public int Width
        {
            get { if (Table != null) return Table.Width; else return -1; }
        }
        public int Height
        {
            get { if (Table != null) return Table.Height; else return -1; }
        }
        public Inventory(Player player)
        {
            Owner= player;
            AmountItemsOnPage = 16;
        }
        private TableLayoutPanel Table;
        public int Count { get { return objects.Count; } }
        [JsonIgnore]
        public Player Owner { get; private set; }
        public int AmountItemsOnPage { get; }
        public bool IsShowed { get; private set; } = false;
        public void Show(MyFrom window)
        {
            if (Table == null)
            {
                InitializeInventory(window);
            }
            Table.Size = new Size(window.Width / 4, window.Height / 2);
            Table.Location = new Point(window.Width - Width - 10, (int)(window.Height - 1.5 * Height));
            if (IsShowed) { window.Controls.Remove(Table); IsShowed = false; Table.Controls.Clear(); }
            else
            {
                FillInventory();
                window.Controls.Add(Table);
                IsShowed = true;
            }
        }
        private void FillInventory()
        {
            var counter = 0;
            var row = 0;
            var i = 0;
            while (row != 4)
            {
                var button = new Button() { Dock = DockStyle.Fill };
                button.FlatAppearance.BorderSize = 0;
                button.FlatStyle = FlatStyle.Flat;
                button.BackColor = Color.LightGreen;
                if (i < objects.Count)
                {
                    var j = i;
                    button.Paint += (s, e) =>
                    {
                        var g = e.Graphics;
                        g.DrawImage(ViewControllers.Models[objects[j].Item2], 0, 0, button.Width, button.Height);
                        g.DrawString(objects[j].Item1.ToString(), new Font("Times New Roman", 25, FontStyle.Bold), Brushes.Red,
                            new RectangleF(0, 0, button.Width, button.Height),
                            new StringFormat
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center,
                                FormatFlags = StringFormatFlags.FitBlackBox
                            });
                    };
                }
                Table.Controls.Add(button, counter, row);
                counter++;
                i++;
                if (counter == 3) { counter = 0; row++; }
            }
        }
        private void InitializeInventory(MyFrom window)
        {
            Table = new TableLayoutPanel();
            Table.BackColor = Color.White;
            for (int i = 0; i < 4; i++)
            {
                Table.RowStyles.Add(new RowStyle(SizeType.Percent, 25f));
                Table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            }
            Table.AutoScroll = true;
            Table.GrowStyle = TableLayoutPanelGrowStyle.AddRows;
            
        }
        public void Add(Tuple<int, Map.Objects> objectItem)
        {
            if (objects.Count > 0)
            {
                bool find = false;
                for (int i = 0; i < objects.Count; i++)
                {
                    if (objects[i].Item2 == objectItem.Item2)
                    { objects[i] = Tuple.Create(objects[i].Item1 + objectItem.Item1, objectItem.Item2); find = true; }
                }
                if (!find) objects.Add(objectItem);
            }
            else { objects.Add(objectItem); }
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
