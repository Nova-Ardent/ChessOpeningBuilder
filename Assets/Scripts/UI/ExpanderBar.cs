using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ExpanderBar : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] RectTransform _this;
        [SerializeField] RectTransform _container;

        [SerializeField] ResizableContainer _left;
        [SerializeField] ResizableContainer _right;

        RectTransform _leftTransform;
        RectTransform _rightTransform;

        bool _isMouseDown = false;


        public void OnPointerDown(PointerEventData eventData)
        {
            _isMouseDown = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMouseDown = false;
        }

        void Awake()
        {
            if (_left.transform is RectTransform leftRect)
            {
                _leftTransform = leftRect;
            }
            else
            {
                Debug.LogError("Left transform is not a RectTransform");
                return;
            }


            if (_right.transform is RectTransform rightRect)
            {
                _rightTransform = rightRect;
            }
            else
            {
                Debug.LogError("Right transform is not a RectTransform");
                return;
            }

            _leftTransform.offsetMax = new Vector2(_this.position.x - _container.rect.width, _leftTransform.offsetMax.y);
            _rightTransform.offsetMin = new Vector2(_this.position.x, _rightTransform.offsetMin.y);
        }

        void Update()
        {
            if (_isMouseDown)
            {
                var position = Input.mousePosition;
                if (_leftTransform != null)
                {
                    _leftTransform.offsetMax = new Vector2(position.x - _container.rect.width, _leftTransform.offsetMax.y);
                    _left.SizeChanged();
                }

                if (_rightTransform != null)
                {
                    _rightTransform.offsetMin = new Vector2(position.x, _rightTransform.offsetMin.y);
                    _right.SizeChanged();
                }
                _this.position = new Vector3(position.x, _this.position.y, _this.position.z);
            }
        }
    }
}
