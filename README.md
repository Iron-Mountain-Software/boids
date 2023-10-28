# Boids
Version: 1.3.4
A library for spawning and using boids.

## Use Cases:
* Making boids with flocking behaviour
## Package Mirrors:
[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODg3LnBuZw==/original/npRUfq.png'>](https://github.com/Iron-Mountain-Software/boids)[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODkyLnBuZw==/original/Fq0ORM.png'>](https://www.npmjs.com/package/com.iron-mountain.boids)[<img src='https://img.itch.zone/aW1nLzEzNzQ2ODk4LnBuZw==/original/Rv4m96.png'>](https://iron-mountain.itch.io/boids)
## Key Scripts & Components:
1. public class **Boid** : MonoBehaviour
   * Properties: 
      * public BoidManager ***Manager***  { get; }
      * public Boolean ***OverrideManagerSpeed***  { get; set; }
      * public float ***InstanceSpeed***  { get; set; }
   * Methods: 
      * public void ***Initialize***(BoidManager newManager)
1. public class **BoidManager** : MonoBehaviour
   * Actions: 
      * public event Action ***OnBoidsChanged*** 
   * Properties: 
      * public ContainerAvoidanceSettings ***ContainerAvoidance***  { get; }
      * public ColliderAvoidanceSettings ***ColliderAvoidance***  { get; }
      * public List<Boid> ***Boids***  { get; }
   * Methods: 
      * public void ***Register***(Boid boid)
      * public void ***Unregister***(Boid boid)
      * public void ***DestroyBoids***()
1. public class **BoidSpawner** : MonoBehaviour
   * Properties: 
      * public BoidManager ***Manager***  { get; set; }
      * public Boid ***Prefab***  { get; set; }
      * public Int32 ***Amount***  { get; set; }
   * Methods: 
      * public virtual List`1 ***SpawnBoids***()
1. public interface **IBoidSpawner**
   * Properties: 
      * public BoidManager ***Manager***  { get; set; }
   * Methods: 
      * public abstract List`1 ***SpawnBoids***()
### Containers
1. public abstract class **BoidContainer** : MonoBehaviour
   * Properties: 
      * public Vector3 ***Offset***  { get; set; }
      * public Vector3 ***Position***  { get; set; }
      * public Vector3 ***Scale***  { get; }
   * Methods: 
      * public abstract Vector3 ***GetRandomWorldPositionInContainer***()
      * public abstract Boolean ***WorldPositionIsInContainer***(Vector3 worldPosition)
      * public abstract Vector3 ***ClosestPointInOrOnContainer***(Vector3 worldPosition)
1. public class **BoidContainerCube** : BoidContainer
   * Properties: 
      * public Vector3 ***Dimensions***  { get; set; }
   * Methods: 
      * public override Vector3 ***GetRandomWorldPositionInContainer***()
      * public override Boolean ***WorldPositionIsInContainer***(Vector3 worldPosition)
      * public override Vector3 ***ClosestPointInOrOnContainer***(Vector3 worldPosition)
1. public class **BoidContainerHemisphere** : BoidContainer
   * Methods: 
      * public override Vector3 ***GetRandomWorldPositionInContainer***()
      * public override Boolean ***WorldPositionIsInContainer***(Vector3 worldPosition)
      * public override Vector3 ***ClosestPointInOrOnContainer***(Vector3 worldPosition)
1. public class **BoidContainerSphere** : BoidContainer
   * Methods: 
      * public override Vector3 ***GetRandomWorldPositionInContainer***()
      * public override Boolean ***WorldPositionIsInContainer***(Vector3 worldPosition)
      * public override Vector3 ***ClosestPointInOrOnContainer***(Vector3 worldPosition)
