using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CyberBulletRun.Options.View 
{
    public interface IScreen {
        public void Init(Action onHideClick);
        public void ShowImmediate();
        public void HideImmediate();
        public UniTask Show();
        public UniTask Hide();
        public void Release();
    }

    public class Screen : MonoBehaviour, IScreen
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Button _hideButton;

        private Action _onHide;

        public void Init(Action onHideClick) {
            _onHide = onHideClick;
            _hideButton.onClick.RemoveAllListeners();
            _hideButton.onClick.AddListener(OnHideButtonClick);
        }
        
        public void ShowImmediate()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true);
        }

        public async UniTask Show()
        {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true);
        }

        public void HideImmediate()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
        }

        public async UniTask Hide()
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
            _onHide?.Invoke();
        }

        public void Release() 
        {
            GameObject.Destroy(gameObject);
        }
        
        private async void OnHideButtonClick() {
            await Hide();
            _onHide?.Invoke();
        }
        
    }
}

