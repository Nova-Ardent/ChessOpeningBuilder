using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ScrollablePanel : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
    {
        [SerializeField] RectTransform rectTransform;
        [SerializeField] float _scrollSpeed;

        float _containerSize;
        float _dataSize;
        float _scrollWindow;

        float _scroll;

        Action<float> _onScroll;

        bool _mouseInWindow = false;

        private void Update()
        {
            _containerSize = rectTransform.rect.height;
            _scrollWindow = Mathf.Max(0, _dataSize - _containerSize);

            float newScroll = _scroll;
            if (_mouseInWindow && Input.mouseScrollDelta != Vector2.zero)
            {
                newScroll -= Input.mouseScrollDelta.y * _scrollSpeed;
            }

            newScroll = Mathf.Clamp(newScroll, 0, _scrollWindow);

            if (!Mathf.Approximately(newScroll, _scroll))
            {
                _scroll = newScroll;
                _onScroll?.Invoke(_scroll);
            }
        }

        public void SetDataSize(float dataSize)
        {
            _dataSize = dataSize;
        }

        public void RegisterOnScrollUpdate(Action<float> onScroll)
        {
            _onScroll = onScroll;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _mouseInWindow = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _mouseInWindow = true;
        }
    }
}
