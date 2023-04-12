using UnityEngine;
using Random = UnityEngine.Random;

namespace SpellBoundAR.Boids.Containers
{
    public class BoidContainerCube : BoidContainer
    {
        [SerializeField] private Vector3 dimensions = new (10, 10, 10);
        
        public Vector3 Dimensions
        {
            get => dimensions;
            set => dimensions = value;
        }

        public override Vector3 GetRandomWorldPositionInContainer()
        {
            Vector3 cagePosition = Position + Offset;
            Vector3 currentScale = Scale;
            float xRadius = currentScale.x * Dimensions.x / 2;
            float yRadius = currentScale.y * Dimensions.y / 2;
            float zRadius = currentScale.z * Dimensions.z / 2;
            return new Vector3(
                cagePosition.x + Random.Range(-xRadius, xRadius),
                cagePosition.y + Random.Range(-yRadius, yRadius),
                cagePosition.z + Random.Range(-zRadius, zRadius)
            );
        }

        public override bool WorldPositionIsInContainer(Vector3 worldPosition)
        {
            Vector3 cagePosition = Position + Offset;
            Vector3 currentScale = Scale;
            float xRadius = currentScale.x * Dimensions.x / 2;
            float yRadius = currentScale.y * Dimensions.y / 2;
            float zRadius = currentScale.z * Dimensions.z / 2;
            return cagePosition.x - xRadius <= worldPosition.x 
                   && worldPosition.x <= cagePosition.x + xRadius 
                   && cagePosition.y - yRadius <= worldPosition.y 
                   && worldPosition.y <= cagePosition.y + yRadius
                   && cagePosition.z - zRadius <= worldPosition.z
                   && worldPosition.z <= cagePosition.z + zRadius;
        }

        public override Vector3 ClosestPointInOrOnContainer(Vector3 worldPosition)
        {
            Vector3 cagePosition = Position + Offset;
            Vector3 currentScale = Scale;
            float xRadius = currentScale.x * Dimensions.x / 2;
            float yRadius = currentScale.y * Dimensions.y / 2;
            float zRadius = currentScale.z * Dimensions.z / 2;
            return new Vector3(
                Mathf.Clamp(worldPosition.x, cagePosition.x - xRadius, cagePosition.x + xRadius),
                Mathf.Clamp(worldPosition.y, cagePosition.y - yRadius, cagePosition.y + yRadius),
                Mathf.Clamp(worldPosition.z, cagePosition.z - zRadius, cagePosition.z + zRadius)
            );
        }
        
#if UNITY_EDITOR
        
        private void OnValidate()
        {
            if (dimensions.x < 0) dimensions.x = 0;
            if (dimensions.y < 0) dimensions.y = 0;
            if (dimensions.z < 0) dimensions.z = 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Vector3 currentScale = Scale;
            Gizmos.DrawWireCube(
                Position + Offset, 
                new Vector3(
                    currentScale.x * Dimensions.x,
                    currentScale.y * Dimensions.y,
                    currentScale.z * Dimensions.z
                    )
                );
        }

#endif

    }
}