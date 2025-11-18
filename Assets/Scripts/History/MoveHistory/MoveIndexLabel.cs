using TMPro;
using UnityEngine;

namespace History.MoveHistory
{
    public class MoveIndexLabel : MonoBehaviour
    {
        public TextMeshProUGUI label;

        public void SetMove(int index)
        {
            label.text = index.ToString() + ".";
        }
    }
}
