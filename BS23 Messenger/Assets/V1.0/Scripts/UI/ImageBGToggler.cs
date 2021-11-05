using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBGToggler : BaseToggleModifier
{

    public Image UnmuteImage;
    public Image MuteImage;


    public override void ToggleValueChange()
    {
        base.ToggleValueChange();
        if(toggle.isOn)
        {
            MuteImage.enabled = true;
            UnmuteImage.enabled = false;
        }
        else
        {
            UnmuteImage.enabled = true;
            MuteImage.enabled = false;
        }
    }

}
