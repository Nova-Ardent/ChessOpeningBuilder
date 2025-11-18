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
        public PercentageBar PercentageBar;

        Action<TrainerMoveInformation> _callBack = null;
        TrainerMoveInformation _moveInformation;

        public void Init(TrainerMoveInformation moveInformation)
        {
            _moveInformation = moveInformation;
            MoveNameText.text = moveInformation.MoveNotation;
            PercentageBar.Percentage = 1f;
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
    }
}
