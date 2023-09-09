using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;
    public PlayerLook look;

    RunHandler.GameState state;
    RunHandler master;

    // Start is called before the first frame update
    void Awake()
    {
        GameObject x = GameObject.FindGameObjectWithTag("Master");
        master = x.GetComponent<RunHandler>();
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        look = GetComponent<PlayerLook>();
    }

    // Update is called once per frame
    void Update()
    {
        state = master.state;
        if (state==RunHandler.GameState.Running)
            look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {

        /*Best Practice using LateUpdate for Camera since Update is mostly used for movement
        GameObject settings = GameObject.FindGameObjectWithTag("Setting");
        bool inMenu = settings.GetComponent<HideCursor>().inMenu;
        if(!inMenu)
            look.ProcessLook(onFoot.Look.ReadValue<Vector2>());*/
    }

    public void OnSensChange(float n) {
        look.ChangeSensitivity(n);
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
