using Board.Moves;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UI;
using Trainer.Data.Moves;

namespace Trainer.MoveViewer
{
    public class TrainerMoveInformationDisplay : MonoBehaviour
    {
        public Color BaseColor;
        public Color WhiteColor;
        public Color BlackColor;

        public RawImage Background;
        public TextMeshProUGUI MoveNameText;
        public TextMeshProUGUI MoveDescriptionText;
        public PercentageBar PercentageBar;

        public TextMeshProUGUI MovePercentageText;
        public TextMeshProUGUI TotalMovePercentageText;

        Action<TrainerMoveInformation> _callBack = null;
        TrainerMoveInformation _moveInformation;

        public void Init(TrainerMoveInformation moveInformation)
        {
            _moveInformation = moveInformation;
            MoveNameText.text = moveInformation.MoveNotation;
            MoveDescriptionText.text = moveInformation.MoveDescription?.Trim() ?? "";
            PercentageBar.Percentage = 1f;

            UpdateDisplaySize();
            RefreshMovePercentage();
        }

        public void SetCallBack(Action<TrainerMoveInformation> callBack)
        {
            _callBack = callBack;
        }

        public void OnClick()
        {
            _callBack?.Invoke(_moveInformation);
        }

        public void SetAsWhiteTile()
        {
            Background.color = WhiteColor;
        }

        public void SetAsBlackTile()
        {
            Background.color = BlackColor;
        }

        public void SetMovePercentChange(float percentChance)
        {
            if (_moveInformation.ParentMove != null)
            {
                _moveInformation.moveChangePercentage = percentChance;
                _moveInformation.moveChangeTotalPercentage = percentChance * _moveInformation.ParentMove.moveChangeTotalPercentage;

                MovePercentageText.text = (100 * percentChance).ToString("0.0") + "%";
                TotalMovePercentageText.text = (100 * percentChance * _moveInformation.ParentMove.moveChangeTotalPercentage).ToString("0.0") + "%";
            }
            else
            {
                _moveInformation.moveChangePercentage = 1f;
                _moveInformation.moveChangeTotalPercentage = 1f;

                MovePercentageText.text = (100 * _moveInformation.moveChangePercentage).ToString("0.0") + "%";
                TotalMovePercentageText.text = (100 * _moveInformation.moveChangePercentage).ToString("0.0") + "%";
            }
        }

        public void RefreshMovePercentage()
        {
            SetMovePercentChange(_moveInformation.moveChangePercentage);
        }

        public void SetDescription(string text)
        {
            MoveDescriptionText.text = text?.Trim() ?? "";
            _moveInformation.MoveDescription = text?.Trim() ?? "";

            UpdateDisplaySize();
        }

        public string GetDescription()
        {
            return _moveInformation.MoveDescription;
        }

        void UpdateDisplaySize()
        {
            if (string.IsNullOrEmpty(MoveDescriptionText.text))
            {
                if (this.transform is RectTransform transform)
                {
                    transform.sizeDelta = new Vector2(transform.sizeDelta.x, 60);
                }
            }
            else
            {
                if (this.transform is RectTransform transform)
                {
                    transform.sizeDelta = new Vector2(transform.sizeDelta.x, 72.5f);
                }
            }
        }
    }
}
