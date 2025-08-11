using System;
using Cysharp.Threading.Tasks;
using Shared.Disposable;
using UnityEngine;

namespace Shared.UI {
    public interface IWindow {
        public void ShowImmediate();
        public void HideImmediate();
        public UniTask Show();
        public UniTask Hide();
        public void SetOnHideCallback(Action onHideCallback);
        public void Release();
    }
}
