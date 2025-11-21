using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace CyberBulletRun.Game {
    public class CharacterView : MonoBehaviour, IDisposable {

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private GameObject _body;
        [SerializeField] private GameObject _weapon;
        [SerializeField] private Transform _weaponFire;
        public struct CharacterViewCtx {
            public CharacterDataRealtime DataRealtime;
            public ReactiveCommand<MoveTo> MoveTo;
            public ReactiveCommand MoveEnd;
            public ReactiveCommand<Vector3> TargetPos;
            public ReactiveProperty<Transform> WeaponFire;
        }

        public struct MoveTo {
            public Vector3 Position;
            public bool IsAnimate;
        }

        private CharacterViewCtx _ctx;
        private bool _isMoving;
        private Vector3 _targetPos;
        private Sequence _weaponMove;
        private Vector3 _weaponLocalPosition;
        
        public void Init(CharacterViewCtx ctx) {
            _agent.updateRotation = false;
            _ctx = ctx;
            _ctx.MoveTo?.Subscribe(async (moveTo) => await OnMoveTo(moveTo));
            _ctx.TargetPos?.Subscribe(async (targetPos) => await OnTargetPos(targetPos));
            _isMoving = false;
            _ctx.WeaponFire.Value = _weaponFire;
            _weaponLocalPosition = _weapon.transform.localPosition;
        }

        private async UniTask OnMoveTo(MoveTo moveTo) {
            _isMoving = true;
            _weaponMove.Kill(true);
            _weaponMove = null;
            if (moveTo.IsAnimate) {
                _agent.enabled = true;
                _agent.SetDestination(moveTo.Position);
                Debug.Log("SetDestination: " + moveTo.Position);
            } else {
                transform.position = moveTo.Position;
                _agent.enabled = true;
                _agent.Warp(moveTo.Position);
                Debug.Log("Warp: " + moveTo.Position);
            }
        }

        private async UniTask OnTargetPos(Vector3 targetPos) {
            _targetPos = targetPos;
        }
        
        async void Update() {
            
            var rot = transform.rotation.eulerAngles;
            rot.x = 0f;
            rot.z = 0f;
            transform.rotation = Quaternion.Euler(rot);
            
            if (!_isMoving) {
                if (_weaponMove == null && _ctx.DataRealtime.Weapon != null && !_ctx.DataRealtime.IsEnemy) {
                    
                    Vector3 directionBody = _targetPos - _body.transform.position;
                    directionBody.y = 0f;
                    Quaternion targetRotation = Quaternion.LookRotation(directionBody);
                    _body.transform.rotation = targetRotation;

                    //await UniTask.NextFrame();
                    
                    var directionWeapon = _weapon.transform.parent.InverseTransformPoint(_targetPos) - _weapon.transform.localPosition;
                    //var directionWeapon = _targetPos - _weapon.transform.position;
                    _weapon.transform.localRotation = Quaternion.LookRotation(directionWeapon, Vector3.up);
                    
                    var downRotation = _weapon.transform.localRotation *
                                       Quaternion.Euler(-_ctx.DataRealtime.Weapon.Radius, 0, 0);
                    var upRotation = _weapon.transform.localRotation *
                                       Quaternion.Euler(_ctx.DataRealtime.Weapon.Radius, 0, 0);

                    _weaponMove = DOTween.Sequence();
                    _weaponMove.Append(_weapon.transform
                        .DOLocalRotateQuaternion(downRotation, _ctx.DataRealtime.Weapon.RadiusSpeed / 2f).SetEase(Ease.Linear));
                    _weaponMove.Append(_weapon.transform.DOLocalRotateQuaternion(upRotation, _ctx.DataRealtime.Weapon.RadiusSpeed).SetEase(Ease.Linear)
                        .SetLoops(int.MaxValue, LoopType.Yoyo));
                }

                return;
            }
            if (HasArrivedOrFailed()) {
                _ctx.MoveEnd.Execute();
            }
            
            // body rotation
            Vector3 direction = _targetPos - _body.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _body.transform.rotation = targetRotation;
            }
            
            // weapon rotation
            direction = _weapon.transform.parent.InverseTransformPoint(_targetPos) - _weapon.transform.localPosition;
            if (direction.sqrMagnitude > 0.0001f)
            {
                _weapon.transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
            }
            
        }

        public bool HasArrivedOrFailed() {
            if (_agent == null) {
                return false;
            }
            if (!_agent.pathPending && _agent.isOnNavMesh)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f) {
                        _isMoving = false;
                        //_agent.isStopped = true;
                        //_agent.enabled = false;
                        return true;
                    }
                }
            }
            return false;
        }

        public Vector3 WeaponDirection() {
            return (_weapon.transform.rotation * Vector3.forward).normalized;
        }
        public void Dispose() {
            _weapon.transform.DOKill();
            if (_weaponMove != null) {
                _weaponMove.Kill();
            }
            Destroy(gameObject);
        }

    }
}
