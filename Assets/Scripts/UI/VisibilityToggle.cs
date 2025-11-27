using UnityEngine;

namespace UI
{
    public class VisibilityToggle : MonoBehaviour
    {
        public bool _isVisible = false;

        public void Toggle()
        {
            if (_isVisible)
            {
                _isVisible = false;
                gameObject.SetActive(false);
            }
            else
            {
                _isVisible = true;
                gameObject.SetActive(true);
            }
        }
    }
}
