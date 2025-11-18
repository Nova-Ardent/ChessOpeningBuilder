using Board.Common;
using UnityEngine;

namespace Board.Display.Moves
{
    public class Move : MonoBehaviour
    {
        [SerializeField] RectTransform Transform;

        [SerializeField] Files _file;
        [SerializeField] Ranks _rank;
        [SerializeField] bool _isTake;

        public Files File
        {
            get { return _file; }
            set
            {
                _file = value;
                UpdatePosition();
            }
        }

        public Ranks Rank
        {
            get { return _rank; }
            set
            {
                _rank = value;
                UpdatePosition();
            }
        }

        private void Awake()
        {
            UpdatePosition();
        }

        void UpdatePosition()
        {
            int x = (int)_file;
            int y = (int)_rank;
            Vector2 p1 = 100 * new Vector2(x, y) - new Vector2(350, 350);

            Transform.localPosition = p1;
        }
    }
}