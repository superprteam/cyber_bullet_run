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
        private ReactiveCommand MoveEnd;


        public Character(CharacterData data) {
            Data = data;
        }

        public void Init(IController controller, CharacterView characterView) {
            _controller = controller;
            _сharacterView = characterView;
            
            MoveTo = new ReactiveCommand<CharacterView.MoveTo>();
            MoveEnd = new ReactiveCommand(); 

            _controller.SetCommands(MoveTo, MoveEnd);
            _сharacterView.Init(new CharacterView.CharacterViewCtx() {
                Data = Data,
                MoveTo = MoveTo,
                MoveEnd = MoveEnd,
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