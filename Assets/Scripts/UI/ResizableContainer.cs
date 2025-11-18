using UnityEngine;

namespace UI
{
    public class ResizableContainer : MonoBehaviour
    {
        [SerializeField] ResizableContainer[] _children;

        private void Start()
        {
            SizeChanged();
        }

        public virtual void SizeChanged()
        {
            foreach (var child in _children)
            {
                child.SizeChanged();
            }
        }
    }
}

