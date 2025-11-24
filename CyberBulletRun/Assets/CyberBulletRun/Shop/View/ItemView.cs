using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class ItemView : MonoBehaviour {
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _status;
    [SerializeField] private Button _button;

    private Action<ItemView> _onClickButton;
    private string _item;
    private void OnEnable() {
        _button.onClick.AddListener(OnClickButton);
    }
    private void OnDisable() {
        _button.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton() {
        _onClickButton?.Invoke(this);
    }

    public string GetItem() {
        return _item;
    }

    public void Init(string name, string status, Action<ItemView> onClickButton) {
        _onClickButton = onClickButton;
        _item = name;
        
        _name.text = name;
        _status.text = status;
    }
}
