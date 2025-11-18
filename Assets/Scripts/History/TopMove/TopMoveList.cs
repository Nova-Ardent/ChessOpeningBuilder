using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Board.History.TopMove
{
    public class TopMoveList : MonoBehaviour
    {
        [SerializeField] CurrentTopMove[] _topMoves;
        TopMoveData[] _moveData;
        bool[] _isMoveDataDirty;
        private void Awake()
        {
            _moveData = new TopMoveData[_topMoves.Length];
            _isMoveDataDirty = new bool[_topMoves.Length];
        }

        private void Update()
        {
            for (int i = 0; i < _topMoves.Length; i++)
            {
                if (_isMoveDataDirty[i])
                {
                    _isMoveDataDirty[i] = false;
                    _topMoves[i].gameObject.SetActive(true);

                    _topMoves[i].EvalAmount = _moveData[i].Evaluation;
                    _topMoves[i].MateValue = _moveData[i].MateIn;
                    _topMoves[i].HasMate = _moveData[i].HasMate;

                    _topMoves[i].Move = _moveData[i].UCI;
                    _topMoves[i].Depth = _moveData[i].Depth;

                    _topMoves[i].UpdateColorsAndText();
                }
            }
        }

        public void ClearData()
        {
            for (int i = 0; i < 5; i++)
            {
                _topMoves[i].gameObject.SetActive(false);
                _moveData[i] = new TopMoveData();
                _isMoveDataDirty[i] = false;
            }
        }

        public void SetTopMoveData(int multipv, TopMoveData moveData)
        {
            multipv--;
            if (multipv < 0 || multipv >= _topMoves.Length)
            {
                Debug.Log($"multipv {multipv} is out of bounds");
                return;
            }

            _moveData[multipv] = moveData;
            _isMoveDataDirty[multipv] = true;
        }
    }
}
