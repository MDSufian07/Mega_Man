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

            _inputActions.Player.Move.performed += OnMovePerformed;
            _inputActions.Player.Move.canceled += OnMoveCanceled;

            _inputActions.Player.Jump.performed += OnJumpPerformed;
            _inputActions.Player.Jump.canceled += OnJumpCanceled;

            _inputActions.Player.Fire.performed += OnFirePerformed;
            _inputActions.Player.Fire.canceled += OnFireCanceled;
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void OnDestroy()
        {
            if (_inputActions == null) return;

            _inputActions.Player.Move.performed -= OnMovePerformed;
            _inputActions.Player.Move.canceled -= OnMoveCanceled;

            _inputActions.Player.Jump.performed -= OnJumpPerformed;
            _inputActions.Player.Jump.canceled -= OnJumpCanceled;

            _inputActions.Player.Fire.performed -= OnFirePerformed;
            _inputActions.Player.Fire.canceled -= OnFireCanceled;
        }

        private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            MoveInput = ctx.ReadValue<float>();
        }

        private void OnMoveCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            MoveInput = 0f;
        }

        private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            JumpPressed = true;
        }

        private void OnJumpCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            JumpPressed = false;
        }

        private void OnFirePerformed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            FirePressed = true;
        }

        private void OnFireCanceled(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            FirePressed = false;
        }
    }
}