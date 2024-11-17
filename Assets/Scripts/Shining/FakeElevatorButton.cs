using System.Collections.Generic;
using InteractionManagement;
using UnityEngine;

namespace Shining
{
    public class FakeElevatorButton : Interactable
    {
        private bool m_CallButtonPressed;

        private List<string> m_DisplayInfoList = new List<string>
        {
            "What are you doing down here?",
            "I'm not gonna hurt you",
            "This inhuman place makes human monsters",
            "They shine because they are stars"
        };

        private void Awake()
        {
            m_DisplayInfo = "CALL ELEVATOR";
        }

        public override void OnLookAt()
        {
            base.OnLookAt();
            m_DisplayInfo = m_CallButtonPressed ? m_DisplayInfoList[Random.Range(0,m_DisplayInfoList.Count)] : "Call Elevator";
        }

        public override void Interact()
        {
            base.Interact();
            OnElevatorButtonPressed();
        }

        
        private void OnElevatorButtonPressed()
        {
            if (m_CallButtonPressed)
                return;
            
            m_CallButtonPressed = true;
           
            OnLookAway();
        }
    }
}
