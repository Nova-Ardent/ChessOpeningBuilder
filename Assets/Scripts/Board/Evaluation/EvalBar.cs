using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Board.Evaluation
{
    public class EvalBar : MonoBehaviour
    {
        [SerializeField] float MaxWhiteMatePosition = 0;
        [SerializeField] float MinBlackMatePosition = -800;

        [SerializeField] float MaxWhiteEvalPosition = -50f;
        [SerializeField] float MinBlackEvalPosition = -750f;

        [SerializeField] float EvalRange = 10;

        [SerializeField] float _evalAmount;
        public float EvalAmount
        {
            get { return _evalAmount; }
            set
            {
                _evalAmount = value;
            }
        }

        [SerializeField] bool _hasMate;
        public bool HasMate
        {
            get { return _hasMate; }
            set
            {
                _hasMate = value;
            }
        }

        [SerializeField] int _mateValue;
        public int MateValue
        {
            get { return _mateValue; }
            set
            {
                _mateValue = value;
            }
        }

        float visualPosition = 0;

        [SerializeField] RawImage EvalPercentageBar;
        [SerializeField] TextMeshProUGUI BlackText;
        [SerializeField] TextMeshProUGUI WhiteText;

        void Update()
        {
            UpdateEvalText();

            if (HasMate)
            {
                float evalAmount = MateValue < 0 ? 1 : -1;
                float remap = (evalAmount + 1) / 2; // Remap from -1 to 1 into 0 to 1

                if (EvalPercentageBar.transform is RectTransform transform)
                {
                    float resultingPosition = Mathf.Lerp(MaxWhiteMatePosition, MinBlackMatePosition, remap);

                    visualPosition = Mathf.Lerp(transform.offsetMax.y, resultingPosition, 0.05f);

                    transform.offsetMax = new Vector2
                        ( transform.anchoredPosition.x
                        , visualPosition
                        );
                }

                if (MateValue > 0)
                {
                    WhiteText.gameObject.SetActive(true);
                    BlackText.gameObject.SetActive(false);
                }
                else
                {
                    WhiteText.gameObject.SetActive(false);
                    BlackText.gameObject.SetActive(true);
                }
            }
            else
            {
                float evalAmount = -Mathf.Clamp(EvalAmount, -EvalRange, EvalRange) / EvalRange;
                float remap = (evalAmount + 1) / 2; // Remap from -1 to 1 into 0 to 1

                if (EvalPercentageBar.transform is RectTransform transform)
                {
                    float resultingPosition = Mathf.Lerp(MaxWhiteEvalPosition, MinBlackEvalPosition, remap);

                    visualPosition = Mathf.Lerp(transform.offsetMax.y, resultingPosition, 0.05f);

                    transform.offsetMax = new Vector2
                        ( transform.anchoredPosition.x
                        , visualPosition
                        );
                }

                if (EvalAmount > 0)
                {
                    WhiteText.gameObject.SetActive(true);
                    BlackText.gameObject.SetActive(false);
                }
                else
                {
                    WhiteText.gameObject.SetActive(false);
                    BlackText.gameObject.SetActive(true);
                }
            }
        }

        void UpdateEvalText()
        {
            if (HasMate)
            {
                int mateValue = Mathf.Abs(this.MateValue);
                WhiteText.text = "M" + mateValue.ToString();
                BlackText.text = "M" + mateValue.ToString();
            }
            else
            {
                float evalAmount = Mathf.Abs(EvalAmount);
                WhiteText.text = evalAmount.ToString("0.0");
                BlackText.text = evalAmount.ToString("0.0");
            }
        }
    }
}
