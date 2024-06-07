using Swed32;
using System.Runtime.InteropServices;
using System.Threading;
using ezOverLay;
using System.Diagnostics;
namespace AcHackMenu
{
    public partial class Form1 : Form
    {
        ez ez = new ez();
        bool Aimbot;
        bool infiniammo;
        bool esp;

        Form2 EspForm = new Form2();
        methods? m;
        Entity localplayer = new Entity();
        List<Entity> entities = new List<Entity>();

        [DllImport("user32.dll")]

        static extern short GetAsyncKeyState(Keys vKey);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            CheckForIllegalCrossThreadCalls = false;
            m = new methods();
            if (m != null)
            {
                Thread thread = new Thread(()=>main(EspForm)) { IsBackground = true };
                thread.Start();
            }
            EspForm.Show();

        }



        void main(Form2 espFormWindow)
        {
            IntPtr ammoAddress = m.moduleBase + 0xC73EF;
            while (true)
            {
                localplayer = m.ReadLocalPlayer();
                entities = m.ReadEntities(localplayer);

                entities = entities.OrderBy(o => o.mag).ToList();
                if (GetAsyncKeyState(Keys.XButton2) < 0 && Aimbot == true)
                {
                    if (entities.Count > 0)
                    {
                        foreach (var ent in entities)
                        {
                            if (ent.team != localplayer.team)
                            {
                                var angles = m.CalcAngls(localplayer, ent);
                                m.Aim(localplayer, angles.X, angles.Y);
                                break;

                            }
                        }
                    }
                }

                if (infiniammo == true)
                {
                    m.mem.WriteBytes(ammoAddress, "90 90");
                }
                else
                {
                    m.mem.WriteBytes(ammoAddress, "FF 08");
                }
                if (esp == true)
                {
                    espFormWindow.Show();
                    espFormWindow.Refresh();
                } else
                {
                    espFormWindow.Hide();
                }
                label4.Text = localplayer.health.ToString();


                Thread.Sleep(20);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Aimbot = !Aimbot;

        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            infiniammo = !infiniammo;
            Debug.WriteLine("Checked");

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            esp = !esp;
            Debug.WriteLine("Checked");
        }
    }
}
