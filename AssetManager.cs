using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoSound;

namespace GazeOGL
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
        static SoundEffect[] victoryDitties;
        public static void Load(ContentManager Content)
        {
            string spriteContentPath = "Content/Sprites/";
            ships = new Texture2D[18];
            ships[0] = Texture2D.FromFile(Main.device, spriteContentPath+"Hunter.png");
            ships[1] = Texture2D.FromFile(Main.device, spriteContentPath+"Conqueror.png");
            ships[2] = Texture2D.FromFile(Main.device, spriteContentPath+"Strafer.png");
            ships[3] = Texture2D.FromFile(Main.device, spriteContentPath+"EscortCore.png");
            ships[4] = Texture2D.FromFile(Main.device, spriteContentPath+"Illusioner.png");
            ships[5] = Texture2D.FromFile(Main.device, spriteContentPath+"Assassin.png");
            ships[6] = Texture2D.FromFile(Main.device, spriteContentPath+"Palladin.png");
            ships[7] = Texture2D.FromFile(Main.device, spriteContentPath+"Trooper.png");
            ships[8] = Texture2D.FromFile(Main.device, spriteContentPath+"Eagle.png");
            ships[9] = Texture2D.FromFile(Main.device, spriteContentPath+"Apocalypse.png");
            ships[10] = Texture2D.FromFile(Main.device, spriteContentPath+"Trebuchet.png");
            ships[11] = Texture2D.FromFile(Main.device, spriteContentPath+"Buccaneer.png");
            ships[12] = Texture2D.FromFile(Main.device, spriteContentPath+"Missionary.png");
            ships[13] = Texture2D.FromFile(Main.device, spriteContentPath+"Lancer.png");
            ships[14] = Texture2D.FromFile(Main.device, spriteContentPath+"Mechanic.png");
            ships[15] = Texture2D.FromFile(Main.device, spriteContentPath+"Frigate.png");
            ships[16] = Texture2D.FromFile(Main.device, spriteContentPath+"Brute.png");
            ships[17] = Texture2D.FromFile(Main.device, spriteContentPath+"Surge.png");
            
            projectiles = new Texture2D[21];
            projectiles[0] = Texture2D.FromFile(Main.device, spriteContentPath+"HunterPelt.png");
            projectiles[1] = Texture2D.FromFile(Main.device, spriteContentPath+"TripwireTips.png");
            projectiles[2] = Texture2D.FromFile(Main.device, spriteContentPath+"TripwireLink.png");
            projectiles[3] = Texture2D.FromFile(Main.device, spriteContentPath+"Missile.png");
            projectiles[4] = Texture2D.FromFile(Main.device, spriteContentPath+"EMP.png");      
            projectiles[5] = Texture2D.FromFile(Main.device, spriteContentPath+"GreenPulse.png");
            projectiles[6] = Texture2D.FromFile(Main.device, spriteContentPath+"IllBolt.png");
            projectiles[7] = Texture2D.FromFile(Main.device, spriteContentPath+"TwistedWormhole.png");
            projectiles[8] = Texture2D.FromFile(Main.device, spriteContentPath+"PalShell.png");
            projectiles[9] = Texture2D.FromFile(Main.device, spriteContentPath+"TropperWave.png"); //whoops...
            projectiles[10] = Texture2D.FromFile(Main.device, spriteContentPath+"MicroMissile.png");
            projectiles[11] = Texture2D.FromFile(Main.device, spriteContentPath+"MarkOfStatehood.png");
            projectiles[12] = Texture2D.FromFile(Main.device, spriteContentPath+"Blotch.png");
            projectiles[13] = Texture2D.FromFile(Main.device, spriteContentPath+"Mine.png");
            projectiles[14] = Texture2D.FromFile(Main.device, spriteContentPath+"Kugelblitz.png");
            projectiles[15] = Texture2D.FromFile(Main.device, spriteContentPath+"Grapple.png");
            projectiles[16] = Texture2D.FromFile(Main.device, spriteContentPath+"GrappleChain.png");
            projectiles[17] = Texture2D.FromFile(Main.device, spriteContentPath+"Inq.png");
            projectiles[18] = Texture2D.FromFile(Main.device, spriteContentPath+"PsuedostableVacum.png");
            projectiles[19] = Texture2D.FromFile(Main.device, spriteContentPath+"MechShot.png");
            projectiles[20] = Texture2D.FromFile(Main.device, spriteContentPath+"Capsule.png");

            ui = new Texture2D[21];
            ui[0] = Texture2D.FromFile(Main.device, spriteContentPath+"UISide.png");
            ui[1] = Texture2D.FromFile(Main.device, spriteContentPath+"SideBar.png");
            ui[2] = Texture2D.FromFile(Main.device, spriteContentPath+"StatBar.png");
            ui[3] = Texture2D.FromFile(Main.device, spriteContentPath+"EnergyUnit.png");
            ui[4] = Texture2D.FromFile(Main.device, spriteContentPath+"LifeUnit.png");
            ui[5] = Texture2D.FromFile(Main.device, spriteContentPath+"X.png");
            ui[6] = Texture2D.FromFile(Main.device, spriteContentPath+"MenuBox.png");
            ui[7] = Texture2D.FromFile(Main.device, spriteContentPath+"Selector.png");
            ui[8] = Texture2D.FromFile(Main.device, spriteContentPath+"Selector2.png");
            ui[9] = Texture2D.FromFile(Main.device, spriteContentPath+"StraferWithTurrets.png");
            ui[10] = Texture2D.FromFile(Main.device, spriteContentPath+"Escort.png");
            ui[11] = Texture2D.FromFile(Main.device, spriteContentPath+"UIBottom.png");
            ui[12] = Texture2D.FromFile(Main.device, spriteContentPath+"Cursor.png");
            ui[13] = Texture2D.FromFile(Main.device, spriteContentPath+"ApocalypseWithTurret.png");
            ui[14] = Texture2D.FromFile(Main.device, spriteContentPath+"BuccaneerWithSword.png");
            ui[15] = Texture2D.FromFile(Main.device, spriteContentPath+"BlankPortrait.png");
            ui[16] = Texture2D.FromFile(Main.device, spriteContentPath+"SurgeWithLightning.png");
            ui[17] = Texture2D.FromFile(Main.device, spriteContentPath+"MissionaryWithTurrets.png");
            ui[18] = Texture2D.FromFile(Main.device, spriteContentPath+"LifeIcon.png");
            ui[19] = Texture2D.FromFile(Main.device, spriteContentPath+"EnergyIcon.png");
            ui[20] = Texture2D.FromFile(Main.device, spriteContentPath+"SlideInUIBox.png");

            turrets = new Texture2D[6];
            turrets[0] = Texture2D.FromFile(Main.device, spriteContentPath+"StraferTurret.png");
            turrets[1] = Texture2D.FromFile(Main.device, spriteContentPath+"EscortTurret.png");
            turrets[2] = Texture2D.FromFile(Main.device, spriteContentPath+"ApocalypseTurret.png");
            turrets[3] = Texture2D.FromFile(Main.device, spriteContentPath+"Sword.png");
            turrets[4] = Texture2D.FromFile(Main.device, spriteContentPath+"MissionaryTurret.png");
            turrets[5] = Texture2D.FromFile(Main.device, spriteContentPath+"TrebuchetTurret.png");

            effects = new Texture2D[3];
            effects[0] = Texture2D.FromFile(Main.device, spriteContentPath+"EMPBlast.png");
            effects[1] = Texture2D.FromFile(Main.device, spriteContentPath+"ShellSplash.png");
            effects[2] = Texture2D.FromFile(Main.device, spriteContentPath+"KugelCrash.png");

            planets = new Texture2D[1];
            planets[0] = Texture2D.FromFile(Main.device, spriteContentPath+"Planet.png");

            extraEntities = new Texture2D[15];
            extraEntities[0] = Texture2D.FromFile(Main.device, spriteContentPath+"EscortPlatform.png");
            extraEntities[1] = Texture2D.FromFile(Main.device, spriteContentPath+"EagleAnimated.png");
            extraEntities[2] = Texture2D.FromFile(Main.device, spriteContentPath+"Eaglet.png");
            extraEntities[3] = Texture2D.FromFile(Main.device, spriteContentPath+"KineticShield.png");
            extraEntities[4] = Texture2D.FromFile(Main.device, spriteContentPath+"RepairBeam.png");
            extraEntities[5] = Texture2D.FromFile(Main.device, spriteContentPath+"LightningAnchor.png");
            extraEntities[6] = Texture2D.FromFile(Main.device, spriteContentPath+"Emitter.png");
            extraEntities[7] = Texture2D.FromFile(Main.device, spriteContentPath+"ApocalypseEyelids.png");
            extraEntities[8] = Texture2D.FromFile(Main.device, spriteContentPath+"MissileFinLeft.png");
            extraEntities[9] = Texture2D.FromFile(Main.device, spriteContentPath+"MissileFinRight.png");
            extraEntities[10] = Texture2D.FromFile(Main.device, spriteContentPath+"ConquerorHit.png");
            extraEntities[11] = Texture2D.FromFile(Main.device, spriteContentPath+"ConquerorNoEngines.png");
            extraEntities[12] = Texture2D.FromFile(Main.device, spriteContentPath+"ConquerorEngine.png");
            extraEntities[13] = Texture2D.FromFile(Main.device, spriteContentPath+"EscortPlatformDebris.png");
            extraEntities[14] = Texture2D.FromFile(Main.device, spriteContentPath+"TrebuchetWithTurret.png");

            string soundContentPath = "Content/Sounds/";
            
            sounds = new SoundEffect[16];
            sounds[0] = MonoSoundManager.GetEffect(soundContentPath+"Pew.mp3"); //Pew
            sounds[1] = MonoSoundManager.GetEffect(soundContentPath+"Pew2.mp3");
            sounds[2] = MonoSoundManager.GetEffect(soundContentPath+"Shield.mp3"); //shield
            sounds[3] = MonoSoundManager.GetEffect(soundContentPath+"Warp.mp3");
            sounds[4] = MonoSoundManager.GetEffect(soundContentPath+"Death.mp3");
            sounds[5] = MonoSoundManager.GetEffect(soundContentPath+"MissileLaunch.mp3");
            sounds[6] = MonoSoundManager.GetEffect(soundContentPath+"WeakHit.mp3");
            sounds[7] = MonoSoundManager.GetEffect(soundContentPath+"CallMilitia.mp3"); //csll militia
            sounds[8] = MonoSoundManager.GetEffect(soundContentPath+"TwinFang.mp3"); //twin fang
            sounds[9] = MonoSoundManager.GetEffect(soundContentPath+"Zap.mp3"); //zap
            sounds[10] = MonoSoundManager.GetEffect(soundContentPath+"Beam.mp3"); //Beam
            sounds[11] = MonoSoundManager.GetEffect(soundContentPath+"CreateIllusion.mp3");
            sounds[12] = MonoSoundManager.GetEffect(soundContentPath+"IllusionDown.mp3");
            sounds[13] = MonoSoundManager.GetEffect(soundContentPath+"Small Explosion.mp3");
            sounds[14] = MonoSoundManager.GetEffect(soundContentPath+"MediumHit.mp3");

            string victoryDittyPath = soundContentPath+"VictoryDitties/";
            victoryDitties = new SoundEffect[18];
            victoryDitties[(int)ShipID.Apocalypse] = MonoSoundManager.GetEffect(victoryDittyPath+"Apocalypse.mp3");
            victoryDitties[(int)ShipID.Assassin] = MonoSoundManager.GetEffect(victoryDittyPath+"Assassin.mp3");
            victoryDitties[(int)ShipID.Brute] = MonoSoundManager.GetEffect(victoryDittyPath+"Brute.mp3");
            victoryDitties[(int)ShipID.Buccaneer] = MonoSoundManager.GetEffect(victoryDittyPath+"Bucaneer.mp3");
            victoryDitties[(int)ShipID.Conqueror] = MonoSoundManager.GetEffect(victoryDittyPath+"Conqueror.mp3");
            victoryDitties[(int)ShipID.Escort] = MonoSoundManager.GetEffect(victoryDittyPath+"Escort.mp3");
            victoryDitties[(int)ShipID.Frigate] = MonoSoundManager.GetEffect(victoryDittyPath+"Frigate.mp3");
            victoryDitties[(int)ShipID.Hunter] = MonoSoundManager.GetEffect(victoryDittyPath+"Hunter.mp3");
            victoryDitties[(int)ShipID.Illusioner] = MonoSoundManager.GetEffect(victoryDittyPath+"Illusionist.mp3");
            victoryDitties[(int)ShipID.Lancer] = MonoSoundManager.GetEffect(victoryDittyPath+"Lancer.mp3");
            victoryDitties[(int)ShipID.Mechanic] = MonoSoundManager.GetEffect(victoryDittyPath+"Mechanic.mp3");
            victoryDitties[(int)ShipID.Missionary] = MonoSoundManager.GetEffect(victoryDittyPath+"Missionary.mp3");
            victoryDitties[(int)ShipID.Palladin] = MonoSoundManager.GetEffect(victoryDittyPath+"Palladin.mp3");
            victoryDitties[(int)ShipID.Strafer] = MonoSoundManager.GetEffect(victoryDittyPath+"Strafer.mp3");
            victoryDitties[(int)ShipID.Surge] = MonoSoundManager.GetEffect(victoryDittyPath+"Surge.mp3");
            victoryDitties[(int)ShipID.Trebeche] = MonoSoundManager.GetEffect(victoryDittyPath+"Trebuchet.mp3");
            victoryDitties[(int)ShipID.Trooper] = MonoSoundManager.GetEffect(victoryDittyPath+"Trooper.mp3");
            victoryDitties[(int)ShipID.Eagle] = MonoSoundManager.GetEffect(victoryDittyPath+"USG.mp3");

            music = MonoSoundManager.GetEffect(soundContentPath+"Intro.mp3").CreateInstance();
            music.IsLooped = true;
            music.Play();
            UpdateMusicVolume();
        }
        static SoundEffectInstance music;
        public static float defaultVolume = 0.4f;
        public static void PlaySound(SoundID soundID, float pitch = 0)
        {
            if (Main.mode != Mode.Menu && Main.mode != Mode.Test)
            {
                float volume = defaultVolume;
                switch (soundID)
                {
                    case SoundID.Warp:
                        volume *= 0.3f;
                        break;
                    case SoundID.CallMilitia:
                        volume *= 0.5f;
                        break;
                }
                sounds[(int)soundID].Play(volume, pitch, 0);
            }
        }
        public static void PlayDitty(ShipID shipID)
        {
            if (Main.mode != Mode.Menu && Main.mode != Mode.Test)
            {
                float volume = defaultVolume;
                victoryDitties[(int)shipID].Play(volume, 0, 0);
                
            }
        }

        public static float defaultMusicVolume = 0.5f;
        public static void UpdateMusicVolume()
        {
            music.Volume = defaultMusicVolume;
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
        TwinFang,
        Zap,
        Beam,
        CreateIllusion,
        IllusionDown,
        SmallExplosion,
        MediumHit
    }
}
