using System;
using System.Collections.Generic;
using System.Linq;
using Roro.Scripts.SettingImplementations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneController : MonoBehaviour
    {
        [SerializeField]
        private SceneId m_SceneId;
        public SceneId SceneId => m_SceneId;
        
        [SerializeField]
        private List<Canvas> m_Canvases;

        private SceneLoader sceneManager;

        private GeneralSettings settings;

        private void Awake()
        {
            //TODO add debug
            settings = GeneralSettings.Get();
            
            // if(settings.IsInDebugMode)
            //     return;
            
            sceneManager = SceneLoader.Instance;
            if (!sceneManager.SceneControllers.Contains(this))
            {
                sceneManager.SceneControllers.Add(this);   
            }
            else
            {
                throw new Exception("SceneController with SceneId " + m_SceneId + " already exists in SceneLoader");
            }
        }

        public void OnDeActiveScene()
        {
            foreach (var canvas in m_Canvases.TakeWhile(canvas => canvas != null))
            {
                canvas.enabled = false;
            }
        }
        
        public void OnActiveScene()
        {
            foreach (var canvas in m_Canvases.TakeWhile(canvas => canvas != null))
            {
                canvas.enabled = true;
            }
        }
    }
}