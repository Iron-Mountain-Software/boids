using System;
using System.Collections.Generic;
using SpellBoundAR.Boids.Cages;
using UnityEngine;

namespace SpellBoundAR.Boids
{
    public class BoidManager : MonoBehaviour
    {
        public event Action OnBoidsChanged;
        
        [Serializable]
        public struct ContainerAvoidanceSettings
        {
            [SerializeField] private bool enabled;
            [SerializeField] private float weight;
            [SerializeField] private BoidContainer container;

            public bool Enabled => enabled;
            public float Weight => weight;
            public BoidContainer Container => container;
        }
        
        [Serializable]
        public struct ColliderAvoidanceSettings
        {
            [SerializeField] private bool enabled;
            [SerializeField] private float weight;
            [SerializeField] private LayerMask layers;
            [SerializeField] private float raycastDistance;

            public bool Enabled => enabled;
            public float Weight => weight;
            public LayerMask Layers => layers;
            public float RaycastDistance => raycastDistance;
        }
        
        public float speed;
        public int framesBetweenForceUpdates = 0;
        public Space movementSpace = Space.World;
        public float perceptionRadius;
        [Space]
        public float separationWeight;
        public float cohesionWeight;
        public float alignmentWeight;
        [Space]
        [SerializeField] private ContainerAvoidanceSettings containerAvoidance;
        [SerializeField] private ColliderAvoidanceSettings colliderAvoidance;

        [Header("Cache")]
        private readonly List<Boid> _boids = new ();

        public ContainerAvoidanceSettings ContainerAvoidance => containerAvoidance;
        public ColliderAvoidanceSettings ColliderAvoidance => colliderAvoidance;
        public List<Boid> Boids => _boids;

        public void RegisterBoid(Boid boid)
        {
            if (!boid || _boids.Contains(boid)) return;
            _boids.Add(boid);
            OnBoidsChanged?.Invoke();
        }

        public void UnregisterBoid(Boid boid)
        {
            if (!boid || !_boids.Contains(boid)) return;
            _boids.Remove(boid);
            OnBoidsChanged?.Invoke();
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