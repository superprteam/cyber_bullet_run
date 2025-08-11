using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Shared.UI {
    public abstract class BaseWindow : MonoBehaviour, IWindow {

        [SerializeField] protected CanvasGroup _canvasGroup;
        
        private Action _onHide;
        
        public virtual void ShowImmediate() {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true);
        }

        public virtual void HideImmediate() {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
            _onHide?.Invoke();
        }

        public virtual async UniTask Show() {
            _canvasGroup.alpha = 1f;
            _canvasGroup.gameObject.SetActive(true);
        }

        public virtual async UniTask Hide() {
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
            _onHide?.Invoke();
        }

        public virtual void SetOnHideCallback(Action onHideCallback) {
            _onHide = onHideCallback;
        }

        public virtual void Release() {
            GameObject.Destroy(gameObject);
        }
        
    }
}
