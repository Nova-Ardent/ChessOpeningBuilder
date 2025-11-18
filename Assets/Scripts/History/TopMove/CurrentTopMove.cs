using Board.Common;
using Board.State;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Board.History.TopMove
{
    public class CurrentTopMove : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] Color BackgroundColorWhite;
        [SerializeField] Color BackgroundColorBlack;

        [SerializeField] Color TextColorWhite;
        [SerializeField] Color TextColorBlack;

        [SerializeField] RawImage EvalBackground;
        [SerializeField] TextMeshProUGUI EvalText;
        [SerializeField] TextMeshProUGUI MoveText;
        [SerializeField] TextMeshProUGUI DepthText;

        [SerializeField] BoardObject BoardObject;

        [SerializeField] float _evalAmount;

        bool _mouseHovering = false;

        public float EvalAmount
        {
            get { return _evalAmount; }
            set
            {
                _evalAmount = value;
            }
        }

        [SerializeField] bool _hasMate;
        public bool HasMate
        {
            get { return _hasMate; }
            set
            {
                _hasMate = value;
            }
        }

        [SerializeField] int _mateValue;
        public int MateValue
        {
            get { return _mateValue; }
            set
            {
                _mateValue = value;
            }
        }

        string _pendingMoveClear;
        [SerializeField] string _move;
        public string Move
        {
            get { return _move; }
            set
            {
                if (_mouseHovering)
                {
                    _pendingMoveClear = _move;
                }
                _move = value;
            }
        }

        [SerializeField] int _depth;
        public int Depth
        {
            get { return _depth; }
            set
            {
                _depth = value;
            }
        }

        public void MakeMove()
        {
            BoardObject.MovePieceUCI(_move);
        }

        public void UpdateColorsAndText()
        {
            MoveText.text = _move;
            DepthText.text = "depth: " + _depth.ToString();

            if (HasMate)
            {
                if (MateValue < 0)
                {
                    EvalBackground.color = BackgroundColorBlack;
                    EvalText.color = TextColorBlack;
                    EvalText.text = "M" + Mathf.Abs(MateValue);
                }
                else
                {
                    EvalBackground.color = BackgroundColorWhite;
                    EvalText.color = TextColorWhite;
                    EvalText.text = "M" + Mathf.Abs(MateValue);
                }
            }
            else
            {
                if (EvalAmount < 0)
                {
                    EvalBackground.color = BackgroundColorBlack;
                    EvalText.color = TextColorBlack;
                    
                    if (Mathf.Abs(EvalAmount) < 10)
                    {
                        EvalText.text = EvalAmount.ToString("0.00");    
                    }
                    else
                    {
                        EvalText.text = EvalAmount.ToString("00.0");
                    }
                }
                else
                {
                    EvalBackground.color = BackgroundColorWhite;
                    EvalText.color = TextColorWhite;

                    if (EvalAmount < 10)
                    {
                        EvalText.text = '+' +  EvalAmount.ToString("0.00");
                    }
                    else
                    {
                        EvalText.text = '+' + EvalAmount.ToString("00.0");
                    }
                }
            }
        }

        void HighlightMove()
        {
            if (!(_move.Length == 4 || _move.Length == 5))
            {
                Debug.LogError("invalid UCI length");
                return;
            }

            Files fromFile = _move[0].ToFile();
            Ranks fromRank = _move[1].ToRank();

            Files toFile = _move[2].ToFile();
            Ranks toRank = _move[3].ToRank();

            BoardObject.AddArrow(fromFile, fromRank, toFile, toRank);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                HighlightMove();
            }
        }
    }
}

