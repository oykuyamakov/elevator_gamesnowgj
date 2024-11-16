using InteractionManagement;
using UnityEngine;

namespace SceneManagement
{
    public class CallElevatorButton : Interactable
    {
        [SerializeField] 
        private Elevator m_Elevator;
        
        private bool m_CallButtonPressed;

        private void Awake()
        {
            m_DisplayInfo = "CALL ELEVATOR";
        }

        public override void OnLookAt()
        {
            base.OnLookAt();
            m_DisplayInfo = !m_Elevator.IsPlayerIn ? m_CallButtonPressed ? "" : "Call Elevatoto" : "";
        }

        public override void Interact()
        {
            base.Interact();
            OnElevatorButtonPressed();
        }

      

        private void OnElevatorButtonPressed()
        {
            if(m_Elevator.IsPlayerIn)
                return;
            
            if (m_CallButtonPressed)
                return;
            
            m_CallButtonPressed = true;
            m_Elevator.OnElevatorCalled();
            
            m_DisplayInfo = "";
            OnLookAway();
        }
    }
}
