using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class ClampedContainer : ResizableContainer
    {
        RectTransform _parent;
        RectTransform _transform;

        private void Awake()
        {
            if (this.gameObject.transform.parent is RectTransform transform)
            {
                _parent = transform;
            }
            else
            {
                Debug.LogError("Parent needs to be a RectTransform for ParentClamp to work.");
            }

            if (this.gameObject.transform is RectTransform rectTransform)
            {
                _transform = rectTransform;
            }
            else
            {
                Debug.LogError("This GameObject needs to be a RectTransform for ParentClamp to work.");
            }
        }

        public override void SizeChanged()
        {
            float width = _parent.rect.width / 800;
            float height = _parent.rect.height / 800;

            _transform.localScale = new Vector3(width, height, 1);

            base.SizeChanged();
        }
    }
}
