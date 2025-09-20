using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game.Controllers {
    public class AIController : IController {
        public void Update() { 
        }

        public void SetCommands(ReactiveCommand<CharacterView.MoveTo> moveTo, ReactiveCommand moveEnd, ReactiveCommand<Vector3> targetPos) {
            
        }
    }
}
