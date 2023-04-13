using UnityEngine;

namespace SpellBoundAR.Boids
{
    public class Boid : MonoBehaviour
    {
        private const int MaximumObstacleRaycasts = 15;
        private const float RaycastIncrement = .15f;

        [SerializeField] private BoidManager manager;
        
        [Header("Cache")]
        private Transform _transform;
        private int _frameOffset;
        private int _lastUpdateFrame;
        private Vector3 _separationForce;
        private Vector3 _cohesionForce;
        private Vector3 _alignmentForce;
        private Vector3 _avoidWallsForce;
        private Vector3 _avoidObstacleForce;
        private Vector3 _finalForce;
        private Vector3 _velocity;
        
        public BoidManager Manager
        {
            get => manager;
            private set
            {
                if (manager == value) return;
                if (manager) manager.UnregisterBoid(this);
                manager = value;
                if (manager && enabled) manager.RegisterBoid(this);
            }
        }

        private void Awake()
        {
            _transform = transform;
            _frameOffset = Manager ? Random.Range(0, Manager.framesBetweenForceUpdates) : 0;
        }

        public void Initialize(BoidManager newManager)
        {
            Manager = newManager;
            _frameOffset = Manager ? Random.Range(0, Manager.framesBetweenForceUpdates) : 0;
        }

        private void OnEnable()
        {
            if (Manager) Manager.RegisterBoid(this);
        }

        private void OnDisable()
        {
            if (Manager) Manager.UnregisterBoid(this);
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
            
            Vector3 myPosition = _transform.position;
            Vector3 separationSum = Vector3.zero;
            Vector3 cohesionSum = Vector3.zero;
            Vector3 alignmentSum = Vector3.zero;

            int boidsNearby = 0;

            foreach (Boid otherBoid in Manager.Boids)
            {
                if (!otherBoid || !otherBoid.enabled || otherBoid == this) continue;
                Vector3 otherBoidPosition = otherBoid._transform.position;
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
                _separationForce = separationSum / boidsNearby;
                _cohesionForce = cohesionSum / boidsNearby - myPosition;
                _alignmentForce = alignmentSum / boidsNearby;
            }
            else
            {
                _separationForce = Vector3.zero;
                _cohesionForce = Vector3.zero;
                _alignmentForce = Vector3.zero;
            }
            
            if (Manager.ContainerAvoidance.Enabled
                && Manager.ContainerAvoidance.Container
                && !Manager.ContainerAvoidance.Container.WorldPositionIsInContainer(myPosition))
            {
                Vector3 closestPointOnCage = Manager.ContainerAvoidance.Container.ClosestPointInOrOnContainer(myPosition);
                _avoidWallsForce = (closestPointOnCage - myPosition).normalized;
            }
            else _avoidWallsForce = Vector3.zero;

            if (Manager.ColliderAvoidance.Enabled)
            {
                _avoidObstacleForce = CheckForObstacle(myPosition, _transform.forward);
            }
            
            _finalForce = 
                _separationForce * Manager.separationWeight +
                _cohesionForce * Manager.cohesionWeight +
                _alignmentForce * Manager.alignmentWeight +
                _avoidWallsForce * Manager.ContainerAvoidance.Weight +
                _avoidObstacleForce * Manager.ColliderAvoidance.Weight;
        }

        private void MoveForward()
        {
            if (!Manager) return;

            _velocity = transform.forward * Manager.speed + _finalForce * Time.deltaTime;
            _velocity = _velocity.normalized * (Manager.speed * Time.deltaTime);

            switch (Manager.movementSpace)
            {
                case Space.Self:
                    transform.localPosition += _velocity;
                    break;
                case Space.World:
                    transform.position += _velocity;
                    break;
            }

            if (_velocity != Vector3.zero) transform.rotation = Quaternion.LookRotation(_velocity);
        }

        private Vector3 CheckForObstacle(Vector3 pos, Vector3 direction)
        {
            if (!Manager) return Vector3.zero;

            var layer = Manager.ColliderAvoidance.Layers;
            if (Physics.Raycast(pos, direction, out RaycastHit hit, Manager.ColliderAvoidance.RaycastDistance))
            {
                var raycastTries = 0;
                var inc = RaycastIncrement;
                while (raycastTries < MaximumObstacleRaycasts)
                {
                    var up = new Vector3(direction.x, direction.y + inc, direction.z - inc);
                    if (!Physics.Raycast(pos, up, Manager.ColliderAvoidance.RaycastDistance))
                        return new Vector3(direction.x, direction.y + inc * 2, direction.z - inc * 2);
                    var right = new Vector3(direction.x + inc, direction.y, direction.z - inc);
                    if (!Physics.Raycast(pos, right, Manager.ColliderAvoidance.RaycastDistance))
                        return new Vector3(direction.x + inc * 2, direction.y, direction.z - inc * 2);
                    var down = new Vector3(direction.x, direction.y - inc, direction.z - inc);
                    if (!Physics.Raycast(pos, down, Manager.ColliderAvoidance.RaycastDistance))
                        return new Vector3(direction.x, direction.y - inc * 2, direction.z - inc * 2);
                    var left = new Vector3(direction.x - inc, direction.y, direction.z - inc);
                    if (!Physics.Raycast(pos, left, Manager.ColliderAvoidance.RaycastDistance))
                        return new Vector3(direction.x - inc * 2, direction.y, direction.z - inc * 2);
                    inc += RaycastIncrement;
                    raycastTries++;
                }

                return new Vector3(direction.x, direction.y + inc * 2, direction.z - inc * 2);
            }
            return Vector3.zero;
        }
    }
}