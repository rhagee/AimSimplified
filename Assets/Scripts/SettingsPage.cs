using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsPage : MonoBehaviour
{
    public TextMeshProUGUI txtSens;

    public void OnSensChange(float newValue)
    {
        newValue = Mathf.Clamp(newValue, 0.01f, 0.99f);
        txtSens.text = ((decimal)Math.Round((decimal)newValue * 10, 3) ).ToString();
    }

}
