using UnityEngine;
using System.Collections.Generic;
using System;

namespace Board.FlipBoard
{
    public class FlipBoardHandler : MonoBehaviour
    {
        [SerializeField] bool maintainRotation = false;
        [SerializeField] List<FlipBoardHandler> Children = new List<FlipBoardHandler>();
   
        bool _isFlipped = false;
        Transform _transform;
        Action<bool> _onFlip;

        private void Awake()
        {
            _transform = transform;
        }

        public void SyncBoardFlipped()
        {
            SetBoardFlipped(_isFlipped);
        }

        public void FlipBoard()
        {
            SetBoardFlipped(!_isFlipped);
        }

        public void SetBoardFlipped(bool isFlipped)
        {
            _isFlipped = isFlipped;

            if (maintainRotation)
            {
                _transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                if (!_isFlipped)
                    _transform.rotation = Quaternion.Euler(0, 0, 0);
                else
                    _transform.rotation = Quaternion.Euler(0, 0, 180);
            }

            foreach (var child in Children)
            {
                child.SetBoardFlipped(isFlipped);
            }

            _onFlip?.Invoke(isFlipped);
        }

        public void AddChild(FlipBoardHandler child)
        {
            Children.Add(child);
        }

        public void RemoveChild(FlipBoardHandler child)
        {
            Children.Remove(child);
        }

        public void ClearChildren()
        {
            Children.Clear();
        }

        public void RegisterOnFlip(Action<bool> callback)
        {
            _onFlip = callback;
        }
    }
}