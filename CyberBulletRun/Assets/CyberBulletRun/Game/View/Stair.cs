using System;
using Unity.AI.Navigation;
using UnityEngine;

namespace CyberBulletRun.Game.View {
    public class Stair : MonoBehaviour, IDisposable {
        [SerializeField] private Transform _linkPointDown;
        [SerializeField] private Transform _linkPointUp;
        [SerializeField] private Transform _stopPoint;
        [SerializeField] private Transform _enemyPoint;
        [SerializeField] private Transform _enemySpawnPoint;
        [SerializeField] private Transform _cameraPos;
        [SerializeField] private NavMeshSurface[] _navMeshSurfaces;

        public Transform LinkPointDown => _linkPointDown;
        public Transform LinkPointUp => _linkPointUp;
        public Transform StopPoint => _stopPoint;
        public Transform EnemyPoint => _enemyPoint;
        public Transform EnemySpawnPoint => _enemySpawnPoint;
        public Transform CameraPos => _cameraPos;
        public NavMeshSurface[] NavMeshSurfaces => _navMeshSurfaces;

        public void Dispose() {
            Destroy(gameObject);
        }
    }
}
