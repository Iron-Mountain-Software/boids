using UnityEditor;
using UnityEngine;

namespace SpellBoundAR.Boids.Editor
{
    [CustomEditor(typeof(BoidSpawner), true)]
    public class BoidSpawnerInspector : UnityEditor.Editor
    {
        [Header("Cache")]
        private BoidSpawner _boidSpawner;
        
        private void OnEnable()
        {
            _boidSpawner = (BoidSpawner) target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Spawn")) _boidSpawner.SpawnBoids();
        }
    }
}
