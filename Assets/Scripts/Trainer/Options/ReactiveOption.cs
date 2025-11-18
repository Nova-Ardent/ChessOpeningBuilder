using Board.Common;
using System;
using TMPro;
using UnityEngine;

namespace Trainer.Options
{
    [System.Serializable]
    public class ReactiveOption<T> where T : System.Enum, IConvertible
    {
        [SerializeField] TMP_Dropdown dropdown;

        Action<T> _onValueChanged = x => { };

        T _value;
        public T Value 
        {
            get => _value;
            set 
            {
                SetValue(value);
            } 
        }

        public void Init()
        {
            if (dropdown != null)
            {
                dropdown.onValueChanged.AddListener(delegate { OnDropDownValueChanged(); });
            }
        }

        public void RegisterOnValueChanged(Action<T> onValueChanged)
        {
            _onValueChanged += onValueChanged;
        }

        public void OnDropDownValueChanged()
        {
            SetValueWithIndex(dropdown.value);
        }

        public void SetValue(T newValue)
        {
            if (_value.Equals(newValue))
                return;

            _value = newValue;
            if (dropdown != null)
                dropdown.value = Convert.ToInt32(newValue);

            _onValueChanged?.Invoke(_value);
        }

        public void SetValueWithIndex(int index)
        {
            if (Convert.ToInt32(_value) == index)
                return;

            _value = (T)System.Enum.ToObject(typeof(T), index);
            if (dropdown != null)
                dropdown.value = index;

            _onValueChanged?.Invoke(_value);
        }
    }
}