using UnityEngine;

namespace SpellBoundAR.Boids.Cages
{
    [ExecuteAlways]
    public abstract class BoidCage : MonoBehaviour
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

        public abstract Vector3 GetRandomWorldPositionInCage();
        public abstract bool WorldPositionIsInCage(Vector3 worldPosition);
        public abstract Vector3 ClosestPointInOrOnCage(Vector3 worldPosition);
    }
}