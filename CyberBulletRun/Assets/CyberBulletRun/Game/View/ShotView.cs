using System;
using Cysharp.Threading.Tasks;
using Shared.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CyberBulletRun.Game.View 
{
    public class ShotView : MonoBehaviour {

        public bool IsVisible;
        private Shot _shot;
        private float _destroyTimer;
        private Action<ShotView, Collider> _collisionCallback;
        public void Init(Shot shot, float destroyTimer, Action<ShotView, Collider> collisionCallback) {
            _shot = shot;
            _collisionCallback = collisionCallback;
            Reset(destroyTimer);
        }

        private void Reset(float destroyTimer) {
            transform.position = _shot.StartPos;
            _destroyTimer = destroyTimer;
            IsVisible = true;
            gameObject.SetActive(true);
        }

        public void UpdateInternal() {
            transform.position += _shot.Direction * (_shot.Weapon.Speed * Time.deltaTime);
            _destroyTimer -= Time.deltaTime;
            if (_destroyTimer < 0) {
                DisableView(null);
            }
        }

        public void DisableView(Collider collider) {
            IsVisible = false;
            gameObject.SetActive(false);
            _collisionCallback?.Invoke(this, collider);
        }
        
        public Shot GetShot() {
            return _shot;
        }
        
        void OnTriggerEnter(Collider collider) {
            Debug.Log("OnTriggerEnter: " + collider.gameObject.tag);
            DisableView(collider);
        }        
    }
}

