using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{

    public InputManager im;
    public void OnSensChange(float newValue)
    {
        newValue = Mathf.Clamp(newValue, 0.01f, 0.99f);
        im = GetComponent<InputManager>();
        im.OnSensChange((float)Math.Round((decimal)newValue, 3));
    }
}
