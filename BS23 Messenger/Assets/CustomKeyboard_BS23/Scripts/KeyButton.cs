using UnityEngine;
using UnityEngine.EventSystems;
public class KeyButton : MonoBehaviour//, IPointerClickHandler
{
    public enum AnimationState { unpressed, pressed };
    public AnimationState animationState;
    public string id;
    public GameObject image;
    public GameObject text;

    public void OnClick_KeyButton()
    {
        KeyboardManager.instance.OnClick_KeyButton(new KeyboardKeyPressResponse() { id = id, label = text.GetComponent<TMPro.TMP_Text>().text });
    }

    public void OnPointerEnter()
    {
        if (animationState == AnimationState.unpressed)
        {
            animationState = AnimationState.pressed;
            GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_onpressed"));
        }
    }

    public void OnPointerDown()
    {
        //if (id == "shift1") { CancelInvoke(); return; }

        if (animationState == AnimationState.unpressed)
        {
            animationState = AnimationState.pressed;
            GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_onpressed"));
        }
        InvokeRepeating("LongPressed", 1, 0.15f);
    }
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (id == "shift1" && !KeyboardManager.instance.isSingleCapKey) {
    //        if (animationState == AnimationState.unpressed)
    //        {
    //            animationState = AnimationState.pressed;
    //            GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_onpressed"));
    //            int clickCount = eventData.clickCount;
    //            if (clickCount == 1)
    //                OnSingleClick(); 
    //            else if (clickCount == 2)
    //                OnDoubleClick();
    //        }
            
    //    }
    //    CancelInvoke();
    //}

    void OnSingleClick()
    {
        KeyboardManager.instance.isSingleCapKey = true;
        KeyboardManager.instance.OnClick_KeyButton(new KeyboardKeyPressResponse() { id = "shift1", label = text.GetComponent<TMPro.TMP_Text>().text });
    }

    void OnDoubleClick()
    {
        KeyboardManager.instance.isSingleCapKey = false;
        KeyboardManager.instance.OnClick_KeyButton(new KeyboardKeyPressResponse() { id = "shift1", label = text.GetComponent<TMPro.TMP_Text>().text });
        
    }

    void OnMultiClick()
    {
        Debug.Log("MultiClick Clicked");
    }
    public void OnPointerExit()
    {
        if (animationState == AnimationState.pressed)
        {

            animationState = AnimationState.unpressed;
            GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_unpressed"));
        }
        CancelInvoke();
    }

    public void OnPointerUp()
    {
        if (animationState == AnimationState.pressed)
        {
            animationState = AnimationState.unpressed;
            GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_unpressed"));
        }
        CancelInvoke();
    }

    void LongPressed()
    {
        OnClick_KeyButton();
    }

    
}