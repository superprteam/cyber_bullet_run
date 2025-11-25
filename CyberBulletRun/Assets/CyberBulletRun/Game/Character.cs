using System;
using System.Collections.Generic;
using CyberBulletRun.DataSet;
using CyberBulletRun.Game.Controllers;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.Disposable;
using Shared.LocalCache;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game {
    public class Character : BaseDisposable {

        public enum CharacterState {
            IDLE = 0,
            MOVE = 1,
        }
        
        public CharacterDataRealtime DataRealtime;
        public CharacterState CurrentState;
        
        private IController _controller;
        private CharacterView _сharacterView;
        private ReactiveCommand<CharacterView.MoveTo> MoveTo;
        private ReactiveCommand<Vector3> TargetPos;
        private ReactiveCommand MoveEnd;
        private ReactiveCommand<Shot> Shooting;
        private ReactiveProperty<Transform> WeaponFire;
        private ReactiveCommand<Shot> ShotSpawn;
        private ReactiveCommand NextStair;
        private ReactiveCommand<ShotSpawner.ShotCollision> ShotCollision;

        public Character(CharacterDataRealtime dataRealtime) {
            DataRealtime = dataRealtime;
            CurrentState = CharacterState.IDLE;
        }

        public async UniTask Init(IController controller, CharacterView characterView,
                                  ReactiveCommand<Shot> shotSpawn, ReactiveCommand nextStair, ReactiveCommand<ShotSpawner.ShotCollision> shotCollision) {
            _controller = controller;
            _сharacterView = characterView;
            ShotSpawn = shotSpawn;
            NextStair = nextStair;
            ShotCollision = shotCollision;

            CurrentState = CharacterState.IDLE;
            
            MoveTo = new ReactiveCommand<CharacterView.MoveTo>().AddTo(this);
            TargetPos = new ReactiveCommand<Vector3>().AddTo(this);
            MoveEnd = new ReactiveCommand().AddTo(this);
            Shooting = new ReactiveCommand<Shot>().AddTo(this);
            WeaponFire = new ReactiveProperty<Transform>().AddTo(this);

            MoveTo.Subscribe(async (moveTo) => await OnMoveTo(moveTo));
            MoveEnd.Subscribe(async (Unit) => await OnMoveEnd());
            Shooting.Subscribe(async (shot) => await OnShooting(shot));
            ShotCollision.Subscribe(async (shotCollision) => await OnShotCollision(shotCollision));
            
            _controller.SetCommands(MoveTo, MoveEnd, TargetPos, Shooting, WeaponFire);
            _controller.SetCharacter(this);
            _сharacterView.Init(new CharacterView.CharacterViewCtx() {
                DataRealtime = DataRealtime,
                MoveTo = MoveTo,
                MoveEnd = MoveEnd,
                TargetPos = TargetPos, 
                WeaponFire = WeaponFire,
            });
        }

        private async UniTask<Shot> OnShooting(Shot shot) {
            shot.Weapon = DataRealtime.Weapon;
            ShotSpawn.Execute(shot);
            return default;
        }

        private async UniTask<Unit> OnMoveEnd() {
            CurrentState = CharacterState.IDLE;
            return default;
        }

        private async UniTask OnMoveTo(CharacterView.MoveTo moveTo) {
            CurrentState = CharacterState.MOVE;
        }

        public Vector3 WeaponDirection() {
            return _сharacterView.WeaponDirection();
        }
        private async UniTask OnShotCollision(ShotSpawner.ShotCollision shotCollision) {
            if (shotCollision.Collider != null && shotCollision.Collider.gameObject.CompareTag("Character")) {
                if (shotCollision.Collider.gameObject.transform.parent.GetComponent<CharacterView>() == _сharacterView) {
                    DataRealtime.HP -= shotCollision.Shot.Weapon.Damage;
                    if (DataRealtime.HP > 0 && DataRealtime.IsEnemy) {
                        ((AIController)_controller).Retreat();
                    }
                    if (DataRealtime.HP <= 0 && !DataRealtime.IsEnemy) {
                        _controller.EndGame(new EndGameData() {
                            IsWin = false,
                        });
                    }
                    NextStair.Execute();
                }
            } else {
                if (shotCollision.IsLastCollision && DataRealtime.IsEnemy) {
                    Debug.Log("Enemy shot to player");
                    _controller.Shot();
                }
            }
        }
        
        public void Dispose() {
            _сharacterView.Dispose();
        }
    }
}