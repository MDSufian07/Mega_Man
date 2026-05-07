using UnityEngine;

namespace Player
{
    public class InputHandler : MonoBehaviour
    {
        private PlayerInputActions _inputActions;

        public float MoveInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool FirePressed { get; private set; }

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();

            _inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<float>();
            _inputActions.Player.Move.canceled += _ => MoveInput = 0f;

            _inputActions.Player.Jump.performed += _ => JumpPressed = true;
            _inputActions.Player.Jump.canceled += _ => JumpPressed = false;

            _inputActions.Player.Fire.performed += _ => FirePressed = true;
            _inputActions.Player.Fire.canceled += _ => FirePressed = false;
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }
    }
}