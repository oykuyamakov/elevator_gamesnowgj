using System;
using System.Collections.Generic;
using _3rd_Party.Systems.StarterAssets.FirstPersonController.Scripts;
using Events;
using InputManagement;
using Roro.Scripts.Serialization;
using Roro.Scripts.Sounds.Core;
using Roro.Scripts.Utility;
using SceneManagement;
using SceneManagement.EventImplementations;
using Sirenix.OdinInspector;
using UnityCommon.Modules;
using UnityCommon.Singletons;
using UnityCommon.Variables;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace Roro.Scripts.GameManagement
{
    [DefaultExecutionOrder(ExecOrder.GameManager)]
    public class GameManager : SingletonBehaviour<GameManager>
    {
        [SerializeField]
        private int m_TargetFrameRate = 60;

        [SerializeField] 
        private BoolVariable m_GameIsRunning;
        public BoolVariable GameIsRunning => m_GameIsRunning;
        
        private FirstPersonController player;
    
        private List<SceneId> notDiscoveredScenes = new List<SceneId>()
        {
            SceneId.Oyku_Rave,
            SceneId.Oyku_Gossip,
        };

        [Button]
        public void ToggleGame()
        {
            m_GameIsRunning.Value = !m_GameIsRunning.Value;
        }

        private void OnEnable()
        {
            GEM.AddListener<SceneChangedEvent>(OnSceneLoaded);
        }

        private void Awake()
        {
            if (!SetupInstance())
                return;

            Variable.Initialize();
            
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            Application.targetFrameRate = m_TargetFrameRate;
            
            ConditionalsModule.CreateSingletonInstance();
            
            m_GameIsRunning =  Variable.Get<BoolVariable>("GameIsRunning");
        }
        
        private void OnSceneLoaded(SceneChangedEvent evt)
        {
            Debug.Log(evt.SceneId);

            if (!notDiscoveredScenes.Contains(evt.SceneId) && evt.SceneId != SceneId.Tutorial) 
                return;
            
            
            if(player == null)
                player = FindObjectOfType<FirstPersonController>();
            
            InputManager.Instance.CursorLocked = true;
            
            // if(player.gameObject.activeSelf) 
            //     return;         
            
            player.gameObject.SetActive(true);


        }
        
        public void SetPlayer(FirstPersonController player)
        {
            this.player = player;
            player.gameObject.SetActive(false);
        }

        public void ResetNotDiscoveredScenes()
        {
            notDiscoveredScenes = new List<SceneId>()
            {
                SceneId.Oyku_Rave,
                SceneId.Oyku_Gossip,
            };
        }
        
        public SceneId GetNewRandomScene()
        {
            var newScene = notDiscoveredScenes[Random.Range(0, notDiscoveredScenes.Count)];
            notDiscoveredScenes.Remove(newScene);
            return newScene;
        }

        private void OnDisable()
        {
            GEM.RemoveListener<SceneChangedEvent>(OnSceneLoaded);
        }
    }
}