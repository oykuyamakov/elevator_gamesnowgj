using System;
using Events;
using SceneManagement;
using SceneManagement.EventImplementations;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class LoadingUI : MonoBehaviour
    {
        [SerializeField]
        private Image m_LoadingBG;
        
        private void Awake()
        {
            GEM.AddListener<SceneChangedEvent>(OnSceneChangedEvent);
        }

        private void OnDisable()
        {
            GEM.RemoveListener<SceneChangedEvent>(OnSceneChangedEvent);
        }

        private void OnSceneChangedEvent(SceneChangedEvent evt)
        {
            if(evt.SceneId == SceneId.Tutorial)
                OnPassMainMenu();
        }
        
        private void OnPassMainMenu()
        {
            m_LoadingBG.enabled = false;
        }
    }
}
