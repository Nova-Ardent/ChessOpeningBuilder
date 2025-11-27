using System;
using TMPro;
using Trainer.Data;
using Unity.VisualScripting;
using UnityEngine;

namespace UI
{
    public class ValueInput : MonoBehaviour
    {
        [SerializeField] bool _hasMax = false;
        [SerializeField] int _maxValue;

        [SerializeField] bool _hasMin = false;
        [SerializeField] int _minValue;

        public int CurrentValue { get; private set; }


        [SerializeField] TMP_InputField InputField;
        [SerializeField] TextMeshProUGUI DefaultText;

        Action<int> _onValueChangedCallback;

        private void Awake()
        {
            if (_hasMin)
            {
                DefaultText.text = _minValue.ToString();
                CurrentValue = _minValue;
            }
        }

        public void RegisterOnValueChanged(Action<int> callback)
        {
            _onValueChangedCallback = callback;
        }

        public void OnValueChanged()
        {
            if (int.TryParse(InputField.text, out int value))
            {
                if (_hasMax && value > _maxValue)
                {
                    value = _maxValue;
                    InputField.text = value.ToString();
                }

                if (_hasMin && value < _minValue)
                {
                    value = _minValue;
                    InputField.text = value.ToString();
                }

                CurrentValue = value;
                _onValueChangedCallback?.Invoke(value);
                return;
            }

            InputField.text = "";

            CurrentValue = _minValue;
            _onValueChangedCallback?.Invoke(_minValue);
        }
    }
}