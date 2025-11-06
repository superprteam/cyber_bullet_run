using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberBulletRun.Game.View {
    public class Stair : MonoBehaviour, IDisposable {
        [SerializeField] private Transform _linkPointDown;
        [SerializeField] private Transform _linkPointUp;
        [SerializeField] private Transform _stopPoint;
        [SerializeField] private Transform _enemyPoint;
        [SerializeField] private Transform _enemySpawnPoint;
        [SerializeField] private Transform _cameraPosLeft;
        [SerializeField] private Transform _cameraPosRight;
        [SerializeField] private NavMeshSurface[] _navMeshSurfaces;

        public Transform LinkPointDown => _linkPointDown;
        public Transform LinkPointUp => _linkPointUp;
        public Transform StopPoint => _stopPoint;
        public Transform EnemyPoint => _enemyPoint;
        public Transform EnemySpawnPoint => _enemySpawnPoint;
        public Transform CameraPosLeft => _cameraPosLeft;
        public Transform CameraPosRight => _cameraPosRight;
        public NavMeshSurface[] NavMeshSurfaces => _navMeshSurfaces;

        public bool IsLeft;

        public void Dispose() {
            Destroy(gameObject);
        }

        public Vector3 GetCameraPos() {
            return IsLeft ? _cameraPosLeft.position : _cameraPosRight.position;
        }
    }
}
