using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public SceneId CurrentScene => currentActiveScene;
        
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
            {SceneId.Sauna, true},
            {SceneId.Swamp, true},
            {SceneId.BDSM_Room, true},
            {SceneId.Oyku_Shinning, true},
            {SceneId.Car, true},
            {SceneId.Ending, true},
            {SceneId.Introduction, true},
            {SceneId.Networking, true},
        };

        private List<SceneId> ScenesForElevator = new List<SceneId>()
        {
            SceneId.Oyku_Shinning,
            SceneId.Swamp,
            SceneId.BDSM_Room,
            SceneId.Oyku_Rave,
            SceneId.Car,
            SceneId.Networking,
            SceneId.Introduction,
            SceneId.Sauna,
        };

        private HashSet<SceneId> elevatorScenes => ScenesForElevator.ToHashSet();
        
        public bool IsElevatorScene(SceneId id) => elevatorScenes.Contains(id);

        public List<SceneId> ElevatorScenes { get; private set; }
        
        private void Awake()
        {
            if(!SetupInstance())
                return;

            ElevatorScenes = elevatorScenes.ToList();
            
            currentActiveScene = SceneId.Intro;
            LoadGame();
        }
        
        private void LoadGame()
        {
            StartCoroutine(LoadScene(SceneId.Shared, false));
            StartCoroutine(LoadScene(SceneId.Loading, false));
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
            
            if (sceneToLoad.isLoaded)
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
            
            SceneManager.SetActiveScene(sceneId.GetScene());
            if(sceneControllers.Exists(x => x.SceneId == sceneId))
                sceneControllers.Find(x => x.SceneId == sceneId).OnActiveScene();


            SendSceneChangedEvent(sceneId);

            
            if (waitForLoading)
            {
                currentActiveScene = sceneId;

                Debug.Log("Active Scene is now: " + currentActiveScene.ToString());
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
        Sauna = 256,
        Swamp = 512,
        BDSM_Room = 1024,
        Oyku_Shinning = 2048,
        Ending = 4096,
        Car = 8192,
        Networking = 16384,
        Introduction = 32768,
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
