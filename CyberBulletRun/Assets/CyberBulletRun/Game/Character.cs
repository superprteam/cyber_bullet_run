using CyberBulletRun.Game;
using CyberBulletRun.Game.View;
using UniRx;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace CyberBulletRun.Game {
    public class Character {
        public CharacterData Data;
        
        private IController _controller;
        private CharacterView _сharacterView;
        private ReactiveCommand<CharacterView.MoveTo> MoveTo;
        private ReactiveCommand<Vector3> TargetPos;
        private ReactiveCommand MoveEnd;


        public Character(CharacterData data) {
            Data = data;
        }

        public void Init(IController controller, CharacterView characterView) {
            _controller = controller;
            _сharacterView = characterView;
            
            MoveTo = new ReactiveCommand<CharacterView.MoveTo>();
            TargetPos = new ReactiveCommand<Vector3>();
            MoveEnd = new ReactiveCommand(); 

            _controller.SetCommands(MoveTo, MoveEnd, TargetPos);
            _сharacterView.Init(new CharacterView.CharacterViewCtx() {
                Data = Data,
                MoveTo = MoveTo,
                MoveEnd = MoveEnd,
                TargetPos = TargetPos, 
            });
        }
        
        void Update() {
            _controller?.Update();
        }

        public void TakeDamage(int amount) {
            /* ... */
        }
    }
}