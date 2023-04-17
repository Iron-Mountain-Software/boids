using System.Collections.Generic;

namespace SpellBoundAR.Boids
{
    public interface IBoidSpawner
    {
        public BoidManager Manager { get; set; }
        public List<Boid> SpawnBoids();
    }
}