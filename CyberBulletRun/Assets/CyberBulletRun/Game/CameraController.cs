using CyberBulletRun.Game.View;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Shared.Disposable;
using UniRx;
using UnityEngine;

namespace CyberBulletRun.Game
{
    internal class CameraController : BaseDisposable
    {
        public struct Ctx
        {
            public GameObject Root;
            public ReactiveProperty<Stair> CurrentStair;
            public Camera CameraScreen;
        }

        private readonly Ctx _ctx;

        private Sequence _moveCameraSequence;


        public CameraController(Ctx ctx)
        {
            _ctx = ctx;
            _ctx.CurrentStair.Subscribe(async (stair) => await OnChangeCurrentStair(stair));
        }

        private async UniTask OnChangeCurrentStair(Stair stair) {
            if (stair == null) {
                return;
            }
            
            _moveCameraSequence?.Kill();

            var cameraPos = stair.GetCameraPos(); 
            
            Vector3 direction = stair.LinkPointUp.position - cameraPos;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _moveCameraSequence = DOTween.Sequence();
            _moveCameraSequence.Append(_ctx.CameraScreen.transform.DOMove(cameraPos, 0.5f));
            _moveCameraSequence.Join(_ctx.CameraScreen.transform.DORotateQuaternion(targetRotation, 0.5f));
            _moveCameraSequence.Play();
        }
    }
}
