using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeOGL
{
    public class Particle
    {
        public Vector2 Position;
        public int timeLeft;
        public Color color = Color.White;
        public Vector2 velocity = Vector2.Zero;
        public Particle(Vector2 Position, int timeLeft, Color color)
        {
            this.Position = Position;
            this.timeLeft = timeLeft;
            this.color = color;
            Arena.particles.Add(this);
        }
        public Particle(Vector2 Position, int timeLeft, Color color, Vector2 velocity)
        {
            this.Position = Position;
            this.timeLeft = timeLeft;
            this.color = color;
            this.velocity = velocity;
            Arena.particles.Add(this);
        }
        public Particle(Vector2 Position, int timeLeft)
        {
            this.Position = Position;
            this.timeLeft = timeLeft;
            Arena.particles.Add(this);
        }
        public void Update()
        {
            Position += velocity;
            timeLeft--;
            if (timeLeft == 0)
            {
                Arena.particles.Remove(this);
            }
        }
    }
}
