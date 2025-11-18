using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Board.Common;
using System.Linq;
using Board.Display.Moves;
using Board.Pieces;

namespace Board.Controller
{
    public class BoardController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerMoveHandler
    {
        [SerializeField] BoardObject _boardObject;

        RectTransform _transform;
        MouseData _leftClickData;
        MouseData _rightClickData;

        Piece piecePickedUp = null;

        void Awake()
        {
            if (transform is RectTransform rectTransform)
            {
                _transform = rectTransform;
            }
            else
            {
                Debug.LogError("BoardController must be attached to a GameObject with a RectTransform.");
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (_leftClickData.IsMouseDown)
                {
                    return;
                }

                _rightClickData.FromPosition = ToLocalPosition(eventData.pressPosition);
                _rightClickData.IsMouseDown = true;
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (_rightClickData.IsMouseDown)
                {
                    return;
                }

                _leftClickData.FromPosition = ToLocalPosition(eventData.pressPosition);
                _leftClickData.IsMouseDown = true;

                if (_boardObject.CurrentlySelectedPiece != null 
                    && _boardObject.CurrentlySelectedPiece.GetLegalMoves().Any(m => m.File == _leftClickData.FromPosition.File && m.Rank == _leftClickData.FromPosition.Rank))
                {
                    PossibleMoveInfo possibleMoveInfo = _boardObject.CurrentlySelectedPiece.GetLegalMoves().First(m => m.File == _leftClickData.FromPosition.File && m.Rank == _leftClickData.FromPosition.Rank);
                    _boardObject.MoveHighlightedPiece(possibleMoveInfo.File, possibleMoveInfo.Rank);
                }
                else
                {
                    _boardObject.SelectSquare
                        ( _leftClickData.FromPosition.File
                        , _leftClickData.FromPosition.Rank
                        );
                }

                if (_boardObject.GetPiece(_leftClickData.FromPosition.File, _leftClickData.FromPosition.Rank) != null)
                {
                    piecePickedUp = _boardObject.GetPiece(_leftClickData.FromPosition.File, _leftClickData.FromPosition.Rank);
                    UpdatePickedUpPiecePosition(eventData);
                }
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                if (!_rightClickData.IsMouseDown)
                {
                    return;
                }

                _rightClickData.ToPosition = ToLocalPosition(eventData.position);
                _rightClickData.IsMouseDown = false;

                _boardObject.MarkBoard
                    ( _rightClickData.FromPosition.File
                    , _rightClickData.FromPosition.Rank
                    , _rightClickData.ToPosition.File
                    , _rightClickData.ToPosition.Rank
                    );
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (!_leftClickData.IsMouseDown)
                {
                    return;
                }

                _leftClickData.ToPosition = ToLocalPosition(eventData.position);
                _leftClickData.IsMouseDown = false;

                if (piecePickedUp != null)
                {
                    piecePickedUp.UpdatePosition();
                    piecePickedUp = null;
                }

                if (_boardObject.CurrentlySelectedPiece != null)
                {
                    _boardObject.CurrentlySelectedPiece.UpdatePosition();
                }

                if (_boardObject.CurrentlySelectedPiece != null
                    && _boardObject.CurrentlySelectedPiece.GetLegalMoves().Any(m => m.File == _leftClickData.ToPosition.File && m.Rank == _leftClickData.ToPosition.Rank))
                {
                    PossibleMoveInfo possibleMoveInfo = _boardObject.CurrentlySelectedPiece.GetLegalMoves().First(m => m.File == _leftClickData.ToPosition.File && m.Rank == _leftClickData.ToPosition.Rank);
                    _boardObject.MoveHighlightedPiece(possibleMoveInfo.File, possibleMoveInfo.Rank);
                }
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _leftClickData.IsMouseDown = false;
            _rightClickData.IsMouseDown = false;

            if (piecePickedUp != null)
            {
                piecePickedUp.UpdatePosition();
                piecePickedUp = null;
            }
        }

        public void OnPointerMove(PointerEventData eventData)
        {
            if (_leftClickData.IsMouseDown && piecePickedUp != null)
            {
                UpdatePickedUpPiecePosition(eventData);
            }
        }

        void UpdatePickedUpPiecePosition(PointerEventData eventData)
        {
            piecePickedUp.transform.localPosition = piecePickedUp.transform.parent.InverseTransformPoint(eventData.position);
        }

        BoardPosition ToLocalPosition(Vector2 position)
        {
            Vector3 outPosition = transform.InverseTransformPoint(position);
            outPosition.x = 8 * Mathf.Clamp(outPosition.x / _transform.rect.width + 0.5f, 0, 1);
            outPosition.y = 8 * Mathf.Clamp(outPosition.y / _transform.rect.height + 0.5f, 0, 1);
            return new BoardPosition((Files)outPosition.x, (Ranks)outPosition.y);
        }
    }
}