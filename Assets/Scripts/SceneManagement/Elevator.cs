using System;
using System.Collections.Generic;
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
        private SceneId currentScene;
        
        private bool elevatorButtonPressed;
        public bool ElevatorUsed => elevatorButtonPressed;

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
            if (elevatorButtonPressed)
                return;
            
            elevatorButtonPressed = true;
            
            //Close Doors
            
            // Elevator button pressed
            Debug.Log("Elevator button pressed");
            var newScene = GameManager.Instance.GetNewRandomScene();
            SceneLoader.Instance.ChangeScene(newScene);
            //Open Doors After
        }
        
        private void OnNewSceneLoaded(SceneChangedEvent evt)
        {
            if (elevatorButtonPressed)
            {
                GEM.RemoveListener<SceneChangedEvent>(OnNewSceneLoaded);
                SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
            }
        }
        
    }
}
