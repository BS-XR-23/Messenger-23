using TMPro;
using UnityEngine;
public class KeyboardManager : TextKeyboard
{
    public static KeyboardManager instance;
    private void Awake()
    {
        /*
        if (instance != null)
        {
            GetAllTextFromInputFields();
            DestroyImmediate(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }*/
        instance = this;

#if UNITY_ANDROID
        GetAllTextFromInputFields();
        isSingleCapKey = false;
#endif
    }
    void GetAllTextFromInputFields()
    {
        foreach (TMP_InputField inputField in FindObjectsOfType<TMP_InputField>(true))
        {
            inputField.onSelect.AddListener((string input) =>
            {
                isActive = true;
                activeInputField = inputField;
                activeInputField.IsActive();

                activeInputField.ActivateInputField();

                ActiveInputField(inputField);

                OnClickDoneKey -= InputFieldForScreenKeyboardPanelAdjuster.instance.KeyboardDown;
                OnClickDoneKey += InputFieldForScreenKeyboardPanelAdjuster.instance.KeyboardDown;
            });

        }
    }

    private void ActiveInputField(TMP_InputField inputField)
    {
        if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber || inputField.contentType == TMP_InputField.ContentType.DecimalNumber)
        {
            numaricKeyboard.SetActive(true);
            normalKeyboard.SetActive(false);
        }
        else
        {
            numaricKeyboard.SetActive(false);
            normalKeyboard.SetActive(true);
        }
    }
}
