using Board.Common;
using Board.Controller;
using Board.Display.Arrows;
using System.Collections.Generic;
using UnityEngine;

namespace Board.Display.Arrows
{
    public class ArrowManager : MonoBehaviour
    {
        public Arrow ArrowPrefab;

        Dictionary<MouseData, Arrow> _arrowLookup = new Dictionary<MouseData, Arrow>();

        public void UpdateArrow(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            MouseData mouseData = new MouseData
            {
                FromPosition = new BoardPosition(fromFile, fromRank),
                ToPosition = new BoardPosition(toFile, toRank)
            };

            if (_arrowLookup.ContainsKey(mouseData))
            {
                Destroy(_arrowLookup[mouseData].gameObject);
                _arrowLookup.Remove(mouseData);
            }
            else
            {
                CreateArrow(mouseData);
            }
        }

        public void AddArrow(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            MouseData mouseData = new MouseData
            {
                FromPosition = new BoardPosition(fromFile, fromRank),
                ToPosition = new BoardPosition(toFile, toRank)
            };

            if (_arrowLookup.ContainsKey(mouseData))
            {
                return;
            }
            else
            {
                CreateArrow(mouseData);
            }
        }

        void CreateArrow(MouseData mouseData)
        {
            Vector2 p1 = 100 * new Vector2((int)mouseData.FromPosition.File, (int)mouseData.FromPosition.Rank) - new Vector2(350, 350);
            Vector2 p2 = 100 * new Vector2((int)mouseData.ToPosition.File, (int)mouseData.ToPosition.Rank) - new Vector2(350, 350);
            Vector2 position = (p1 + p2) / 2;
            float size = Vector2.Distance(p1, p2);
            float angle = Vector2.SignedAngle(Vector2.right, p2 - p1);

            Arrow arrow = Instantiate<Arrow>(ArrowPrefab, this.transform);

            arrow.Size = size;
            arrow.transform.localPosition = position;
            arrow.transform.localRotation = Quaternion.Euler(0, 0, angle);

            _arrowLookup[mouseData] = arrow;
        }

        public void RemoveArrowIfExists(Files fromFile, Ranks fromRank, Files toFile, Ranks toRank)
        {
            MouseData mouseData = new MouseData
            {
                FromPosition = new BoardPosition(fromFile, fromRank),
                ToPosition = new BoardPosition(toFile, toRank)
            };

            if (_arrowLookup.ContainsKey(mouseData))
            {
                Destroy(_arrowLookup[mouseData].gameObject);
                _arrowLookup.Remove(mouseData);
            }
        }

        public void ClearAll()
        {
            foreach (var arrow in _arrowLookup.Values)
            {
                Destroy(arrow.gameObject);
            }
            _arrowLookup.Clear();
        }
    }
}