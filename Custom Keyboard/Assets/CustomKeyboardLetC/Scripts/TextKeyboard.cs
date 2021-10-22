using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LetC
{
    [ExecuteInEditMode]
    public class TextKeyboard : MonoBehaviour
    {
        public static TextKeyboard instance;

        public enum KeyboardState { ONE = 0, TWO = 1, THREE = 2, FOUR = 3};
        public KeyboardState keyboardState;
        public KeyboardConfiguration[] keyboardConfiguration;
        public RectTransform[] horizontal_layouts;

        public Sprite shiftkey_sprite;
        public Sprite backspace_sprite;

        float h_LayoutWidth, h_LayoutSpacing, h_ChildCount, h_SpaceCount, key_width;

        public TMP_InputField activeInputField;

        public System.Action OnClickDoneKey;

        [HideInInspector]
        public bool isActive;

        private RectTransform r_transform;

        public float GetHeight()
        {
            return r_transform.rect.height;
        }

        private void Awake()
        {
            if(instance != null)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void OnEnable()
        {
            r_transform = GetComponent<RectTransform>();
            Invoke("SetupKeyboardLayout",0.2f);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                if (isActive)
                {
                    r_transform.anchoredPosition = Vector2.Lerp(r_transform.anchoredPosition, Vector2.zero, Time.deltaTime * 5);
                }
                else
                {
                    r_transform.anchoredPosition = Vector2.Lerp(r_transform.anchoredPosition, new Vector2(0, -r_transform.rect.height), Time.deltaTime * 5);
                }
            }
#else
            if (isActive)
            {
                r_transform.anchoredPosition = Vector2.Lerp(r_transform.anchoredPosition,Vector2.zero,Time.deltaTime * 5);
            }
            else
            {
                r_transform.anchoredPosition = Vector2.Lerp(r_transform.anchoredPosition, new Vector2(0, -r_transform.rect.height), Time.deltaTime * 5);
            }
#endif

        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if(!EditorApplication.isPlaying)
                SetupKeyboardLayout();
#endif
        }

        void SetupKeyboardLayout()
        {
            keyboardState       = KeyboardState.ONE;
            h_LayoutWidth       = horizontal_layouts[0].sizeDelta.x;
            h_LayoutSpacing     = horizontal_layouts[0].GetComponent<HorizontalLayoutGroup>().spacing;
            h_ChildCount        = horizontal_layouts[0].childCount;
            h_SpaceCount        = h_ChildCount + 1;
            key_width           = ( h_LayoutWidth - (h_LayoutSpacing * (float)h_SpaceCount) ) / (float)h_ChildCount;
            //Debug.Log("h_LayoutWidth "+ h_LayoutWidth+ " h_LayoutSpacing "+ h_LayoutSpacing+ " h_ChildCount "+ h_ChildCount+ " h_SpaceCount"+ h_SpaceCount+ " key_width " + key_width);
            for (int i = 0; i < horizontal_layouts.Length-1; i++)
            {
                SetKeySize(horizontal_layouts[i]);
            }
            SetFunctionalKeys(horizontal_layouts[3], horizontal_layouts[4]);
            SetKeysAndLabels(keyboardState);
        }

        void SetKeySize(RectTransform h_layout)
        {
            for (int i = 0; i < h_layout.childCount; i++)
            {
                Vector2 size = h_layout.GetChild(i).GetComponent<RectTransform>().sizeDelta;
                size.x = key_width;
                h_layout.GetChild(i).GetComponent<RectTransform>().sizeDelta = size;
            }
        }

        void SetFunctionalKeys(RectTransform h_layout, RectTransform spacekey_layout)
        {
            float width = (h_LayoutWidth - (float)((h_layout.childCount+1f) * h_LayoutSpacing)) - (key_width * 7f);
            float button_width = (width) / 2f;


            // shift key
            Vector2 size = h_layout.GetChild(0).GetComponent<RectTransform>().sizeDelta;
            size.x = button_width;
            h_layout.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;
            // backspace key
            size = h_layout.GetChild(h_layout.childCount-1).GetComponent<RectTransform>().sizeDelta;
            size.x = button_width;
            h_layout.GetChild(h_layout.childCount - 1).GetComponent<RectTransform>().sizeDelta = size;
            // bottomleft key
            size = spacekey_layout.GetChild(0).GetComponent<RectTransform>().sizeDelta;
            size.x = button_width;
            spacekey_layout.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;
            // space key
            float spacekey_width = (h_LayoutWidth - (float)((spacekey_layout.childCount + 1f) * h_LayoutSpacing)) - (button_width * 2f);
            size = spacekey_layout.GetChild(1).GetComponent<RectTransform>().sizeDelta;
            size.x = spacekey_width;
            spacekey_layout.GetChild(1).GetComponent<RectTransform>().sizeDelta = size;
            // bottomright key
            size = spacekey_layout.GetChild(2).GetComponent<RectTransform>().sizeDelta;
            size.x = button_width;
            spacekey_layout.GetChild(2).GetComponent<RectTransform>().sizeDelta = size;
        }

        void SetKeysAndLabels(KeyboardState k_state)
        {
            for (int j = 0; j < keyboardConfiguration[(int)k_state].layouts.Length; j++)
            {
                for (int k = 0; k < keyboardConfiguration[(int)k_state].layouts[j].keys.Length; k++)
                {
                    if (keyboardConfiguration[(int)k_state].layouts[j].keys[k].isfunctionKey)
                    {
                        if (keyboardConfiguration[(int)k_state].layouts[j].keys[k].sprite == null)
                        {
                            horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().image.SetActive(false);
                            horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().text.SetActive(true);
                            horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().id = keyboardConfiguration[(int)k_state].layouts[j].keys[k].id;
                            horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().text.GetComponent<TMP_Text>().text = keyboardConfiguration[(int)k_state].layouts[j].keys[k].label;
                        }
                        else
                        {
                            horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().text.SetActive(false);
                            horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().image.SetActive(true);
                            horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().id = keyboardConfiguration[(int)k_state].layouts[j].keys[k].id;
                            horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().image.GetComponent<Image>().sprite = keyboardConfiguration[(int)k_state].layouts[j].keys[k].sprite;
                        }
                    }
                    else
                    {
                        horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().id = keyboardConfiguration[(int)k_state].layouts[j].keys[k].id;
                        horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().text.GetComponent<TMP_Text>().text = keyboardConfiguration[(int)k_state].layouts[j].keys[k].label;
                    }
                }
            }
        }

        public void OnClick_KeyButton(KeyboardKeyPressResponse response)
        {
            switch (response.id)
            {
                case "symbols1":
                    keyboardState = KeyboardState.THREE;
                    SetKeysAndLabels(keyboardState);
                    break;
                case "symbols2":
                    keyboardState = KeyboardState.ONE;
                    SetKeysAndLabels(keyboardState);
                    break;
                case "shift1":
                    keyboardState = KeyboardState.TWO;
                    SetKeysAndLabels(keyboardState);
                    break;
                case "shift2":
                    keyboardState = KeyboardState.ONE;
                    SetKeysAndLabels(keyboardState);
                    break;
                case "shift3":
                    keyboardState = KeyboardState.FOUR;
                    SetKeysAndLabels(keyboardState);
                    break;
                case "shift4":
                    keyboardState = KeyboardState.THREE;
                    SetKeysAndLabels(keyboardState);
                    break;
                case "space":
                    activeInputField.text += " ";
                    break;
                case "done":
                    OnClickDoneKey?.Invoke();
                    break;
                case "backspace":
                    if (activeInputField.text.Length > 0)
                        activeInputField.text = activeInputField.text.Substring(0, activeInputField.text.Length - 1);
                    break;
                default:
                    activeInputField.text += response.label;
                    break;
            }
        }
    }

    [System.Serializable]
    public class KeyboardConfiguration
    {
        public KeyboardLayout[] layouts;
    }
    [System.Serializable]
    public class KeyboardLayout
    {
        public KeyboardKeyConfiguration[] keys;
    }
    [System.Serializable]
    public class KeyboardKeyConfiguration
    {
        public bool isfunctionKey;
        public string id;
        public Sprite sprite;
        public string label;
    }
    public class KeyboardKeyPressResponse
    {
        public string id;
        public string label;
    }
}
