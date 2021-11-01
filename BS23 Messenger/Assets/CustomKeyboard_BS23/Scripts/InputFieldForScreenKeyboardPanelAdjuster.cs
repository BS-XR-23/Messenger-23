using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        //GetAllTextFromInputFields();
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
        KeyboardManager.instance.isActive = false;
    }
   
    public float GetKeyboardHeightRatio()
    {
        return 0.35f;
    }

    void DelayedReset() 
    {
        panelRectTrans.offsetMin = panelOffsetMinOriginal; 
    }
    void OnDisable()
    {
        DelayedReset();
        currentKeyboardHeightRatio = 0f;
        KeyboardManager.instance.isActive = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        KeyboardDown();
    }
    //void GetAllTextFromInputFields()
    //{
    //    foreach (TMP_InputField inputField in gameObject.GetComponentsInChildren<TMP_InputField>())
    //    {
    //        inputField.onSelect.AddListener((string input) =>
    //        {
    //            KeyboardManager.instance.isActive = true;
    //            KeyboardManager.instance.activeInputField = inputField;
    //            KeyboardManager.instance.activeInputField.IsActive();

    //            KeyboardManager.instance.activeInputField.ActivateInputField();

    //            KeyboardManager.instance.OnClickDoneKey -= KeyboardDown;
    //            KeyboardManager.instance.OnClickDoneKey += KeyboardDown;
    //            /*
    //            RectTransform rt = inputField.GetComponent<RectTransform>();
    //            if (rt.anchoredPosition.y < TextKeyboard.instance.GetHeight())
    //            {
    //                KeyboardUp();
    //            }*/
    //        });
    //    }
    //}
}
