using UnityEngine;
using UnityEngine.UI;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(Text))]
    public class OnValueChangedText : MonoBehaviour
    {
        #region Private Fields

        private Text ValueText;

        #endregion

        #region MonoBehaviour CallBacks

        private void OnEnable()
        {
            ValueText = GetComponent<Text>();
        }

        #endregion

        #region Public Methods

        public void OnSliderValueChanged(float value)
        {
            ValueText.text = value.ToString("");
        }
        
        public void OnSliderValueChangedx10(float value)
        {
            float newValue = value * 10;
            ValueText.text = newValue.ToString("");
        }

        #endregion
    }
}