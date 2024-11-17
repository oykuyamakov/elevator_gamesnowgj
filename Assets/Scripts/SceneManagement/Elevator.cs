
using System.Collections;
using _3rd_Party.Systems.StarterAssets.FirstPersonController.Scripts;
using Cinemachine;
using DG.Tweening;
using Events;
using Roro.Scripts.GameManagement;
using SceneManagement.EventImplementations;
using UnityCommon.Modules;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class Elevator : MonoBehaviour
    {
        [SerializeField]
        private Animator m_ElevatorDoorAnimator;
        
        [SerializeField]
        private CallElevatorButton m_CallElevatorButton;
        
        private SceneId currentScene;
        
        private bool elevatorButtonPressed;
        public bool ElevatorUsed => elevatorButtonPressed;

        private bool isElevatorCalled;
        
        private FirstPersonController player;
        public bool IsPlayerIn => player != null;
        
        private void Awake()
        {
            if (!elevatorButtonPressed)
            {
                GEM.AddListener<SceneChangedEvent>(OnNewSceneLoaded);
                DontDestroyOnLoad(this);
            }
        }
        
        public void MoveElevator()
        {
            if(player == null)
                return;
            
            if (elevatorButtonPressed)
                return;
            
            elevatorButtonPressed = true;
            
            m_ElevatorDoorAnimator.SetBool("Open", false);

            StartCoroutine(Aminakoyayim());
        }

        private IEnumerator Aminakoyayim()
        {
            var newScene = GameManager.Instance.GetNewRandomScene();

            using var evt = ElevatorInEvent.Get(newScene);
            evt.SendGlobal();
            
            GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().isKinematic = true;
            
            yield return new WaitForSeconds(0.5f);
            
            var volume = GameManager.Instance.GetPlayer().plCamera.GetComponent<Volume>();
            
            if(volume.profile.TryGet(out Vignette vin))
            {
                DOTween.To(() => vin.intensity.value, x => vin.intensity.value = x, 0.5f, 1f);
            }
            //
            // Debug.Log(vin.intensity.value);
            //
            yield return new WaitForSeconds(2f);
            
            SceneLoader.Instance.ChangeScene(newScene);
            
            GameManager.Instance.GetPlayer().transform.position = new Vector3(1.5f,1.5f,0f);
            
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.DORotate( newScene == SceneId.Networking ? new Vector3(0,180,0) : Vector3.zero, 1f);
            
            yield return new WaitForSeconds(1f);
            
            GameManager.Instance.GetPlayer().enabled = false;
            GameManager.Instance.GetPlayer().transform.position = new Vector3(1.5f,1.5f,0f);
            GameManager.Instance.GetPlayer().enabled = true;
            
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.DORotate( newScene == SceneId.Networking ? new Vector3(0,180,0) : Vector3.zero, 1f);
            Debug.Log("ananisikem");
            
            GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().isKinematic = false;

            
            DOTween.To(() => vin.intensity.value, x => vin.intensity.value = x, 0f, 1f).SetEase(Ease.InOutSine);
                
            m_CallElevatorButton.gameObject.SetActive(false);
        }
        
        public void OnElevatorCalled()
        {
            if(isElevatorCalled)
                return;
            
            m_ElevatorDoorAnimator.SetBool("Open", true);
            
            isElevatorCalled = true;
        }
        
        private void OnNewSceneLoaded(SceneChangedEvent evt)
        {
            if (elevatorButtonPressed && (SceneLoader.Instance.IsElevatorScene(evt.SceneId) || SceneId.Ending == evt.SceneId))
            {
                GEM.RemoveListener<SceneChangedEvent>(OnNewSceneLoaded);
                SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByName(evt.SceneId.GetName()));
                
                m_ElevatorDoorAnimator.SetBool("Open", true);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<FirstPersonController>(out var pl))
            {
                player = pl;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<FirstPersonController>(out var pl))
            {
                player = null;
            }
        }
    }
}
