using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Conversations 
{
    //Data
    public string DateText;
   

    public void UpdatedDateTime()
    {
        DateText = DateTime.Now.ToString();
    }
}
