using UnityEngine;
using Random = UnityEngine.Random;

namespace SpellBoundAR.Boids
{
    public class Boid : MonoBehaviour
    {
        private const int MaximumObstacleRaycasts = 15;
        private const float RaycastIncrement = .15f;

        [SerializeField] private BoidManager manager;
        
        [Header("Cache")]
        private int _frameOffset;
        private int _lastUpdateFrame;
        private Vector3 _finalForce;
        
        public BoidManager Manager
        {
            get => manager;
            private set
            {
                if (manager == value) return;
                if (manager) manager.Unregister(this);
                manager = value;
                if (manager && enabled) manager.Register(this);
                RefreshFrameOffset();
            }
        }

        private void OnValidate()
        {
            if (manager && enabled) manager.Register(this);
            RefreshFrameOffset();
        }

        public void Initialize(BoidManager newManager)
        {
            Manager = newManager;
            RefreshFrameOffset();
        }

        private void OnEnable()
        {
            if (Manager) Manager.Register(this);
            RefreshFrameOffset();
        }

        private void OnDisable()
        {
            if (Manager) Manager.Unregister(this);
        }

        private void RefreshFrameOffset()
        {
            _frameOffset = manager ? Random.Range(0, manager.framesBetweenForceUpdates) : 0;
        }

        private void Update()
        {
            if (!Manager) return;
            UpdateForces();
            MoveForward();
        }

        private void UpdateForces()
        {
            if (!Manager) return;
            if (Time.frameCount - _lastUpdateFrame + _frameOffset < Manager.framesBetweenForceUpdates) return;
                
            _lastUpdateFrame = Time.frameCount;
            
            Vector3 myPosition = transform.position;
            Vector3 separationSum = Vector3.zero;
            Vector3 cohesionSum = Vector3.zero;
            Vector3 alignmentSum = Vector3.zero;

            Vector3 separationForce;
            Vector3 cohesionForce;
            Vector3 alignmentForce;
            
            int boidsNearby = 0;

            foreach (Boid otherBoid in Manager.Boids)
            {
                if (!otherBoid || !otherBoid.enabled || otherBoid == this) continue;
                Vector3 otherBoidPosition = otherBoid.transform.position;
                Vector3 separation = myPosition - otherBoidPosition;
                float distToOtherBoid = separation.magnitude;
                if (distToOtherBoid > Manager.perceptionRadius) continue;
                float otherBoidWeight = 1 / Mathf.Max(distToOtherBoid, .00001f);
                separationSum += separation * otherBoidWeight;
                cohesionSum += otherBoidPosition;
                alignmentSum += otherBoid.transform.forward;
                boidsNearby++;
            }
            
            if (boidsNearby > 0)
            {
                separationForce = separationSum / boidsNearby;
                cohesionForce = cohesionSum / boidsNearby - myPosition;
                alignmentForce = alignmentSum / boidsNearby;
            }
            else
            {
                separationForce = Vector3.zero;
                cohesionForce = Vector3.zero;
                alignmentForce = Vector3.zero;
            }

            Vector3 avoidWallsForce = GetContainerForce();
            Vector3 avoidObstacleForce = GetAvoidObstaclesForce();

            _finalForce = 
                separationForce * Manager.separationWeight +
                cohesionForce * Manager.cohesionWeight +
                alignmentForce * Manager.alignmentWeight +
                avoidWallsForce * Manager.ContainerAvoidance.Weight +
                avoidObstacleForce * Manager.ColliderAvoidance.Weight;
        }

        private Vector3 GetContainerForce()
        {
            if (Manager
                && Manager.ContainerAvoidance.Enabled
                && Manager.ContainerAvoidance.Container
                && !Manager.ContainerAvoidance.Container.WorldPositionIsInContainer(transform.position))
            {
                Vector3 myPosition = transform.position;
                Vector3 closestPointOnCage = Manager.ContainerAvoidance.Container.ClosestPointInOrOnContainer(myPosition);
                return (closestPointOnCage - myPosition).normalized;
            }
            return Vector3.zero;
        }

        private Vector3 GetAvoidObstaclesForce()
        {
            if (!Manager || !Manager.ColliderAvoidance.Enabled) return Vector3.zero;

            Vector3 origin = transform.position;
            Vector3 forward = transform.forward;

            if (Physics.Raycast(origin, forward, out RaycastHit hit, Manager.ColliderAvoidance.RaycastDistance, Manager.ColliderAvoidance.Layers))
            {
                var raycastTries = 0;
                var inc = RaycastIncrement;
                while (raycastTries < MaximumObstacleRaycasts)
                {
                    var up = new Vector3(forward.x, forward.y + inc, forward.z - inc);
                    if (!Physics.Raycast(origin, up, Manager.ColliderAvoidance.RaycastDistance, Manager.ColliderAvoidance.Layers))
                        return new Vector3(forward.x, forward.y + inc * 2, forward.z - inc * 2);
                    var right = new Vector3(forward.x + inc, forward.y, forward.z - inc);
                    if (!Physics.Raycast(origin, right, Manager.ColliderAvoidance.RaycastDistance, Manager.ColliderAvoidance.Layers))
                        return new Vector3(forward.x + inc * 2, forward.y, forward.z - inc * 2);
                    var down = new Vector3(forward.x, forward.y - inc, forward.z - inc);
                    if (!Physics.Raycast(origin, down, Manager.ColliderAvoidance.RaycastDistance, Manager.ColliderAvoidance.Layers))
                        return new Vector3(forward.x, forward.y - inc * 2, forward.z - inc * 2);
                    var left = new Vector3(forward.x - inc, forward.y, forward.z - inc);
                    if (!Physics.Raycast(origin, left, Manager.ColliderAvoidance.RaycastDistance, Manager.ColliderAvoidance.Layers))
                        return new Vector3(forward.x - inc * 2, forward.y, forward.z - inc * 2);
                    inc += RaycastIncrement;
                    raycastTries++;
                }
                return new Vector3(forward.x, forward.y + inc * 2, forward.z - inc * 2);
            }
            return Vector3.zero;
        }
        
        private void MoveForward()
        {
            if (!Manager) return;

            Vector3 velocity = transform.forward * Manager.speed + _finalForce * Time.deltaTime;
            velocity = velocity.normalized * (Manager.speed * Time.deltaTime);

            switch (Manager.movementSpace)
            {
                case Space.Self:
                    transform.localPosition += velocity;
                    break;
                case Space.World:
                    transform.position += velocity;
                    break;
            }

            if (velocity != Vector3.zero) transform.rotation = Quaternion.LookRotation(velocity);
        }
    }
}