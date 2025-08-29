using System;
using UnityEngine;

namespace CyberBulletRun.Game.View {
    public class Stair : MonoBehaviour, IDisposable {
        [SerializeField] private Transform _linkPointDown;
        [SerializeField] private Transform _linkPointUp;
        [SerializeField] private Transform _cameraPos;

        public Transform LinkPointDown => _linkPointDown;
        public Transform LinkPointUp => _linkPointUp;
        public Transform CameraPos => _cameraPos;

        public void Dispose() {
            Destroy(gameObject);
        }
    }
}
