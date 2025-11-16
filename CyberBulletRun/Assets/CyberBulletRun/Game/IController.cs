using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game {
    public interface IController {
        void Update();
        void SetCommands(ReactiveCommand<CharacterView.MoveTo> moveTo, ReactiveCommand moveEnd,
                         ReactiveCommand<Vector3> targetPos, ReactiveCommand<Shot> shooting, ReactiveProperty<Transform> weaponFire);

        void SetPos(Vector3 position, bool isAnimate);
        void SetTarget(Vector3 position);
        void SetCharacter(Character character);
        void Shot();
        void EndGame(EndGameData endGameData);
    }
}
