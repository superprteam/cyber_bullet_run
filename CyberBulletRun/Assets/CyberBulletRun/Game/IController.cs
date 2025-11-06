using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game {
    public interface IController {
        void Update();
        void SetCommands(ReactiveCommand<CharacterView.MoveTo> moveTo, ReactiveCommand moveEnd,
                         ReactiveCommand<Vector3> targetPos, ReactiveCommand<Shot> shooting, ReactiveProperty<Transform> weaponFire);

        public void SetPos(Vector3 position, bool isAnimate);
        public void SetTarget(Vector3 position);
    }
}
