using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ezOverLay;

namespace AcHackMenu
{
    public partial class Form2 : Form
    {

        ez ez = new ez();
        methods? m;
        Entity localplayer = new Entity();
        List<Entity> entities = new List<Entity>();

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            m = new methods();
            if (m != null)
            {
                Thread thread = new Thread(main) { IsBackground = true };
                thread.Start();
            }
            ez.SetInvi(this);
            ez.DoStuff("AssaultCube", this);

        }

        void main()
        {
            while (true) { 
            localplayer = m.ReadLocalPlayer();
            entities = m.ReadEntities(localplayer);

            entities = entities.OrderBy(o => o.mag).ToList();
            }
        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Graphics gg = e.Graphics;
            Pen red = new Pen(Color.Red, 3);
            Pen green = new Pen(Color.Green, 3);

            foreach (var ent in entities.ToList())
            {
                var wtsFeet = m.WorldToScreen(m.ReadMatrix(), ent.feet, this.Width, this.Height);
                var wtsHead = m.WorldToScreen(m.ReadMatrix(), ent.head, this.Width, this.Height);

                if (wtsFeet.X > 0)
                {
                    if (localplayer.team == ent.team)
                    {
                        gg.DrawLine(green, new Point(Width / 2, Height), wtsFeet);
                        gg.DrawRectangle(green, m.CalcRect(wtsFeet, wtsHead));
                    } else
                    {
                        gg.DrawLine(red, new Point(Width / 2, Height), wtsFeet);
                        gg.DrawRectangle(red, m.CalcRect(wtsFeet, wtsHead));
                    }
                }
            }
        }
    }
}
