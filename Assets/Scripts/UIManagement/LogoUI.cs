using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using SceneManagement;
using SceneManagement.EventImplementations;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace UIManagement
{
    public class LogoUI : MonoBehaviour
    {
        [SerializeField]
        private Image m_LogoImage;
        
        [SerializeField] 
        private List<SceneLogo> m_SceneLogos = new List<SceneLogo>();
        
        private Dictionary<SceneId, Sprite> m_SceneLogoDictionary = new Dictionary<SceneId, Sprite>();
        
        private void Awake()
        {
           
        }

        private void OnEnable()
        {
            //Debug.Log("WAKE");
            
            m_SceneLogoDictionary.Clear();
            foreach (var sceneLogo in m_SceneLogos)
            {
                m_SceneLogoDictionary.Add(sceneLogo.sceneId, sceneLogo.logo);
            }
            
            GEM.AddListener<SceneChangedEvent>(OnSceneLoaded, Priority.VeryLow);
        }

        private void OnDisable()
        {
            //Debug.Log("dEAD");
            
            GEM.RemoveListener<SceneChangedEvent>(OnSceneLoaded);
        }

        private void OnSceneLoaded(SceneChangedEvent evt)
        {
            var id = evt.SceneId;
            if (m_SceneLogoDictionary.ContainsKey(id))
            {
                StartCoroutine(EnableLogo(m_SceneLogoDictionary[id]));
            }
        }

        private IEnumerator EnableLogo(Sprite logo)
        {
            m_LogoImage.sprite = logo;
            m_LogoImage.enabled = true;

            yield return new WaitForSeconds(3f);
            
            m_LogoImage.enabled = false;
            m_LogoImage.sprite = null;
        }
        
    }

    [Serializable]
    public class SceneLogo
    {
        public SceneId sceneId;
        public Sprite logo;
    }
}
