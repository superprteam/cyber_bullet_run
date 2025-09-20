using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CyberBulletRun.Game.View;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game.Controllers {
    public class PlayerController : IController {
        
        public struct PlayerControllerCtx {
            public CharacterData Data;
            public ReactiveProperty<Stair> CurrentStair;
            public List<Stair> Stairs;
        }

        private PlayerControllerCtx _ctx;
        private bool _noAnimation;
        private ReactiveCommand<CharacterView.MoveTo> _moveTo;
        private ReactiveCommand<Vector3> _targetPos;
        private ReactiveCommand<Unit> _moveEnd;
        
        public PlayerController(PlayerControllerCtx ctx) {
            _ctx = ctx;
            _ctx.CurrentStair.Subscribe(async (stair) => await OnChangeCurrentStair(stair));
            _noAnimation = true;
        }

        public void SetPos(Stair stair) { // without animations
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
        
        private async UniTask OnChangeCurrentStair(Stair stair) { // with animations
            if (stair == null) {
                return;
            }
            SetPos(stair.StopPoint.position, true);
        }

        public void Update() {
            
        }

        public void SetCommands(ReactiveCommand<CharacterView.MoveTo> moveTo, ReactiveCommand moveEnd, ReactiveCommand<Vector3> targetPos) {
            _moveTo = moveTo;
            _moveEnd = moveEnd;
            _targetPos = targetPos;
            
            _moveEnd.Subscribe(async (Unit) => await OnMoveEnd());
        }

        private async UniTask<Unit> OnMoveEnd() {
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
            return default;
        }
    }
}
