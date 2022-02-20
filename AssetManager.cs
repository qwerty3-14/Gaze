using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze
{
    public static class AssetManager
    {
        public static Texture2D[] ships;
        public static Texture2D[] projectiles;
        public static Texture2D[] ui;
        public static Texture2D[] turrets;
        public static Texture2D[] effects;
        public static Texture2D[] planets;
        public static Texture2D[] extraEntities;
        public static SoundEffect[] sounds;
        public static void Load(ContentManager Content)
        {
            ships = new Texture2D[18];
            ships[0] = Content.Load<Texture2D>("Hunter");
            ships[1] = Content.Load<Texture2D>("Conqueror");
            ships[2] = Content.Load<Texture2D>("Strafer");
            ships[3] = Content.Load<Texture2D>("EscortCore");
            ships[4] = Content.Load<Texture2D>("Illusioner");
            ships[5] = Content.Load<Texture2D>("Assassin");
            ships[6] = Content.Load<Texture2D>("Palladin");
            ships[7] = Content.Load<Texture2D>("Trooper");
            ships[8] = Content.Load<Texture2D>("Eagle");
            ships[9] = Content.Load<Texture2D>("Apocalypse");
            ships[10] = Content.Load<Texture2D>("Trebeche");
            ships[11] = Content.Load<Texture2D>("Buccaneer");
            ships[12] = Content.Load<Texture2D>("Missionary");
            ships[13] = Content.Load<Texture2D>("Lancer");
            ships[14] = Content.Load<Texture2D>("Mechanic");
            ships[15] = Content.Load<Texture2D>("Frigate");
            ships[16] = Content.Load<Texture2D>("Brute");

            projectiles = new Texture2D[21];
            projectiles[0] = Content.Load<Texture2D>("HunterPelt");
            projectiles[1] = Content.Load<Texture2D>("TripwireTips");
            projectiles[2] = Content.Load<Texture2D>("TripwireLink");
            projectiles[3] = Content.Load<Texture2D>("Missile");
            projectiles[4] = Content.Load<Texture2D>("EMP");
            projectiles[5] = Content.Load<Texture2D>("GreenPulse");
            projectiles[6] = Content.Load<Texture2D>("IllBolt");
            projectiles[7] = Content.Load<Texture2D>("TwistedWormhole");
            projectiles[8] = Content.Load<Texture2D>("PalShell");
            projectiles[9] = Content.Load<Texture2D>("TropperWave"); //whoops...
            projectiles[10] = Content.Load<Texture2D>("MicroMissile");
            projectiles[11] = Content.Load<Texture2D>("MarkOfStatehood");
            projectiles[12] = Content.Load<Texture2D>("Blotch");
            projectiles[13] = Content.Load<Texture2D>("Mine");
            projectiles[14] = Content.Load<Texture2D>("Kugelblitz");
            projectiles[15] = Content.Load<Texture2D>("Grapple");
            projectiles[16] = Content.Load<Texture2D>("GrappleChain");
            projectiles[17] = Content.Load<Texture2D>("Inq");
            projectiles[18] = Content.Load<Texture2D>("PsuedostableVacum");
            projectiles[19] = Content.Load<Texture2D>("MechShot");
            projectiles[20] = Content.Load<Texture2D>("Capsule");

            ui = new Texture2D[16];
            ui[0] = Content.Load<Texture2D>("UISide");
            ui[1] = Content.Load<Texture2D>("UI/SideBar");
            ui[2] = Content.Load<Texture2D>("UI/StatBar");
            ui[3] = Content.Load<Texture2D>("UI/EnergyUnit");
            ui[4] = Content.Load<Texture2D>("UI/LifeUnit");
            ui[5] = Content.Load<Texture2D>("UI/X");
            ui[6] = Content.Load<Texture2D>("UI/MenuBox");
            ui[7] = Content.Load<Texture2D>("UI/Selector");
            ui[8] = Content.Load<Texture2D>("UI/Selector2");
            ui[9] = Content.Load<Texture2D>("StraferWithTurrets");
            ui[10] = Content.Load<Texture2D>("Escort");
            ui[11] = Content.Load<Texture2D>("UIBottom");
            ui[12] = Content.Load<Texture2D>("Cursor");
            ui[13] = Content.Load<Texture2D>("ApocalypseWithTurret");
            ui[14] = Content.Load<Texture2D>("BuccaneerWithSword");
            ui[15] = Content.Load<Texture2D>("BlankPortrait");

            turrets = new Texture2D[54];
            turrets[0] = Content.Load<Texture2D>("StraferTurret");
            turrets[1] = Content.Load<Texture2D>("EscortTurret");
            turrets[2] = Content.Load<Texture2D>("ApocalypseTurret");
            turrets[3] = Content.Load<Texture2D>("Sword");
            turrets[4] = Content.Load<Texture2D>("MissionaryTurret");

            effects = new Texture2D[3];
            effects[0] = Content.Load<Texture2D>("EMPBlast");
            effects[1] = Content.Load<Texture2D>("ShellSplash");
            effects[2] = Content.Load<Texture2D>("KugelCrash");

            planets = new Texture2D[1];
            planets[0] = Content.Load<Texture2D>("Planet");

            extraEntities = new Texture2D[5];
            extraEntities[0] = Content.Load<Texture2D>("EscortPlatform");
            extraEntities[1] = Content.Load<Texture2D>("EagleAnimated");
            extraEntities[2] = Content.Load<Texture2D>("Eaglet");
            extraEntities[3] = Content.Load<Texture2D>("KineticShield");
            extraEntities[4] = Content.Load<Texture2D>("RepairBeam");


            sounds = new SoundEffect[16];
            sounds[0] = Content.Load<SoundEffect>("Pew");
            sounds[1] = Content.Load<SoundEffect>("Pew2");
            sounds[2] = Content.Load<SoundEffect>("Shield");
            sounds[3] = Content.Load<SoundEffect>("Warp");
            sounds[4] = Content.Load<SoundEffect>("Death");
            sounds[5] = Content.Load<SoundEffect>("MissileLaunch");
            sounds[6] = Content.Load<SoundEffect>("WeakHit");
            sounds[7] = Content.Load<SoundEffect>("CallMilitia");
            sounds[8] = Content.Load<SoundEffect>("PlasmaBoulder");
            sounds[9] = Content.Load<SoundEffect>("TwinFang");
            sounds[10] = Content.Load<SoundEffect>("Zap");
            sounds[11] = Content.Load<SoundEffect>("Beam");
            sounds[12] = Content.Load<SoundEffect>("CreateIllusion");
            sounds[13] = Content.Load<SoundEffect>("IllusionDown");
            sounds[14] = Content.Load<SoundEffect>("Small Explosion");
            sounds[15] = Content.Load<SoundEffect>("MediumHit");

        }
        public static void PlaySound(SoundID soundID, float pitch = 0)
        {
            if (Main.mode != Mode.Menu)
            {
                float volume = 0.4f;
                switch (soundID)
                {
                    case SoundID.Warp:
                    case SoundID.CallMilitia:
                        volume *= 0.8f;
                        break;
                }
                sounds[(int)soundID].Play(volume, pitch, 0);
            }
        }
    }
    public enum SoundID : byte
    {
        Pew,
        Pew2,
        Shield,
        Warp,
        Death,
        MissileLaunch,
        WeakHit,
        CallMilitia,
        PlasmaBoulder,
        TwinFang,
        Zap,
        Beam,
        CreateIllusion,
        IllusionDown,
        SmallExplosion,
        MediumHit
    }
}
