using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace SpellBoundAR.Boids.Cages
{
    public class BoidContainerHemisphere : BoidContainer
    {
        [SerializeField] private float radius = 5;

        [Header("Cache")]
        private Plane _plane = new ();
        
        public override Vector3 GetRandomWorldPositionInCage()
        {
            Vector3 randomPoint = Random.insideUnitSphere * radius;
            _plane.SetNormalAndPosition(transform.up, Vector3.zero);
            if (!_plane.GetSide(randomPoint)) randomPoint = -randomPoint;
            return Position + Offset + randomPoint;
        }

        public override bool WorldPositionIsInCage(Vector3 worldPosition)
        {
            Vector3 cagePosition = Position + Offset;
            Vector3 delta = worldPosition - cagePosition;
            bool inSphere = delta.magnitude < radius;
            _plane.SetNormalAndPosition(transform.up, cagePosition);
            bool correctSide = _plane.GetSide(worldPosition);
            return inSphere && correctSide;
        }

        public override Vector3 ClosestPointInOrOnCage(Vector3 worldPosition)
        {
            Vector3 cagePosition = Position + Offset;
            Vector3 delta = worldPosition - cagePosition;
            bool inSphere = delta.magnitude < radius;
            _plane.SetNormalAndPosition(transform.up, cagePosition);
            bool correctSide = _plane.GetSide(worldPosition);
            if (inSphere && correctSide) return worldPosition;
            if (correctSide) return cagePosition + delta.normalized * radius;
            Vector3 closestPointOnPlane = _plane.ClosestPointOnPlane(worldPosition);
            Vector3 closestPointDelta = closestPointOnPlane - cagePosition;
            if (closestPointDelta.magnitude < radius) return closestPointOnPlane;
            return cagePosition + closestPointDelta.normalized * radius;
        }
        
#if UNITY_EDITOR
        
        private void OnValidate()
        {
            if (radius < 0) radius = 0;
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 normal = transform.up;
            Handles.DrawWireDisc(Position + Offset, normal, radius);
            Vector3 perpendicular = Vector3.Cross(normal, Vector3.right).normalized;
            if (perpendicular == Vector3.zero) perpendicular = Vector3.Cross(normal, Vector3.forward).normalized;
            int arcs = 8;
            for (int i = 0; i < arcs; i++)
            {
                float progress = (float) i / arcs;
                Vector3 arcNormal = Quaternion.AngleAxis(Mathf.Lerp(0, 360, progress), normal) * perpendicular;
                Handles.DrawWireArc(Position + Offset, arcNormal, normal, 90, radius);
            }
        }

#endif
        
    }
}