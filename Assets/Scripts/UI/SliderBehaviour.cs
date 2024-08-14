using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using WS20.P3.Overcrowded;

[RequireComponent(typeof(Slider))]
public class SliderBehaviour : MonoBehaviourPunCallbacks
{
    private Slider mySlider;

    [System.Serializable]
    public enum ValueType{Integer, Float};
    public ValueType myValueType;
    public bool x10;
    
    public IntInstance myIntValue;
    public FloatInstance myFloatValue;

    private void Start()
    {
        mySlider = this.GetComponent<Slider>();

        mySlider.onValueChanged.AddListener(delegate { OnValueChanged(); });
        
        
        switch (myValueType)
        {
            case ValueType.Float:
                mySlider.value = myFloatValue.Float;
                break;
            
            case ValueType.Integer:
                if (!x10)
                {
                    mySlider.value = myIntValue.Integer;
                    break;
                }

                mySlider.value = myIntValue.Integer / 10;
                break;
        }
        mySlider.onValueChanged.Invoke(mySlider.value);
    }

    public void OnValueChanged()
    {
        if (this.GetComponent<OnClickedExtra>())
        {
            return;
        }
        
        switch (myValueType)
        {
            case ValueType.Float:
                myFloatValue.Float = mySlider.value;
                break;

            case ValueType.Integer:
                myIntValue.Integer = (int) mySlider.value;
                break;
        }
    }

}
