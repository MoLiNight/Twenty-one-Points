using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Text valueText;

    public void SetValueText(string value)
    {
        valueText.text = value;
    }
}
