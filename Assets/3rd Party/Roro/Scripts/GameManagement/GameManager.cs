using System;
using System.Collections.Generic;
using _3rd_Party.Systems.StarterAssets.FirstPersonController.Scripts;
using Events;
using InputManagement;
using Roro.Scripts.Serialization;
using Roro.Scripts.SettingImplementations;
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

        private List<SceneId> notDiscoveredScenes;

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

            notDiscoveredScenes = SceneLoader.Instance.ElevatorScenes;
            
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            Application.backgroundLoadingPriority = ThreadPriority.Normal;

            Application.targetFrameRate = m_TargetFrameRate;
            
            ConditionalsModule.CreateSingletonInstance();
            
            m_GameIsRunning =  Variable.Get<BoolVariable>("GameIsRunning");
        }
        
        private void OnSceneLoaded(SceneChangedEvent evt)
        {
            if (!notDiscoveredScenes.Contains(evt.SceneId) && evt.SceneId != SceneId.Tutorial) 
                return;
            
            if(player == null)
                player = FindObjectOfType<FirstPersonController>();
            
            InputManager.Instance.CursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            
            // if(player.gameObject.activeSelf) 
            //     return;         
            
            player.gameObject.SetActive(true);

        }
        
        public void SetPlayer(FirstPersonController player)
        {
            if (this.player != null)
            {
                Destroy(this.player.gameObject);
                return;
            }
            
            this.player = player;
            
            if(GeneralSettings.Get().IsInDebugMode)
                return;
            
            player.gameObject.SetActive(false);
        }
        public FirstPersonController GetPlayer()
        {
            return player;
        }

        public void ResetNotDiscoveredScenes()
        {
            notDiscoveredScenes = SceneLoader.Instance.ElevatorScenes;
        }
        
        public SceneId GetNewRandomScene()
        {
            if(notDiscoveredScenes.Count<=0)
            {
                return SceneId.Ending;
            }

            if (SceneLoader.Instance.CurrentScene == SceneId.Tutorial)
            {
                notDiscoveredScenes.Remove(SceneId.Introduction);
                return SceneId.Introduction;
            }
            
            if (SceneLoader.Instance.CurrentScene == SceneId.Introduction)
            {
                notDiscoveredScenes.Remove(SceneId.Sauna);
                return SceneId.Sauna;
            }
            
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