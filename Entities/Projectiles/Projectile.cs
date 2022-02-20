using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectGaze.Entities.Projectiles
{
    public class Projectile : Entity
    {
        public int damage = 1;
        public int lifeTime = 180;
        public Projectile(Vector2 position, Vector2 velocity, int team = 0)
        {
            this.position = position;
            this.velocity = velocity;
            this.team = team;
        }
        public override void ModerateUpdate()
        {
            lifeTime--;
            if(lifeTime <=0)
            {
                Expire();
                Kill();
            }
        }
        public virtual void Expire()
        {

        }
        public virtual void OnHit(Entity Victim)
        {

        }

    }
}
