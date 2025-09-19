using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace CyberBulletRun.Game {
    public class CharacterView : MonoBehaviour {

        [SerializeField] private NavMeshAgent _agent;
        public struct CharacterViewCtx {
            public CharacterData Data;
            public ReactiveCommand<MoveTo> MoveTo;
            public ReactiveCommand MoveEnd;
        }

        public struct MoveTo {
            public Vector3 Position;
            public bool IsAnimate;
        }

        private CharacterViewCtx _ctx;
        private bool _isMoving;
        
        public void Init(CharacterViewCtx ctx) {
            _ctx = ctx;
            _ctx.MoveTo?.Subscribe(async (moveTo) => await OnMoveTo(moveTo));
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

        void Update() {
            if (!_isMoving) {
                return;
            }
            if (HasArrivedOrFailed()) {
                _ctx.MoveEnd.Execute();
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
