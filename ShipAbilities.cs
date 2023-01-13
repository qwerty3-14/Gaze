using Microsoft.Xna.Framework.Graphics;

namespace GazeOGL
{
    public static class ShipAbilities
    {
        public static void GetMain(ShipID type, out string name, out string description)
        {
            name = "";
            description = "";
            switch (type)
            {
                case ShipID.Hunter:
                    name = "Hunter's Pelt";
                    description = "A machine gun with low velocity, but otheriwise well rounded stats. When paired with the hunter's large battery and reactor it can sustain fire for a long time.";
                    break;
                case ShipID.Conqueror:
                    name = "Very Good Missiles";
                    description = "Homing Missiles with high damage and range. Over time the missile's top speed reduces while its turn speed increases.";
                    break;
                case ShipID.Strafer:
                    name = "4xQ Beams";
                    description = "Beams mounted on fast rotating turrets. Great at shooting down enemy projectiles, and the enemy ship itself.";
                    break;
                case ShipID.Escort:
                    name = "Modular Death Projector";
                    description = "Fires weak projectiles at short range. But when amplifed by a combat platform it becomes a much deadlier beam";
                    break;
                case ShipID.Illusioner:
                    name = "Ill bolt";
                    description = "Low damage output but the projectile can't be shot down and has decent range. Illusions will fire thier own fake version that does no damage.";
                    break;
                case ShipID.Assassin:
                    name = "K.N.I.F.E. Beam";
                    description = "Kill now in ferocious efficency! Stupid high damage, but short ranged beam";
                    break;
                case ShipID.Palladin:
                    name = "Quad champp cannons";
                    description = "Each cheap and mass produced pellet may be weak but no other weapon system fires this quickly";
                    break;
                case ShipID.Trooper:
                    name = "GRENE(tm) wave";
                    description = "GRENE(tm) matter forced into a wave shape causing it to fly through space a high velocity over long distances";
                    break;
                case ShipID.Eagle:
                    name = "Mark of Statehood";
                    description = "A short ranged attack that's automaticly aimed at the enemy.";
                    break;
                case ShipID.Apocalypse:
                    name = "Blotch";
                    description = "A long ranged semi machingun with very low velocity. Depletes enemy battery in addition to armor.";
                    break;
                case ShipID.Trebeche:
                    name = "Fall of the Kugelblitz";
                    description = "Fires 2 microscopic black holes that create a massive explosion when they collide with each other. They can't be shot down but won't deal damage until exploding. Hold the attack to deploy them further away.";
                    break;
                case ShipID.Buccaneer:
                    name = "Sword";
                    description = "Very effective at slicing down enemy attacks and enemy ships";
                    break;
                case ShipID.Missionary:
                    name = "Judgment Cannons";
                    description = "Extremly reliable cannons that excel at just about anything. However this comes at the cost of energy efficency, constant fire will rapidly deplete your battery.";
                    break;
                case ShipID.Lancer:
                    name = "Jousting Beam";
                    description = "Charge up then release to do up to 8 damage. Reactor is redirected to the beam while charging";
                    break;
                case ShipID.Mechanic:
                    name = "Repurposed Detonator";
                    description = "Moderate damage with a small spread. A skilled pilot can shoot down most enemy projectiles with it";
                    break;
                case ShipID.Frigate:
                    name = "Capsule";
                    description = "The capsule can explode at long range, emitting a classified substance that damages any nearby hostile entities. Hold attack to make the capsule fly then release to detonate.";
                    break;
                case ShipID.Brute:
                    name = "Shotgun of Excessive Force";
                    description = "Weapons this powerful are rarely used on ships of this size. Every shot sends the ship flying backwards.";
                    break;
                case ShipID.Surge:
                    name = "Chain Lightning";
                    description = "Lighting that bounces between lighting anchors and hostile entities.";
                    break;

            }
        }
        public static void GetSecondary(ShipID type, out string name, out string description)
        {
            name = "";
            description = "";
            switch (type)
            {
                case ShipID.Hunter:
                    name = "Tripwire snare";
                    description = "Teleports a tripwire in front of the enemy, on contact the enemy is disabled for 3 seconds.";
                    break;
                case ShipID.Conqueror:
                    name = "Switchero of Doom";
                    description = "Swaps places with the enemy ship causing them to take whatever beating they planned for you!";
                    break;
                case ShipID.Strafer:
                    name = "EMP Cannon";
                    description = "Long ranged cannon that does no damage but devistates the enemy's battery.";
                    break;
                case ShipID.Escort:
                    name = "Detach/Detonate/Rebuild";
                    description = "When attached seperate from the combat platform. The platform's defenses will continue to function while detached. While detached and the platform is alive, detonate the platform causing damage in an area around it based on the platform's armor. If there is no active platform and battery is full reconstruct a new platform attached to the ship.";
                    break;
                case ShipID.Illusioner:
                    name = "Create Illusions";
                    description = "Create 4 illusions that look identical to you. The direction pushed when the illusions are created determins where in the formation you end up. May have some minor reality altering effects.";
                    break;
                case ShipID.Assassin:
                    name = "Twisted wormhole";
                    description = "Fire a wormhole that can be teleported to when sepcial is released. Teleporting near an enemy ship causes you to face them. Direct hit on the enemy instantly teleports you right behind them.";
                    break;
                case ShipID.Palladin:
                    name = "Aura";
                    description = "Greatly slows down all hostile entites in a large area around the ship.";
                    break;
                case ShipID.Trooper:
                    name = "Micro misiles";
                    description = "Small fast short ranged missiles propelled by GRENE(tm) matter, they are good for blowing up enemy projectiles or dishing out some burst damage and knockback on foes";
                    break;
                case ShipID.Eagle:
                    name = "Call Militia";
                    description = "Creates a mini eaglet drone to fight for you. Two eaglets can be active at once";
                    break;
                case ShipID.Apocalypse:
                    name = "Mine";
                    description = "High damage and knockback mines that cloak when far away and pursue when close";
                    break;
                case ShipID.Trebeche:
                    name = "Kinetic Shield";
                    description = "For half a second half of incoming damage is converted into kinetic energy";
                    break;
                case ShipID.Buccaneer:
                    name = "Grappling hook";
                    description = "Fire a hook at the enemy ship, once stuck using special will pull the ships closer.";
                    break;
                case ShipID.Missionary:
                    name = "Psuedostable vacum";
                    description = "A region of space where the laws of reality are fundamentaly changed, scrambling anything that contacts it.";
                    break;
                case ShipID.Lancer:
                    name = "Lunge";
                    description = "Instantly launch the ship in the direction it faces.";
                    break;
                case ShipID.Mechanic:
                    name = "Repair beam";
                    description = "Enter a vulnerable reapir mode for 1.5 seconds, after which 4 armor is repaired";
                    break;
                case ShipID.Frigate:
                    name = "Emitter";
                    description = "A repurposed capsule where the propulsion is replaced with more emitters. Only 1 active at a time";
                    break;
                case ShipID.Brute:
                    name = "Berserk";
                    description = "As if the shotgun wasn't strong enough, the brute can damage itself to power up its next shot!";
                    break;
                case ShipID.Surge:
                    name = "Lighting  anchor";
                    description = "Places lighting anchors to improve the chain lighting's coverage";
                    break;

            }
        }
        public static void GetPerk(ShipID type, out string name, out string description)
        {
            name = "None";
            description = "This ship has no passive ability";
            switch (type)
            {
                case ShipID.Conqueror:
                    name = "Impact Reactor";
                    description = "Damage recieved charges the ship's battery";
                    break;
                case ShipID.Escort:
                    name = "Combat Platform";
                    description = "Start battle attached to a combat platform. Combat platforms have 2 defense turrets. While detached accelration and turn speed are increased.";
                    break;
                case ShipID.Illusioner:
                    name = "Incroporeal";
                    description = "Collisions won't effect your movement.";
                    break;
                case ShipID.Eagle:
                    name = "Instantnious acceleration";
                    description = "Acceleration is instant";
                    break;
                case ShipID.Apocalypse:
                    name = "Heavy Turret";
                    description = "Slow but powerful defensive beam turret.";
                    break;
                case ShipID.Surge:
                    name = "Reverse Thrust";
                    description = "This ship can accelerate backwards! Why has no one else done this?";
                    break;
                case ShipID.Trebeche:
                    name = "Swatter Turret";
                    description = "Small, lower power, but precise forward firing defensive turret. Reliably knocks out weak projectiles approaching from the front.";
                    break;

            }
        }
    }
}