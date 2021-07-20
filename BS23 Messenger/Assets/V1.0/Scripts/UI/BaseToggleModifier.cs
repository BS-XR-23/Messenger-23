using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BaseToggleModifier : MonoBehaviour
{

    
    public Color selectedToggleBGColor = Color.gray;
    public Color deselectedToggleBGColor = Color.white;
    public UnityEvent OnClick;
    protected Toggle toggle;
    protected RectTransform toggleTransform;
    protected Color selectedColor;


    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggleTransform = GetComponent<RectTransform>();
        toggle.group = GetComponentInParent<ToggleGroup>();
    }


    
    void Start()
    {
        

        
        ToggleValueChange();
        

        toggle.onValueChanged.AddListener((a) =>
        {
            ToggleValueChange();
        });
    }

    public virtual void ToggleValueChange()
    {
        if (toggle.isOn) OnClick?.Invoke();
        toggleTransform.GetComponent<Image>().color = !toggle.isOn ? deselectedToggleBGColor : selectedToggleBGColor;
        
        
    }
}
