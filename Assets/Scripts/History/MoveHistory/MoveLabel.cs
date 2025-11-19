using Board.Display.Moves;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace History.MoveHistory
{
    public class MoveLabel : MonoBehaviour
    {
        [SerializeField] Color failedColor;
        [SerializeField] Color normalColor;
        [SerializeField] RawImage _backGround;

        Action _callback;
        public TextMeshProUGUI label;

        public void SetCallback(Action callback)
        {
            _callback = callback;
        }

        public void SetMove(string move)
        {
            label.text = move.ToString();
            this.name = move.ToString();
        }

        public void OnClick()
        {
            _callback();
        }

        public void SetColorToFailed()
        {
            _backGround.color = failedColor;
        }

        public void SetColorToNormal()
        {
            _backGround.color = normalColor;
        }
    }
}