using System;
using InteractionManagement;
using Roro.Scripts.GameManagement;
using UnityEngine;

namespace SceneManagement
{
    public class ElevatorButton : Interactable
    {
        [SerializeField] 
        private Elevator m_Elevator;
        
        private bool m_ElevatorButtonPressed;

        private void Awake()
        {
            m_DisplayInfo = "GO UP";
        }

        public override void Interact()
        {
            base.Interact();
            OnElevatorButtonPressed();
        }

        private void OnElevatorButtonPressed()
        {
            if (m_ElevatorButtonPressed)
                return;
            
            m_ElevatorButtonPressed = true;
            m_Elevator.MoveElevator();
            
            m_DisplayInfo = "elevator used";
            OnLookAway();
        }
    }
}
