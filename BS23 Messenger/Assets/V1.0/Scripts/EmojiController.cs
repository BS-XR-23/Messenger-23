using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class EmojiController : MonoBehaviour
{

    public GameObject emoji_keyboard;

    public RectTransform panelBody;
    public GameObject emoji_panel;
    public GameObject rect_panel;


   

    

    public void OpenEmojiKeyboard()
    {
        
        emoji_keyboard.SetActive(true);
        KeyboardManager.instance.activeInputField.enabled = true;
           
        
    }
    public void CloseEmojiKeyboard()
    {

        
            emoji_keyboard.SetActive(false);
            TMP_InputField inputField = ChatUIManager.instance.sendMessangeInputField;
            KeyboardManager.instance.activeInputField.enabled = true;
            KeyboardManager.instance.activeInputField.caretPosition = inputField.text.Length + 1;
            KeyboardManager.instance.activeInputField.ForceLabelUpdate();

    }


    public void Open()
    {
        emoji_panel.SetActive(true);
        rect_panel.SetActive(true);
        panelBody.localScale = Vector2.zero;
        panelBody.DOScale(1f, 0.3f);
    }

    public void Close()
    {
        rect_panel.SetActive(false);
        panelBody.DOScale(0f, 0.3f).OnComplete(() => {

            emoji_panel.SetActive(false);
        });



    }


    public void ActivateEmoji(int buttonID)
    {
        TMP_InputField inputField = ChatUIManager.instance.sendMessangeInputField;
        inputField.text = inputField.text + "<sprite=" + buttonID + ">";

        KeyboardManager.instance.activeInputField.caretPosition = inputField.text.Length + 1;
        //Debug.Log("Text length " + KeyboardManager.instance.activeInputField.caretPosition);
        //Debug.Log("Text llength " + inputField.text.Length);

        KeyboardManager.instance.activeInputField.ForceLabelUpdate();

        //Debug.Log("text " + ChatUIManager.instance.sendMessangeInputField.text);
    }
    
}
