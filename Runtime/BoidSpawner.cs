using UnityEngine;
using Random = UnityEngine.Random;

namespace SpellBoundAR.Boids
{
    public class BoidSpawner : MonoBehaviour, IBoidSpawner
    {
        private enum SpawnPositionType
        {
            InManagerContainer,
            AtSelf,
            AtManager
        }

        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private SpawnPositionType spawnPositionType = SpawnPositionType.InManagerContainer;
        [SerializeField] private BoidManager manager;
        [SerializeField] private Boid prefab;
        [SerializeField] private Transform parent;
        [SerializeField] private int amount = 1;

        public BoidManager Manager
        {
            get => manager;
            set => manager = value;
        }
        
        public Boid Prefab
        {
            get => prefab;
            set => prefab = value;
        }
        
        public int Amount
        {
            get => amount;
            set => amount = value;
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
                Vector3 position = GetSpawnPoint();
                Quaternion rotation = Quaternion.Euler(
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f),
                    Random.Range(0f, 360f)
                );
                
                Instantiate(prefab, position, rotation, parent).Initialize(manager);
            }
        }

        private Vector3 GetSpawnPoint()
        {
            switch (spawnPositionType)
            {
                case SpawnPositionType.InManagerContainer:
                    if (!manager) return transform.position;
                    return manager.ContainerAvoidance.Container
                        ? manager.ContainerAvoidance.Container.GetRandomWorldPositionInContainer()
                        : manager.transform.position;
                case SpawnPositionType.AtSelf:
                    return transform.position;
                case SpawnPositionType.AtManager:
                    return manager ? manager.transform.position : transform.position;
                default:
                    return transform.position; 
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
