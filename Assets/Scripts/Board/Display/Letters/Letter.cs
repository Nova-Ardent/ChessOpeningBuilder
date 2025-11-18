using TMPro;
using UnityEngine;

namespace Board.Display.Letters
{
    public class Letter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _character;

        public void SetCharacter(char character)
        {
            _character.text = character.ToString();
        }

        public void SetColor(Color color)
        {
            _character.color = color;
        }
    }
}
