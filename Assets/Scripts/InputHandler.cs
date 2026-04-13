using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private PlayerInputActions inputActions;

    public float MoveInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool FirePressed { get; private set; }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<float>();
        inputActions.Player.Move.canceled += ctx => MoveInput = 0f;

        inputActions.Player.Jump.performed += ctx => JumpPressed = true;
        inputActions.Player.Jump.canceled += ctx => JumpPressed = false;

        inputActions.Player.Fire.performed += ctx => FirePressed = true;
        inputActions.Player.Fire.canceled += ctx => FirePressed = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}