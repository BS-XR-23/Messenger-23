using System.Collections;
using UnityEngine;

public class InputFieldForScreenKeyboardPanelAdjuster : MonoBehaviour
{

    
    private GameObject panel;

    
    private RectTransform panelRectTrans;
    private Vector2 panelOffsetMinOriginal;
    private float panelHeightOriginal;
    private float currentKeyboardHeightRatio;
    private bool isJustPop;

    public void Start()
    {

        panel = gameObject;
        panelRectTrans = panel.GetComponent<RectTransform>();
        panelOffsetMinOriginal = panelRectTrans.offsetMin;
        panelHeightOriginal = panelRectTrans.rect.height;
        
    }

    IEnumerator BufferPeriod()
    {
        isJustPop = true;
        yield return new WaitForFixedUpdate();
        isJustPop = false;
    }


    public void KeyboardUp()
    {
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
        if(isJustPop)
        {
            return;
        }
        if (panelRectTrans.offsetMin != panelOffsetMinOriginal)
            DelayedReset();
        currentKeyboardHeightRatio = 0f;
    }
   
    private float GetKeyboardHeightRatio()
    {
        return 0.5f;


    }

    void DelayedReset() 
    {
        panelRectTrans.offsetMin = panelOffsetMinOriginal; 
    }

   
   
}
