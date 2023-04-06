namespace SpellBoundAR.Boids
{
    public interface IBoidSpawner
    {
        public BoidManager Manager { get; set; }
        public void SpawnBoids();
    }
}