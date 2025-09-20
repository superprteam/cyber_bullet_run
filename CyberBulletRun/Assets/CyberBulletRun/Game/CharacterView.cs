using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace CyberBulletRun.Game {
    public class CharacterView : MonoBehaviour {

        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private GameObject _root;
        public struct CharacterViewCtx {
            public CharacterData Data;
            public ReactiveCommand<MoveTo> MoveTo;
            public ReactiveCommand MoveEnd;
            public ReactiveCommand<Vector3> TargetPos;
        }

        public struct MoveTo {
            public Vector3 Position;
            public bool IsAnimate;
        }

        private CharacterViewCtx _ctx;
        private bool _isMoving;
        private Vector3 _targetPos;
        
        public void Init(CharacterViewCtx ctx) {
            _ctx = ctx;
            _ctx.MoveTo?.Subscribe(async (moveTo) => await OnMoveTo(moveTo));
            _ctx.TargetPos?.Subscribe(async (targetPos) => await OnTargetPos(targetPos));
            _isMoving = false;
        }

        private async UniTask OnMoveTo(MoveTo moveTo) {
            _isMoving = true;
            if (moveTo.IsAnimate) {
                _agent.SetDestination(moveTo.Position);
                Debug.Log("SetDestination: " + moveTo.Position);
            } else {
                _agent.Warp(moveTo.Position);
                Debug.Log("Warp: " + moveTo.Position);
            }
        }

        private async UniTask OnTargetPos(Vector3 targetPos) {
            _targetPos = targetPos;
        }
        
        void Update() {
            if (!_isMoving) {
                return;
            }
            if (HasArrivedOrFailed()) {
                _ctx.MoveEnd.Execute();
            }
            Vector3 direction = _targetPos - _root.transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.0001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                _root.transform.rotation = targetRotation;
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
                        return true;
                    }
                }
            }
            return false;
        }        
    }
}
