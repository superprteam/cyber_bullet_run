using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberBulletRun.Game.View;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game.Controllers {
    public class PlayerController : IController {
        private static readonly Vector3 HEART_UP = new Vector3(0, 0.3f, 0);

        public struct PlayerControllerCtx {
            public Character Character;
            public ReactiveProperty<Stair> CurrentStair;
            public List<Stair> Stairs;
        }

        private PlayerControllerCtx _ctx;
        private bool _noAnimation;
        private ReactiveCommand<CharacterView.MoveTo> _moveTo;
        private ReactiveCommand<Vector3> _targetPos;
        private ReactiveCommand<Unit> _moveEnd;
        private ReactiveCommand<Shot> _shooting;
        private ReactiveProperty<Transform> _weaponFire;

        public PlayerController(PlayerControllerCtx ctx) {
            _ctx = ctx;
            _ctx.CurrentStair.Subscribe(async (stair) => await OnChangeCurrentStair(stair));
            _noAnimation = true;
        }

        public void SetPos(Stair stair) {
            // without animations
            SetPos(stair.StopPoint.position, false);
        }

        public void SetPos(Vector3 position, bool isAnimate) {
            if (_noAnimation) {
                _noAnimation = false;
                isAnimate = false;
            }

            _moveTo?.Execute(new CharacterView.MoveTo() {
                Position = position,
                IsAnimate = isAnimate,
            });
        }

        public void SetTarget(Vector3 position) {
            _targetPos?.Execute(position + HEART_UP);
        }

        private async UniTask OnChangeCurrentStair(Stair stair) {
            // with animations
            if (stair == null) {
                return;
            }

            SetPos(stair.StopPoint.position, true);

            var stairIndex = _ctx.Stairs.IndexOf(stair);

            if (stairIndex + 1 < _ctx.Stairs.Count) {
                _targetPos?.Execute(_ctx.Stairs[stairIndex + 1].EnemyPoint.position + HEART_UP);
            }
        }

        public void Update() {
            if (Input.GetMouseButtonDown(0) && _ctx.Character.CurrentState == Character.CharacterState.IDLE) {
                var stairIndex = _ctx.Stairs.IndexOf(_ctx.CurrentStair.Value);

                var position = _weaponFire.Value.position;
                var shot = new Shot() {
                    StartPos = position,
                    //Direction = Vector3.Normalize(_ctx.Stairs[stairIndex + 1].EnemyPoint.position + HEART_UP - position)
                    Direction = _ctx.Character.WeaponDirection(),
                };
                _shooting.Execute(shot);
            }
        }

        public void SetCommands(ReactiveCommand<CharacterView.MoveTo> moveTo, ReactiveCommand moveEnd,
                                ReactiveCommand<Vector3> targetPos, ReactiveCommand<Shot> shooting,
                                ReactiveProperty<Transform> weaponFire) {
            _moveTo = moveTo;
            _moveEnd = moveEnd;
            _targetPos = targetPos;
            _shooting = shooting;
            _weaponFire = weaponFire;

            _moveEnd.Subscribe(async (Unit) => await OnMoveEnd());
        }

        private async UniTask<Unit> OnMoveEnd() {
            /*
            await UniTask.WaitForSeconds(1);
            for(int i=0; i < _ctx.Stairs.Count; i++) {
                if (_ctx.Stairs[i] == _ctx.CurrentStair.Value) {
                    if (i + 1 < _ctx.Stairs.Count) {
                        _ctx.CurrentStair.Value = _ctx.Stairs[i + 1];
                        Debug.Log("Current Stair: " + (i+1));

                        if (i + 2 < _ctx.Stairs.Count) {
                            _targetPos.Execute(_ctx.Stairs[i + 2].EnemyPoint.position);
                        }
                    }

                    break;
                }
            }
            */
            return default;
        }
    }
}