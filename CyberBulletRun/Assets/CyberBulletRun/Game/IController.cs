using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game {
    public interface IController {
        void Update();
        void SetCommands(ReactiveCommand<CharacterView.MoveTo> moveTo, ReactiveCommand moveEnd, ReactiveCommand<Vector3> targetPos);
    }
}
