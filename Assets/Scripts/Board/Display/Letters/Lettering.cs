using Board.FlipBoard;
using UnityEngine;

namespace Board.Display.Letters
{
    public class Lettering : MonoBehaviour
    {
        [SerializeField] Color _DarkSquareColor;
        [SerializeField] Color _LightSquareColor;

        [SerializeField] Letter _rankPrefab;
        [SerializeField] Letter _filePrefab;

        Letter[] _ranks = new Letter[8];
        Letter[] _files = new Letter[8];

        FlipBoardHandler _boardFlipHandler;

        private void Awake()
        {
            _boardFlipHandler = GetComponent<FlipBoardHandler>();

            for (int i = 0; i < 8; i++)
            {
                _ranks[i] = Instantiate(_rankPrefab, transform);
                _ranks[i].SetCharacter((char)('1' + i));
                _ranks[i].SetColor((i % 2 == 0) ? _DarkSquareColor : _LightSquareColor);
                _ranks[i].transform.localPosition = new Vector3(-350f, -350f + i * 100, 0);
                _boardFlipHandler.AddChild(_ranks[i].GetComponent<FlipBoardHandler>());

                _files[i] = Instantiate(_filePrefab, transform);
                _files[i].SetCharacter((char)('a' + i));
                _files[i].SetColor((i % 2 == 0) ? _DarkSquareColor : _LightSquareColor);
                _files[i].transform.localPosition = new Vector3(-350f + i * 100, -350f, 0);
                _boardFlipHandler.AddChild(_ranks[i].GetComponent<FlipBoardHandler>());
            }

            _boardFlipHandler.RegisterOnFlip(OnFlip);
        }

        public void OnFlip(bool isFlipped)
        {
            for (int i = 0; i < 8; i++)
            {
                if (isFlipped)
                {
                    _ranks[i].SetCharacter((char)('8' - i));
                    _files[i].SetCharacter((char)('h' - i));
                }
                else
                {
                    _ranks[i].SetCharacter((char)('1' + i));
                    _files[i].SetCharacter((char)('a' + i));
                }
            }
        }
    }
}
