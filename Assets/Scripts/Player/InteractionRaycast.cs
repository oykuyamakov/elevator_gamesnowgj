using InputManagement;
using InteractionManagement;
using TMPro;
using UnityEngine;

namespace Player
{
    public class InteractionRaycaster : MonoBehaviour
    {

        private Ray ray;
        private Interactable currentInteractable;

        private InputManager inputManager;
        private bool isInteracting = false;
        
        private bool initialized = false;
        
        [SerializeField] private float maxRaycastDistance = 5f;
        [SerializeField] private TextMeshProUGUI objectText;


        #region Unity Lifecycle
        
        private void Start()
        {
            currentInteractable = null;
            inputManager = InputManager.Instance;
            if (inputManager == null)
            {
                Debug.LogError("No InputManager found in scene");
                return;
            }
            if (objectText == null)
            {
                Debug.LogError("No object text assigned to InteractionRaycaster");
                return;
            }
            initialized = true;
        }
        

        private void FixedUpdate()
        {
            if (!initialized) return;
            HandleRaycast();
            HandleInput();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * maxRaycastDistance);
        }
        
        #endregion


        
        private void HandleRaycast()
        {
            var mousePos = Input.mousePosition;
            ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            
            // Check if we hit anything
            if (Physics.Raycast(ray, out hit, maxRaycastDistance, LayerMask.GetMask("Interactable")))
            {
                Debug.Log("1");

                var interactableObject = hit.collider.gameObject.GetComponent<Interactable>();
                
                // We hit something that is interactable
                if (interactableObject != null)
                {
                    Debug.Log("2");

                    // If the object we are looking at is not the same as the current object
                    if (interactableObject != currentInteractable)
                    {
                        // Disable the current object and enable the new object
                        LookAwayFromCurrentInteractable();
                        LookAtInteractable(interactableObject);
                    }
                } else 
                {
                    Debug.Log("3");

                    // We are not looking at an interactable object
                    // if(Input.mousePosition != mousePos)
                        LookAwayFromCurrentInteractable();
                }
            }
            else
            {
                
                Debug.Log("4");
                // We are not looking at anything
                // if(Input.mousePosition != mousePos)
                    LookAwayFromCurrentInteractable();
            }
        }

        private void HandleInput()
        {
            
            if (inputManager.Interact && !isInteracting)
            {
                isInteracting = true;
            }
            if (!inputManager.Interact && isInteracting)
            {
                isInteracting = false;
                if (currentInteractable != null)
                {
                    currentInteractable.Interact();
                    LookAwayFromCurrentInteractable();
                }
            }
        }
        
        private void LookAtInteractable(Interactable baseInteractableObject)
        {
            currentInteractable = baseInteractableObject;
            currentInteractable.OnLookAt();
            objectText.text = currentInteractable.DisplayInfo;
        }

        private void LookAwayFromCurrentInteractable()
        {
            if (currentInteractable == null) return;
            currentInteractable.OnLookAway();
            currentInteractable = null;
            objectText.text = "";
        }
    }
}