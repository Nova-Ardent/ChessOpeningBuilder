using Board.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Board.Display.Highlight
{
    public class Highlighting : MonoBehaviour
    {
        [SerializeField] Color _darkSquareColor;
        [SerializeField] Color _lightSquareColor;
        [SerializeField] GameObject _highlightSquarePrefab;

        GameObject[] _highlightSquares;
        bool[] _isHighlighted;

        private void Awake()
        {
            _highlightSquares = new GameObject[64];
            _isHighlighted = new bool[64];

            int index = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Vector2 p = 100 * new Vector2(x, y) - new Vector2(350, 350);

                    GameObject highlight = Instantiate<GameObject>(_highlightSquarePrefab, this.transform);
                    RawImage image = highlight.GetComponent<RawImage>();

                    highlight.transform.localPosition = p;

                    if (x % 2 == y % 2)
                    {
                        image.color = _darkSquareColor;
                    }
                    else
                    {
                        image.color = _lightSquareColor;
                    }

                    _highlightSquares[index] = highlight;
                    _isHighlighted[index] = false;
                    index++;
                }
            }
        }

        public void SetHighlight(Files file, Ranks rank, bool highlight)
        {
            int index = (int)file * 8 + (int)rank;
            _isHighlighted[index] = highlight;
            _highlightSquares[index].SetActive(highlight);
        }

        public bool IsHighlighted(Files file, Ranks rank)
        {
            return _isHighlighted[(int)file * 8 + (int)rank];
        }

        public void ToggleHighlight(Files file, Ranks rank)
        {
            SetHighlight(file, rank, !_isHighlighted[(int)file * 8 + (int)rank]);
        }

        public void ClearAll()
        {
            for (int i = 0; i < 64; i++)
            {
                _isHighlighted[i] = false;
                _highlightSquares[i].SetActive(false);
            }
        }
    }
}

