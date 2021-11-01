using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TextKeyboard : MonoBehaviour
{
    //public static TextKeyboard instance;

    public enum KeyboardState { ONE = 0, TWO = 1, THREE = 2, FOUR = 3 };
    public GameObject normalKeyboard, numaricKeyboard;
    public KeyboardState keyboardState;
    public KeyboardConfiguration[] keyboardConfiguration;
    public RectTransform[] horizontal_layouts;

    public Sprite shiftkey_sprite;
    public Sprite backspace_sprite;
    public System.Action OnClickDoneKey;
    public bool isSingleCapKey;
    private float h_LayoutWidth, h_LayoutSpacing, h_ChildCount, h_SpaceCount, key_width;

    [HideInInspector] public TMP_InputField? activeInputField;
    [HideInInspector] public bool isActive;
    [HideInInspector] public GameObject spawnKeyPrefab;
    [HideInInspector] public int selectionPosition;

    private RectTransform r_transform;
    public float GetHeight()
    {
        return r_transform.rect.height;
    }

    //private void Awake()
    //{
    //    if (instance != null)
    //    {
    //        DestroyImmediate(gameObject);
    //    }
    //    else
    //    {
    //        instance = this;
    //        DontDestroyOnLoad(this);
    //    }
    //}

    private void OnEnable()
    {
        r_transform = GetComponent<RectTransform>();
        Invoke("SetupKeyboardLayout", 0.2f);
    }

    private void Update()
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

    void SetupKeyboardLayout()
    {
        
        keyboardState = KeyboardState.ONE;
        h_LayoutWidth = horizontal_layouts[0].sizeDelta.x;
        h_LayoutSpacing = horizontal_layouts[0].GetComponent<HorizontalLayoutGroup>().spacing;
        h_ChildCount = horizontal_layouts[0].childCount;
        h_SpaceCount = h_ChildCount + 1;
        key_width = (h_LayoutWidth - (h_LayoutSpacing * (float)h_SpaceCount)) / (float)h_ChildCount;
        //Debug.Log("h_LayoutWidth "+ h_LayoutWidth+ " h_LayoutSpacing "+ h_LayoutSpacing+ " h_ChildCount "+ h_ChildCount+ " h_SpaceCount"+ h_SpaceCount+ " key_width " + key_width);
        for (int i = 0; i < horizontal_layouts.Length - 1; i++)
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
        float width = (h_LayoutWidth - (float)((h_layout.childCount + 1f) * h_LayoutSpacing)) - (key_width * 7f);
        float button_width = (width) / 2f;

        // shift key
        Vector2 size = h_layout.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        size.x = button_width;
        h_layout.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;
        // backspace key
        size = h_layout.GetChild(h_layout.childCount - 1).GetComponent<RectTransform>().sizeDelta;
        size.x = button_width;
        h_layout.GetChild(h_layout.childCount - 1).GetComponent<RectTransform>().sizeDelta = size;
        // bottomleft key
        size = spacekey_layout.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        size.x = button_width;
        spacekey_layout.GetChild(0).GetComponent<RectTransform>().sizeDelta = size;

        size = spacekey_layout.GetChild(1).GetComponent<RectTransform>().sizeDelta;
        size.x = button_width;
        spacekey_layout.GetChild(1).GetComponent<RectTransform>().sizeDelta = size;
        // space key
        float spacekey_width = (h_LayoutWidth - (float)((spacekey_layout.childCount + 1f) * h_LayoutSpacing)) - (button_width * 3f);
        size = spacekey_layout.GetChild(1).GetComponent<RectTransform>().sizeDelta;
        size.x = spacekey_width;
        spacekey_layout.GetChild(2).GetComponent<RectTransform>().sizeDelta = size;
        // bottomright key
        size = spacekey_layout.GetChild(3).GetComponent<RectTransform>().sizeDelta;
        size.x = button_width;
        spacekey_layout.GetChild(3).GetComponent<RectTransform>().sizeDelta = size;
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
                    horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().id = IdGeneration(j, k, k_state);// keyboardConfiguration[(int)k_state].layouts[j].keys[k].id;
                    horizontal_layouts[j].GetChild(k).GetComponent<KeyButton>().text.GetComponent<TMP_Text>().text = IdGeneration(j, k, k_state);// keyboardConfiguration[(int)k_state].layouts[j].keys[k].label;
                }
            }
        }
    }
    int temp;
    string IdGeneration(int layout, int key, KeyboardState k_state)
    {
        int state = (int)k_state;
        temp = 0;
        if (state == 0)
        {
            return IdGeneration(layout, key);
        }
        else if (state == 1)
        {
            temp = 36;
            return IdGeneration(layout, key + temp);
        }
        else if (state == 2)
        {
            temp = 72;
            return IdGeneration(layout, key + temp);
        }
        else
        {
            temp = 98;
            return IdGeneration(layout, key + temp);
        }
    }
    string IdGeneration(int layout, int key)
    {
        if (layout == 0)
        {
            int i = key - temp;
            return AlphabetData.englishAlphabetDictionary[i];
        }
        else if (layout == 1)
        {
            key += 10;
            return AlphabetData.englishAlphabetDictionary[key];
        }
        else if (layout == 2)
        {
            key += 20;
            return AlphabetData.englishAlphabetDictionary[key];
        }
        else if (layout == 3)
        {
            key += 28;
            return AlphabetData.englishAlphabetDictionary[key];
        }
        else
        {
            return "";
        }
    }
    public void OnClick_KeyButton(KeyboardKeyPressResponse response)
    {
        Debug.Log(isSingleCapKey);
        switch (response.id)
        {
            case "symbols1":
                keyboardState = KeyboardState.THREE;
                SetKeysAndLabels(keyboardState);
                SingleCapKey();
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
                if (activeInputField.text.Length+1 >= activeInputField.characterLimit && activeInputField.characterLimit != 0) break;
                if (activeInputField.text.Length == 0) break;
                string s = activeInputField.text.Insert(activeInputField.caretPosition, " ");
                activeInputField.text = s;
                activeInputField.caretPosition++;
                //activeInputField.selectionFocusPosition++;
                activeInputField.ForceLabelUpdate();
                SingleCapKey();
                break;
            case "done":
                SingleCapKey();
                OnClickDoneKey?.Invoke();
                break;
            case "backspace":
                if (activeInputField.text.Length == 0) break;

                string value = activeInputField.text;
                List<char> charList = new List<char>();
                foreach (char item in value)
                {
                    charList.Add(item);
                }
                charList.RemoveAt(activeInputField.caretPosition - 1);
                string originalword = "";

                foreach (var letter in charList)
                {
                    originalword += letter.ToString();
                }
                activeInputField.text = originalword;
                if (activeInputField.caretPosition == activeInputField.text.Length)
                {
                    activeInputField.ForceLabelUpdate();
                    return;
                }
                else
                {
                    activeInputField.caretPosition--;
                    activeInputField.ForceLabelUpdate();
                }
                SingleCapKey();
                break;
            default:
                if (activeInputField.text.Length >= activeInputField.characterLimit && activeInputField.characterLimit != 0) break;
                string t = activeInputField.text.Insert(activeInputField.caretPosition, response.label);
                activeInputField.text = t;
                selectionPosition++;
                activeInputField.caretPosition++;
                activeInputField.ForceLabelUpdate();
                SingleCapKey();
                break;
        }
    }
    private void SingleCapKey()
    {
        if (isSingleCapKey)
        {
            keyboardState = KeyboardState.ONE;
            SetKeysAndLabels(keyboardState);
            isSingleCapKey = false;
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