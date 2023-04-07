using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace SpellBoundAR.Boids.Cages
{
    public class BoidContainerSphere : BoidContainer
    {
        [SerializeField] private float radius = 5;

        public override Vector3 GetRandomWorldPositionInCage()
        {
            return Position + Offset + Random.insideUnitSphere * radius;
        }

        public override bool WorldPositionIsInCage(Vector3 worldPosition)
        {
            Vector3 cagePosition = Position + Offset;
            return (worldPosition - cagePosition).magnitude < radius;
        }

        public override Vector3 ClosestPointInOrOnCage(Vector3 worldPosition)
        {
            Vector3 cagePosition = Position + Offset;
            Vector3 delta = worldPosition - cagePosition;
            if (delta.magnitude < radius) return worldPosition;
            return cagePosition + delta.normalized * radius;
        }
        
#if UNITY_EDITOR
        
        private void OnValidate()
        {
            if (radius < 0) radius = 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(Position + Offset, radius);
        }

#endif
        
    }
}
