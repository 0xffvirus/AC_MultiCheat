using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Swed32;

namespace AcHackMenu
{
    public class methods
    {
        public Swed mem;
        public IntPtr moduleBase;

        public Entity ReadLocalPlayer()
        {
            var localplayer = ReadEntity(mem.ReadPointer(moduleBase, Offsets.iLocalPlayer));
            localplayer.viewAngles.X = mem.ReadFloat(localplayer.baseAddress, Offsets.vAngles);
            localplayer.viewAngles.Y = mem.ReadFloat(localplayer.baseAddress + Offsets.vAngles + 0x4);

            return localplayer;

        }


        public Entity ReadEntity(IntPtr entBase)
        {
            var ent = new Entity(); // connect to Entity class in "Data.cs"
            ent.baseAddress = entBase; 
            ent.currentAmmo = mem.ReadInt(ent.baseAddress, Offsets.iCurrentAmmo); // reading current ammo value from player in game using base address + offset
            ent.health = mem.ReadInt(ent.baseAddress, Offsets.iHealth);
            ent.team = mem.ReadInt(ent.baseAddress, Offsets.iteam);
            ent.feet = mem.ReadVec(ent.baseAddress, Offsets.vFeet); // reading vector feet from player in game using base address + offset
            ent.head = mem.ReadVec(ent.baseAddress, Offsets.vHead);
            ent.name = Encoding.UTF8.GetString(mem.ReadBytes(ent.baseAddress, Offsets.sname, 11)); // reading player name in bytes then convert it to string

            return ent;       
        }



        public List<Entity> ReadEntities(Entity localplayer)
        {
            var entities = new List<Entity>();
            var entityList = mem.ReadPointer(moduleBase, Offsets.iEntityList);
            var nop = ReadNumofPlayers(mem.ReadPointer(moduleBase, Offsets.iNumOfPlayersAdd + 0)); // read number of players in game from the pointer ( with offset 0 )
            for (int i = 0; i < nop - 1; i++)
            {
                var currentEntBase = mem.ReadPointer(entityList, i * 0x4);
                var ent = ReadEntity(currentEntBase);
                ent.mag = MagCalc(localplayer, ent);

                if (ent.health > 0 && ent.health < 101)
                    entities.Add(ent);
            }

            return entities;
        }

        public int ReadNumofPlayers(IntPtr nopBase) 
        {
            return mem.ReadInt(nopBase, 0); // return the number of players in game

        }

        public Vector2 CalcAngls(Entity localplayer, Entity destEnt)
        {
            float x, y;
            var DeltaX = destEnt.head.X - localplayer.head.X;
            var DeltaY = destEnt.head.Y - localplayer.head.Y;

            x = (float)(Math.Atan2(DeltaY, DeltaX) * 180 / Math.PI) + 90; // get the angles using Tan then convert it from radians to angles

            float DeltaZ = destEnt.head.Z - localplayer.head.Z;
            float dist = CalcDistince(localplayer, destEnt);

            y = (float)(Math.Atan2(DeltaZ, dist) * 180 / Math.PI);

            return new Vector2(x, y);
        }

        public void Aim(Entity ent, float x, float y)
        {
            mem.WriteFloat(ent.baseAddress, Offsets.vAngles, x);
            mem.WriteFloat(ent.baseAddress, Offsets.vAngles + 0x4, y);

        }

        public static float CalcDistince(Entity localplayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.feet.X - localplayer.feet.X, 2) + Math.Pow(destEnt.feet.Y - localplayer.feet.Y, 2));
        }


        public static float MagCalc(Entity localplayer, Entity destEnt)
        {
            return (float)
                Math.Sqrt(Math.Pow(destEnt.feet.X - localplayer.feet.X, 2) + Math.Pow(destEnt.feet.Y - localplayer.feet.Y, 2) + Math.Pow(destEnt.feet.Z - localplayer.feet.Z, 2));

        }

        public Rectangle CalcRect(Point feet, Point head)
        {
            var rect = new Rectangle();
            rect.X = head.X - (feet.Y - head.Y) / 4;
            rect.Y = head.Y;

            rect.Width = (feet.Y - head.Y) / 2;
            rect.Height = feet.Y - head.Y;

            return rect;
        }

        public Point WorldToScreen(viewMatrix mtx, Vector3 pos, int width, int height)
        {
            var twoD = new Point();
            var screenW = (mtx.m14 * pos.X) + (mtx.m24 * pos.Y) + (mtx.m34 * pos.Z) + mtx.m44;

            if (screenW > 0.001f)
            {
                var screenX = (mtx.m11 * pos.X) + (mtx.m21 * pos.Y) + (mtx.m31 * pos.Z) + mtx.m41;
                var screenY = (mtx.m12 * pos.X) + (mtx.m22 * pos.Y) + (mtx.m32 * pos.Z) + mtx.m42;

                var camX = width / 2f;
                var camY = height / 2f;

                float X = camX + (camX * screenX / screenW);
                float Y = camY - (camY * screenY / screenW);

                twoD.X = (int)X;
                twoD.Y = (int)Y;

                return twoD;
            } else
            {
                return new Point(-99, -99);
            }


        }

        

        public viewMatrix ReadMatrix()
        {
            var viewMatrix = new viewMatrix();
            var mtx = mem.ReadMatrix(moduleBase + Offsets.iViewMatrix);

            viewMatrix.m11 = mtx[0];
            viewMatrix.m12 = mtx[1];
            viewMatrix.m13 = mtx[2];
            viewMatrix.m14 = mtx[3];

            viewMatrix.m21 = mtx[4];
            viewMatrix.m22 = mtx[5];
            viewMatrix.m23 = mtx[6];
            viewMatrix.m24 = mtx[7];

            viewMatrix.m31 = mtx[8];
            viewMatrix.m32 = mtx[9];
            viewMatrix.m33 = mtx[10];
            viewMatrix.m34 = mtx[11];

            viewMatrix.m41 = mtx[12];
            viewMatrix.m42 = mtx[13];
            viewMatrix.m43 = mtx[14];
            viewMatrix.m44 = mtx[15];

            return viewMatrix;
        }
 

        public methods() {
            mem = new Swed("ac_client");
            moduleBase = mem.GetModuleBase(".exe");
        }


    }
}
