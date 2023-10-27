using System.Collections.Generic;

namespace IronMountain.Boids
{
    public interface IBoidSpawner
    {
        public BoidManager Manager { get; set; }
        public List<Boid> SpawnBoids();
    }
}