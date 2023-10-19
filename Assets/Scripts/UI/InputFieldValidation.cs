using UnityEngine;
using UnityEngine.UI;

public class InputFieldValidation : MonoBehaviour
{
    
    public InputField inputField;
    
    void Start()
    {
        inputField.onValidateInput += delegate (string s, int i, char c) { return Val(c); };
    }

    char Val(char c)
    {
        c =  char.ToUpper(c);
        return (char.IsLetter(c)|| char.IsDigit(c)) ? c : '\0';
    }
    
}
