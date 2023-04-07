using UnityEngine;
using Random = UnityEngine.Random;

namespace SpellBoundAR.Boids
{
    public class BoidSpawner : MonoBehaviour, IBoidSpawner
    {
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private BoidManager manager;
        [SerializeField] private Boid prefab;
        [SerializeField] private Transform parent;
        [SerializeField] private int amount = 1;

        public BoidManager Manager
        {
            get => manager;
            set => manager = value;
        }

        private void Start()
        {
            if (spawnOnStart) SpawnBoids();
        }

        public void SpawnBoids()
        {
            if (!prefab) return;
            for (int i = 0; i < amount; i++)
            {
                Vector3 position = manager ? manager.transform.position : Vector3.zero;
                if (manager && manager.ContainerAvoidance.Container) position = manager.ContainerAvoidance.Container.GetRandomWorldPositionInCage();
                
                Quaternion rotation = Quaternion.Euler(
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f)
                );
                
                Instantiate(prefab, position, rotation, parent).Initialize(manager);
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (!manager) manager = GetComponentInParent<BoidManager>();
            if (amount < 0) amount = 0;
        }

#endif
    }
}
