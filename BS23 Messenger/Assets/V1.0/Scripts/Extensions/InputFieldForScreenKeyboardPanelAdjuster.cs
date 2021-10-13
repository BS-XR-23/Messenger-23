using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class InputFieldForScreenKeyboardPanelAdjuster : MonoBehaviour, IPointerDownHandler
{
    public static InputFieldForScreenKeyboardPanelAdjuster instance;//temp

    private GameObject panel;


    private RectTransform panelRectTrans;
    private Vector2 panelOffsetMinOriginal;
    private float panelHeightOriginal;
    private float currentKeyboardHeightRatio;
    private bool isJustPop;

    public void Start()
    {
        instance = this;//temp
        panel = gameObject;
        panelRectTrans = panel.GetComponent<RectTransform>();
        panelOffsetMinOriginal = panelRectTrans.offsetMin;
        panelHeightOriginal = panelRectTrans.rect.height;
        GetAllTextFromInputFields();
    }

    IEnumerator BufferPeriod()
    {
        isJustPop = true;
        yield return new WaitForFixedUpdate();
        isJustPop = false;
    }


    public void KeyboardUp()
    {
        if (isJustPop)
            return;
        float newKeyboardHeightRatio = GetKeyboardHeightRatio();
        if (currentKeyboardHeightRatio != newKeyboardHeightRatio)
        {
            StartCoroutine(BufferPeriod());
            currentKeyboardHeightRatio = newKeyboardHeightRatio;
            panelRectTrans.offsetMin = new Vector2(panelOffsetMinOriginal.x, panelHeightOriginal * currentKeyboardHeightRatio);
        }
    }

    public void KeyboardDown()
    {
        if (isJustPop)
            return;
        StartCoroutine(BufferPeriod());
        DelayedReset();
        currentKeyboardHeightRatio = 0f;
        TextKeyboard.instance.isActive = false;
    }

    public float GetKeyboardHeightRatio()
    {
        return 0.5f;
    }

    void DelayedReset()
    {
        panelRectTrans.offsetMin = panelOffsetMinOriginal;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        KeyboardDown();
        
    }
    void GetAllTextFromInputFields()
    {
        foreach (TMP_InputField inputField in gameObject.GetComponentsInChildren<TMP_InputField>())
        {
            inputField.onSelect.AddListener((string input) =>
            {

                TextKeyboard.instance.isActive = true;
                TextKeyboard.instance.activeInputField = inputField;
                TextKeyboard.instance.activeInputField.IsActive();
                
                TextKeyboard.instance.activeInputField.ActivateInputField();

                RectTransform rt = inputField.GetComponent<RectTransform>();
                if (rt.anchoredPosition.y < TextKeyboard.instance.GetHeight())
                {
                    KeyboardUp();
                }
            });
        }
    }


    
    private void OnDisable()
    {
        DelayedReset();
        currentKeyboardHeightRatio = 0f;
        TextKeyboard.instance.isActive = false;
    }
}
