using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Shared.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace CyberBulletRun.Game.View 
{
    public class Screen : BaseWindow
    {
        [SerializeField] private Button _hideButton;
        [SerializeField] private TMP_Text _scoreWin;
        [SerializeField] private TMP_Text _scoreFail;
        [SerializeField] private GameObject _winPanel;
        [SerializeField] private GameObject _failPanel;

        private ReactiveCommand<EndGameData> _endGame;
        private ReactiveCommand<int> _keyPressed;
        private bool _isEndGame = false;

        public void Start() {
            
        }
        public override void SetOnHide(Action onHideCallback) {
            base.SetOnHide(onHideCallback);
            _hideButton.onClick.RemoveAllListeners();
            _hideButton.onClick.AddListener(OnHideButtonClick);
        }

        private void OnHideButtonClick() {
            _onHide?.Invoke();
        }

        private void ShowWinPanel(EndGameData endGameData) {
            _winPanel.SetActive(true);
            _scoreWin.text = "Score: " + endGameData.Score;
        }
        private void ShowFailPanel(EndGameData endGameData) {
            _failPanel.SetActive(true);
            _scoreFail.text = "Score: " + endGameData.Score;
        }

        public void SetCommands(ReactiveCommand<EndGameData> endGame, ReactiveCommand<int> keyPressed) {
            _endGame = endGame;
            _endGame.Subscribe(async (endGameData) => await OnEndGameChange(endGameData));
            _keyPressed = keyPressed;
        }

        private async UniTask OnEndGameChange(EndGameData endGameData) {
            _isEndGame = true;
            if (endGameData.IsWin) {
                ShowWinPanel(endGameData);
            } else {
                ShowFailPanel(endGameData);
            }
        }

        private void Update() {
            if (Input.GetMouseButtonDown(0)) {
                if (!_isEndGame) {
                    _keyPressed.Execute(0);
                }
            }
        }
    }
}

