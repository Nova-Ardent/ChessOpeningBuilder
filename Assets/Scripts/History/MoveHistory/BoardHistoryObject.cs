using Board;
using Board.Moves;
using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using static History.MoveHistory.BoardHistoryObject;

namespace History.MoveHistory
{
    public class BoardHistoryObject : MonoBehaviour
    {
        [System.Serializable]
        public struct MovePositionData
        {
            public float indexX;

            public float x;
            public float y;
            public float dx;
            public float dy;
        }

        [Header("Prefab info")]
        [SerializeField] MoveIndexLabel _moveIndexLabelPrefab;
        [SerializeField] MoveLabel _moveLabelPrefab;
        [SerializeField] MovePositionData _movePositionData;

        [Header("Links")]
        [SerializeField] BoardObject _boardObject;

        [Header("Scrolling")]
        [SerializeField] ScrollablePanel _scrollPanel;
        float _scrollAmount;

        List<MoveLabel> _moveLabelPool = new List<MoveLabel>();
        List<MoveIndexLabel> _moveIndexLabelPool = new List<MoveIndexLabel>();

        Action _onLabelsUpdatedCallback = () => { };

        private void Start()
        {
            _scrollPanel.RegisterOnScrollUpdate(OnScrollUpdate);
            _boardObject.RegisterOnMoveCallback(UpdateView);
            UpdateView();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _boardObject.ViewMove(_boardObject.GetViewedMoveIndex() + 1);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _boardObject.ViewMove(_boardObject.GetViewedMoveIndex() - 1);
            }
        }

        void UpdateView()
        {
            UpdateIndexLabels();
            UpdateMoveLabels();
            OnScrollUpdate(_scrollAmount);

            _onLabelsUpdatedCallback();
        }

        void UpdateIndexLabels()
        {
            int indicesCount = _boardObject.GetMoveHistoryCount() / 2 + 1;
            for (int i = 0; i < indicesCount; i++)
            {
                if (i < _moveIndexLabelPool.Count)
                {
                    _moveIndexLabelPool[i].gameObject.SetActive(true);
                }
                else
                {
                    MoveIndexLabel newIndexLabel = Instantiate(_moveIndexLabelPrefab, transform);
                    newIndexLabel.SetMove(i + 1);
                    _moveIndexLabelPool.Add(newIndexLabel);
                }
            }
        }

        void UpdateMoveLabels()
        {
            int labelIndex = 0;
            foreach (MoveInformation moveInformation in _boardObject.GetMoveHistory())
            {
                if (labelIndex < _moveLabelPool.Count)
                {
                    _moveLabelPool[labelIndex].gameObject.SetActive(true);
                    _moveLabelPool[labelIndex].SetMove(moveInformation.ToString());
                }
                else
                {
                    MoveLabel moveLabel = Instantiate(_moveLabelPrefab, transform);
                    moveLabel.SetMove(moveInformation.ToString());
                    _moveLabelPool.Add(moveLabel);
                }

                int onClickIndex = labelIndex;
                _moveLabelPool[labelIndex].SetCallback(() => _boardObject.ViewMove(onClickIndex));

                labelIndex++;
            }

            _scrollPanel.SetDataSize(((_moveLabelPool.Count + 1) / 2) * _movePositionData.dy + 10);
        }

        void OnScrollUpdate(float newScrollAmount)
        {
            _scrollAmount = newScrollAmount;

            int index = 0;
            foreach (MoveIndexLabel indexLabel in _moveIndexLabelPool)
            {
                if (indexLabel.transform is RectTransform rt)
                {
                    rt.anchoredPosition = new Vector2
                        ( _movePositionData.indexX
                        , _movePositionData.y - _movePositionData.dy * index + _scrollAmount
                        );
                }

                index++;
            }

            index = 0;
            foreach (MoveLabel label in _moveLabelPool)
            {
                if (label.transform is RectTransform rt)
                {
                    int row = index / 2;
                    int column = index % 2;
                    rt.anchoredPosition = new Vector2
                        ( _movePositionData.x + _movePositionData.dx * column
                        , _movePositionData.y - _movePositionData.dy * row + _scrollAmount
                        );
                }

                index++;
            }
        }

        public void ClearHistory()
        {
            foreach (var label in _moveLabelPool)
            {
                label.gameObject.SetActive(false);
                label.SetColorToNormal();
            }

            foreach (var indexLabel in _moveIndexLabelPool)
            {
                indexLabel.gameObject.SetActive(false);
            }

            _boardObject.ClearHistory();
            UpdateView();
        }

        public void RemoveLatestMove()
        {
            foreach (var label in _moveLabelPool)
            {
                label.gameObject.SetActive(false);
            }

            foreach (var indexLabel in _moveIndexLabelPool)
            {
                indexLabel.gameObject.SetActive(false);
            }

            _boardObject.RemoveLatestMove();
            UpdateView();
        }

        public void RegisterLabelsUpdatedCallback(Action callback)
        {
            _onLabelsUpdatedCallback += callback;
        }

        public void MarkLabelAsIncorrect(int index)
        {
            if (index >= 0 && index < _moveLabelPool.Count)
            {
                _moveLabelPool[index].SetColorToFailed();
            }
            else
            {
                while (index >= _moveLabelPool.Count)
                {
                    MoveLabel moveLabel = Instantiate(_moveLabelPrefab, transform);
                    moveLabel.gameObject.SetActive(false);
                    _moveLabelPool.Add(moveLabel);
                }

                OnScrollUpdate(_scrollAmount);
                _moveLabelPool[index].SetColorToFailed();
            }
        }
    }
}
