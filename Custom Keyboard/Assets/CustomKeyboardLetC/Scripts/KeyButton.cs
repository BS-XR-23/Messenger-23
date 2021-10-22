using UnityEngine;

namespace LetC
{
    public class KeyButton : MonoBehaviour
    {
        public enum AnimationState { unpressed, pressed};
        public AnimationState animationState;
        public string id;
        public GameObject image;
        public GameObject text;

        public void OnClick_KeyButton()
        {
            TextKeyboard.instance.OnClick_KeyButton(new KeyboardKeyPressResponse() { id = id, label = text.GetComponent<TMPro.TMP_Text>().text});
        }

        public void OnPointerEnter()
        {
            if(animationState == AnimationState.unpressed)
            {
                animationState = AnimationState.pressed;
                GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_onpressed"));
            }
        }

        public void OnPointerDown()
        {
            if (animationState == AnimationState.unpressed)
            {
                animationState = AnimationState.pressed;
                GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_onpressed"));
            }
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
        }

        public void OnPointerUp()
        {
            if (animationState == AnimationState.pressed)
            {
                animationState = AnimationState.unpressed;
                GetComponent<Animator>().Play(Animator.StringToHash("keyboard_btn_unpressed"));
            }
        }
    }

}