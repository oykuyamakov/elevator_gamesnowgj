using UnityEngine;

namespace InteractionManagement
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField]
        protected string m_DisplayInfo;
        public string DisplayInfo => m_DisplayInfo;
        
        public virtual void Interact()
        {
        }
        
        public virtual void OnLookAt()
        {
            
        }
        
        public virtual void OnLookAway()
        {
            
        }
    }
}
