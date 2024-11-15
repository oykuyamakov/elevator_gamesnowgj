using System;
using System.Collections.Generic;
using _3rd_Party.Systems.StarterAssets.FirstPersonController.Scripts;
using DG.Tweening;
using Events;
using Roro.Scripts.GameManagement;
using SceneManagement.EventImplementations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace SceneManagement
{
    public class Elevator : MonoBehaviour
    {
        [SerializeField]
        private Transform m_ElevatorDoor;
        
        [SerializeField]
        private CallElevatorButton m_CallElevatorButton;
        
        private SceneId currentScene;
        
        private bool elevatorButtonPressed;
        public bool ElevatorUsed => elevatorButtonPressed;

        private bool isElevatorCalled;

        private Vector3 m_InitialDoorPosition;
        
        private FirstPersonController player;
        public bool IsPlayerIn => player != null;
        
        private void Awake()
        {
            if (!elevatorButtonPressed)
            {
                GEM.AddListener<SceneChangedEvent>(OnNewSceneLoaded);
                DontDestroyOnLoad(this);
            }
            
            m_InitialDoorPosition = m_ElevatorDoor.transform.position;

        }
        
        public void MoveElevator()
        {
            if(player == null)
                return;
            
            if (elevatorButtonPressed)
                return;
            
            elevatorButtonPressed = true;
            
            m_ElevatorDoor.DOMove(m_InitialDoorPosition, 0.5f).OnComplete(() =>
            {
                player.transform.position = Vector3.up;
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
                
                var newScene = GameManager.Instance.GetNewRandomScene();
                SceneLoader.Instance.ChangeScene(newScene);
                
                m_CallElevatorButton.gameObject.SetActive(false);
            });
        }
        
        public void OnElevatorCalled()
        {
            if(isElevatorCalled)
                return;
            
            m_ElevatorDoor.DOMove(m_InitialDoorPosition + Vector3.up * 10, 0.5f);
            isElevatorCalled = true;
        }
        
        private void OnNewSceneLoaded(SceneChangedEvent evt)
        {
            if (elevatorButtonPressed && SceneLoader.Instance.IsElevatorScene(evt.SceneId))
            {
                GEM.RemoveListener<SceneChangedEvent>(OnNewSceneLoaded);
                SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetSceneByName(evt.SceneId.GetName()));
                
                m_ElevatorDoor.DOMove(m_ElevatorDoor.transform.position + Vector3.up * 10, 0.5f);
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
