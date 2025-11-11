using System;
using System.Collections.Generic;
using CyberBulletRun.DataSet;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.LocalCache;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game {
    public class Character : IDisposable {

        public enum CharacterState {
            IDLE = 0,
            MOVE = 1,
        }
        
        public CharacterData Data;
        public CharacterState CurrentState;
        
        private IController _controller;
        private CharacterView _сharacterView;
        private ReactiveCommand<CharacterView.MoveTo> MoveTo;
        private ReactiveCommand<Vector3> TargetPos;
        private ReactiveCommand MoveEnd;
        private ReactiveCommand<Shot> Shooting;
        private WeaponData _currentWeapon;
        private ReactiveProperty<Transform> WeaponFire;
        private ReactiveCommand<Shot> _shotSpawn;


        public Character(CharacterData data) {
            Data = data;
            CurrentState = CharacterState.IDLE;
        }

        public async UniTask Init(IController controller, CharacterView characterView,
                                  ReactiveCommand<Shot> shotSpawn) {
            _controller = controller;
            _сharacterView = characterView;
            _shotSpawn = shotSpawn;

            await Load();
            
            CurrentState = CharacterState.IDLE;
            
            MoveTo = new ReactiveCommand<CharacterView.MoveTo>();
            TargetPos = new ReactiveCommand<Vector3>();
            MoveEnd = new ReactiveCommand();
            Shooting = new ReactiveCommand<Shot>();
            WeaponFire = new ReactiveProperty<Transform>();

            MoveTo.Subscribe(async (moveTo) => await OnMoveTo(moveTo));
            MoveEnd.Subscribe(async (Unit) => await OnMoveEnd());
            Shooting.Subscribe(async (shot) => await OnShooting(shot));
            
            _controller.SetCommands(MoveTo, MoveEnd, TargetPos, Shooting, WeaponFire);
            _сharacterView.Init(new CharacterView.CharacterViewCtx() {
                Data = Data,
                MoveTo = MoveTo,
                MoveEnd = MoveEnd,
                TargetPos = TargetPos, 
                WeaponFire = WeaponFire,
            });
        }

        private async UniTask Load() {
            var path = $"DataSet/weapon.json";
            var weaponText = await Cacher.GetTextAsync(path);
            var weapons = JsonConvert.DeserializeObject<List<WeaponData>>(weaponText);
            _currentWeapon = weapons.Find(w => w.Id == Data.WeaponId);
            Data.Weapon = _currentWeapon;
        }

        private async UniTask<Shot> OnShooting(Shot shot) {
            shot.Speed = _currentWeapon.Speed;
            Debug.Log("ShotSpeed: " + shot.Speed);
            _shotSpawn.Execute(shot);
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
        public void TakeDamage(int amount) {
            /* ... */
        }

        public void Dispose() {
            _сharacterView.Dispose();
        }
    }
}