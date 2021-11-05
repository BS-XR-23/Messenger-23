using UnityEngine;

namespace LetC
{
    public class InputfieldPositionManager : MonoBehaviour
    {
        public RectTransform ui_panel;
        public RectTransform inputField;

        //public void OnClick_Inputfield()
        //{
        //    TextKeyboard.instance.isActive = true;
        //    TextKeyboard.instance.activeInputField = inputField.GetComponent<TMPro.TMP_InputField>();
        //    TextKeyboard.instance.OnClickDoneKey -= OnClick_DoneKey;
        //    TextKeyboard.instance.OnClickDoneKey += OnClick_DoneKey;

        //    if (inputField.anchoredPosition.y < TextKeyboard.instance.GetHeight())
        //    {
        //        InputFieldForScreenKeyboardPanelAdjuster.instance.KeyboardUp();
        //        //ui_panel.anchoredPosition = new Vector2(ui_panel.anchoredPosition.x, TextKeyboard.instance.GetHeight() - inputField.anchoredPosition.y);
        //    }
        //}

        //public void OnClick_DoneKey()
        //{
        //    InputFieldForScreenKeyboardPanelAdjuster.instance.KeyboardDown();
        //    TextKeyboard.instance.isActive = false;
        //    ui_panel.anchoredPosition = new Vector2(ui_panel.anchoredPosition.x, 0);
        //}
    }
}

