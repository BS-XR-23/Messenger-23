using UnityEngine;

public class KeyButton : MonoBehaviour
{
    public enum AnimationState { unpressed, pressed };
    public AnimationState animationState;
    public string id;
    public GameObject image;
    public GameObject text;

    bool isLong;
    public void OnClick_KeyButton()
    {
        TextKeyboard.instance.OnClick_KeyButton(new KeyboardKeyPressResponse() { id = id, label = text.GetComponent<TMPro.TMP_Text>().text });
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
        if (animationState == AnimationState.unpressed)
        {
            Debug.Log("Removing");
            animationState = AnimationState.pressed;
            GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_onpressed"));
        }
        InvokeRepeating("LongPressed", 1, 0.15f);
    }

    public void OnPointerClick()
    {
        if (animationState == AnimationState.unpressed)
        {
            animationState = AnimationState.pressed;
            GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_onpressed"));
        }
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