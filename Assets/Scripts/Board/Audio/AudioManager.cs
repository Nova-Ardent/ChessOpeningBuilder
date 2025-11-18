using Board.Moves;
using UnityEngine;

namespace Board.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] AudioSource Move;
        [SerializeField] AudioSource Capture;
        [SerializeField] AudioSource Castle;
        [SerializeField] AudioSource Check;
        [SerializeField] AudioSource Promotion;

        public void Play(MoveInformation moveInformation)
        {
            if (moveInformation.IsCheck)
            {
                Check.Play();
            }
            else if (moveInformation.IsCastle)
            {
                Castle.Play();
            }
            else if (moveInformation.IsCapture)
            {
                Capture.Play();
            }
            else if (moveInformation.Promotion != null)
            {
                Promotion.Play();
            }
            else
            {
                Move.Play();
            }
        }
    }
}
