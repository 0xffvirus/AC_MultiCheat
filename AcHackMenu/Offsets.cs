using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcHackMenu
{
    public class Offsets
    {
        public static int
            // All these offsets got extracted by Cheat Engine


            iViewMatrix = 0x17DFFC-0x6C  + 0x4*16, // view matrix address
            iLocalPlayer = 0x0018AC00, // local player address
            iEntityList = 0x00191FCC, // entity list address
            iNumOfPlayersAdd = 0x0005C434, // Number of players pointer address

            // offset from entity class


            vHead = 0x4, // player camera/head position
            vFeet = 0x28, // player feet position
            vAngles = 0x34, // player view angels
            iHealth = 0xEC, // player health
            iDead = 0xB4, // if player is dead
            sname = 0x205, // player name
            iteam = 0x30C, // which team the player with
            iCurrentAmmo = 0x140, // player current ammo
            iNumOfPlayers = 0; // Number of players 
    }
}
