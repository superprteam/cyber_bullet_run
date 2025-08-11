using Cysharp.Threading.Tasks;
using Shared.UI;
using UnityEngine;

namespace CyberBulletRun.Loading.View 
{
    public class Screen : BaseWindow
    {
        [SerializeField] private float _showHideDuration;
        [SerializeField] private Transform _spinnerView;
        [SerializeField] private float _rotateSpeed;

        private void Update()
        {
            _spinnerView.rotation *= Quaternion.Euler(_rotateSpeed * Time.deltaTime * Vector3.forward);
        }

        public override async UniTask Show()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(true);

            var delayMs = 50;
            var deltaTime = delayMs / 1000f;

            var timer = _showHideDuration;
            while (timer >= 0f)
            {
                _canvasGroup.alpha = 1f - (timer / _showHideDuration);
                timer -= deltaTime;
                await UniTask.Delay(delayMs, true);
            }

            _canvasGroup.alpha = 1f;
        }

        public override async UniTask Hide()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true);

            var delayMs = 50;
            var deltaTime = delayMs / 1000f;

            var timer = _showHideDuration;
            while (timer >= 0f)
            {
                _canvasGroup.alpha = timer / _showHideDuration;
                timer -= deltaTime;
                await UniTask.Delay(delayMs, true);
            }

            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
        }
    }
}

