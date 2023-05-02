using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    public static InputManager Instance => instance;

    public float minHoldDistance = 0.1f;

    public Vector2 Delta { get; private set; }
    public Vector2 Position { get; private set; }
    public bool Holding => holding;
    public bool Clicked => clicked;

    private bool clicked = false;
    private bool holding = false;

    public delegate void MouseEvent();
    public static event MouseEvent RegisterHoldEvent;
    public static event MouseEvent RegisterClickEvent;
    public static event MouseEvent RegisterStopHoldEvent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateDelta(InputAction.CallbackContext context)
    {
        Delta = context.ReadValue<Vector2>();
        if(clicked && Delta.sqrMagnitude > minHoldDistance && !holding){
            holding = true;
            RegisterHoldEvent?.Invoke();
        }
    }

    public void UpdatePosition(InputAction.CallbackContext context)
    {
        Position = context.ReadValue<Vector2>();
    }

    public void ClickAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            clicked = true;
        }

        if(context.canceled){
            clicked = false;
            if(holding){
                holding = false;
                RegisterStopHoldEvent?.Invoke();
            }else{
                RegisterClickEvent?.Invoke();
            }
        }
    }

}
