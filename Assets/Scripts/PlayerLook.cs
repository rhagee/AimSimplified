using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{

    public Camera cam;
    public float xRotation = 0f;

    private float xSensitivity;
    private float ySensitivity;
  
    public PlayerLook()
    {
        xSensitivity = 5.0f;
        ySensitivity = 5.0f;
    }

    public void ProcessLook(Vector2 Input)
    {
        //Vector2 Input passed as Parameter (will be the new movement mouse controller - Delta Mouse)
        //This give double input X and Y with positive and negative values based on mouse movement

        float mouseX = Input.x;
        float mouseY = Input.y;

        //Rotation Calculation based on Sensitivity FPS Free
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        
        //Adjusment Value for 80° , -80° (Clamping)
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        //Apply to camera in localRotation (Depend on parent rotation)
        //Applied to player camera
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //Applied to player (this)
        //Directly apply Y Axis Rotation to the player body (Depend on the world)
        //Vector3.up = Streight way to do Vector3(0,1,0)
        //Streight : transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity)); 
        //Long : transform.Rotate(new Vector3(0,1,0) * (mouseX * Time.deltaTime) * xSensitivity);
        // Mine :
        this.transform.Rotate(new Vector3(0,(mouseX * Time.deltaTime) * xSensitivity, 0) );
    }


    public void ChangeSensitivity(float n)
    {
        xSensitivity = n*10;
        ySensitivity = n*10;
    }
}
