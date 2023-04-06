using System;
using System.Collections.Generic;
using SpellBoundAR.Boids.Cages;
using UnityEngine;

namespace SpellBoundAR.Boids
{
    public class BoidManager : MonoBehaviour
    {
        [Header("Settings: Spawning")]
        [SerializeField] private BoidCage cage;
        public LayerMask obstacleLayers;
        
        [Header("Settings: Steering")]
        public float speed;
        public int framesBetweenForceUpdates = 0;
        public Space movementSpace = Space.World;
        public float perceptionRadius;
        [Space]
        public float separationWeight;
        public float cohesionWeight;
        public float alignmentWeight;
        public float avoidWallsWeight;
        public float avoidObstacleWeight;
        [Space]
        public float obstacleViewDistance;

        [Header("References")]
        private readonly List<Boid> _boids = new ();
        
        public List<Boid> Boids => _boids;
        public BoidCage Cage => cage;

        public void RegisterBoid(Boid boid)
        {
            if (!boid || _boids.Contains(boid)) return;
            _boids.Add(boid);
        }

        public void UnregisterBoid(Boid boid)
        {
            if (!boid || !_boids.Contains(boid)) return;
            _boids.Remove(boid);
        }

        public void DestroyBoids()
        {
            List<Boid> toDestroy = _boids;
            foreach (Boid boid in toDestroy)
            {
                if (boid) Destroy(boid.gameObject);
            }
            _boids.Clear();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (framesBetweenForceUpdates < 0) framesBetweenForceUpdates = 0;
        }

#endif
        
    }
}