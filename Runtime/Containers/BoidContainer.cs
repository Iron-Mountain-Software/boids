using UnityEngine;

namespace IronMountain.Boids.Containers
{
    [ExecuteAlways]
    public abstract class BoidContainer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector3 offset;
        
        [Header("Cache")]
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public Vector3 Offset
        {
            get => offset;
            set => offset = value;
        }

        public Vector3 Position        
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        
        public Vector3 Scale => _transform.localScale;

        public abstract Vector3 GetRandomWorldPositionInContainer();
        public abstract bool WorldPositionIsInContainer(Vector3 worldPosition);
        public abstract Vector3 ClosestPointInOrOnContainer(Vector3 worldPosition);
    }
}