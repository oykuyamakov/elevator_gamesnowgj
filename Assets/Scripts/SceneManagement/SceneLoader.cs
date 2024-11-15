using System.Collections;
using System.Collections.Generic;
using Events;
using Roro.Scripts.SettingImplementations;
using SceneManagement.EventImplementations;
using Sirenix.OdinInspector;
using UnityCommon.Runtime.UI;
using UnityCommon.Singletons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneManagement
{
    public class SceneLoader : SingletonBehaviour<SceneLoader>
    {
        public static float LoadingProgress { get; private set; }
        
        public List<SceneController> SceneControllers => sceneControllers;
        
        private List<SceneController> sceneControllers = new List<SceneController>();
        
        private SceneId currentActiveScene;
        
        private GeneralSettings settings => GeneralSettings.Get();

        private bool loading;

        private Dictionary<SceneId, bool> mainScenesByIds = new Dictionary<SceneId, bool>()
        {
            {SceneId.Intro, true},
            {SceneId.Loading, false},
            {SceneId.Shared, false},
            {SceneId.MainMenu, true},
            {SceneId.Tutorial, true},
            {SceneId.Oyku_Rave, true},
            {SceneId.Oyku_Gossip, true},
        };

        private HashSet<SceneId> elevatorScenes = new HashSet<SceneId>()
        {
            SceneId.Oyku_Rave,
            SceneId.Oyku_Gossip
        };
        
        private void Awake()
        {
            if(!SetupInstance())
                return;
            
            currentActiveScene = SceneId.Intro;
            LoadGame();
        }
        
        private void LoadGame()
        {
            StartCoroutine(LoadScene(SceneId.Shared, false));
            StartCoroutine(LoadScene(SceneId.Loading, false));
            //StartCoroutine(LoadScene(SceneId.MainMenu, true));
        }
        
        [Button]
        public void ChangeScene(SceneId sceneId)
        {
            StartCoroutine(LoadScene(sceneId, true));
        }
        
        private IEnumerator ActivateLoadingScene(SceneId nextScene)
        {
            loading = true;
            
            StartCoroutine(ActivateScene(SceneId.Loading, false));
            var dur = elevatorScenes.Contains(nextScene) ? settings.ElevatorDuration : settings.LoadingDuration;
            yield return new WaitForSeconds(dur);
            loading = false;
        }

        private IEnumerator LoadScene(SceneId sceneId, bool activate)
        {
            if (mainScenesByIds[sceneId])
            {
                // FadeInOut.Instance.DoTransition(() =>
                // { }, settings.LoadingFadeInDuration);

                //yield return new WaitForSeconds(settings.LoadingFadeInDuration/2);
                
                StartCoroutine(ActivateLoadingScene(sceneId));
            }
            
            var sceneToLoad = sceneId.GetScene();
            
            if (sceneToLoad.IsValid())
            {
                //Debug.Log($"The scene is already loaded {sceneId.ToString()}");

                if (activate)
                {
                    //Debug.Log($"will activate it now. {sceneId.ToString()}");
                    StartCoroutine(ActivateScene(sceneId, mainScenesByIds[sceneId]));
                }
                yield break; 
            }
            
            yield return null;
            
            var sceneName = sceneId.GetName();
            var asyncOp = SceneManager.LoadSceneAsync(
                sceneName, new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.None));
            
            while (!asyncOp.isDone)
            {
                LoadingProgress = asyncOp.progress;
                //Debug.Log($"Loading progress: {LoadingProgress}");
                yield return null;
            }
            
            LoadingProgress = 1f;
            yield return null;
            
            //Debug.Log($"{sceneId.ToString()} scene is loaded." );

            if (activate)
            {
                //Debug.Log($"will activate it now. {sceneId.ToString()}");
                yield return StartCoroutine(ActivateScene(sceneId, mainScenesByIds[sceneId]));
            }
        }

        private IEnumerator ActivateScene(SceneId sceneId,bool waitForLoading)
        {
            if (waitForLoading)
            {
                StartCoroutine(UnLoadScene(currentActiveScene));
                //The scene will be activated after the loading time passes so we break now
                yield return new WaitUntil(() => loading == false);
            }

            if (sceneId != SceneId.Loading)
            {
                // FadeInOut.Instance.DoTransition(() =>
                // {
                    foreach (var controller in sceneControllers)
                    {
                        if(controller.SceneId == sceneId)
                            continue;
                
                        controller.OnDeActiveScene();
                    }
                
                //}, settings.LoadingFadeOutDuration);
                
            }
            
            Debug.Log(sceneId.ToString() + " is being activated");
            SceneManager.SetActiveScene(sceneId.GetScene());
            if(sceneControllers.Exists(x => x.SceneId == sceneId))
                sceneControllers.Find(x => x.SceneId == sceneId).OnActiveScene();
            
            SendSceneChangedEvent(sceneId);

            
            if (waitForLoading)
            {
                currentActiveScene = sceneId;
                //Debug.Log("Active Scene is now: " + currentActiveScene.ToString());
            }
        }
        
        private IEnumerator UnLoadScene(SceneId sceneId)
        {
            if (sceneId != SceneId.None)
            {
                var sceneToLoad = sceneId.GetScene();
                if (!sceneToLoad.IsValid())
                {
                    Debug.LogError($"The scene: {sceneId.ToString()} that you are trying to unload is not loaded.");
                }
                else
                {
                    yield return null;
                    
                    SceneManager.UnloadSceneAsync(sceneToLoad);

                    yield return null;

                    //Debug.Log($"Unloaded : {sceneId.ToString()}" );
                }
            }
            else
            {
                Debug.LogError("Scene ID cannot be \"None\", nothing to unload");

                yield return null;
            }
        }
        
        private void SendSceneChangedEvent(SceneId sceneId)
        {
            using var evt = SceneChangedEvent.Get(sceneId);
            evt.SendGlobal();
        }
    }

    public enum SceneId
    {
        MainMenu = 1,
        Loading = 2,
        Intro = 4,
        Shared = 8,
        None = 16,
        Tutorial = 32,
        Oyku_Rave = 64,
        Oyku_Gossip = 128,
    }
    
    public static class SceneExtensions
    {
        public static Scene GetScene(this SceneId id)
        {
            return SceneManager.GetSceneByName(id.ToString());
        }
        
        public static string GetName(this SceneId id)
        {
            return id.ToString();
        }
    }
    
    public static class Shared
    {
        public static Camera MainCamera => m_Cam == null ? m_Cam = Camera.main : m_Cam;
        private static Camera m_Cam;
    }
}
