using System;
using UnityCommon.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputManagement
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : SingletonBehaviour<InputManager>
    {
        private PlayerInput playerInput => GetComponent<PlayerInput>();

        private void Awake()
        {
            if (!SetupInstance())
                return;
        }
        
        [Header("Mouse Cursor Settings")]
        public bool CursorLocked = true;
        public bool CursorInputForLook = true;
        
        public bool IsCurrentDeviceMouse => playerInput.currentControlScheme == "KeyboardMouse";
        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }
        public bool Jump { get; private set; }
        public bool AnalogMovement { get; }
        public bool Sprint { get; private set; }
        public bool Interact { get; private set; }
        public bool ExitInteraction { get; private set; }
        
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnLook(InputValue value)
        {
            if (CursorInputForLook)
            {
                LookInput(value.Get<Vector2>());
            }
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnSprint(InputValue value)
        {
            SprintInput(value.isPressed);
        }
        
        public void OnInteract(InputValue value)
        {
            InteractInput(value.isPressed);
        }

        public void OnExitInteraction(InputValue value)
        {
            ExitInteractionInput(value.isPressed);
        }
    

        public void MoveInput(Vector2 newMoveDirection)
        {
            Move = newMoveDirection;
        }

        public void LookInput(Vector2 newLookDirection)
        {
            Look = newLookDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            Jump = newJumpState;
        }

        public void SprintInput(bool newSprintState)
        {
            Sprint = newSprintState;
        }
        
        private void InteractInput(bool value)
        {
            Interact = value;
        }
        
        private void ExitInteractionInput(bool value)
        {
            ExitInteraction = value;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(CursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}