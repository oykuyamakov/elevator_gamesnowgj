using System;
using Roro.Scripts.SettingImplementations;
using UnityCommon.Modules;
using UnityEngine;

namespace SceneManagement
{
    public class IntroManager : MonoBehaviour
    {

        private GeneralSettings settings;
        
        private void Start()
        {
            settings = GeneralSettings.Get();
            LoadMainMenu();
        }
        
        private void LoadMainMenu()
        {
            Conditional.Wait(settings.IntroSceneWaitDuration).Do(() =>
            {
                SceneLoader.Instance.ChangeScene(SceneId.MainMenu);
            });
        }
    }
}
