using System;
using SceneManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Roro.Scripts.SettingImplementations
{
    [CreateAssetMenu(fileName =" GeneralSettings" )]
    public class GeneralSettings : ScriptableObject
    {
        private static GeneralSettings _GeneralSettings;

        private static GeneralSettings generalSettings
        {
            get
            {
                if (!_GeneralSettings)
                {
                    _GeneralSettings = Resources.Load<GeneralSettings>($"Settings/GeneralSettings");

                    if (!_GeneralSettings)
                    {
#if UNITY_EDITOR
                        Debug.Log("General Settings not found AND NOT creating and a new one");
                        //_GeneralSettings = CreateInstance<GeneralSettings>();
                        // var path = "Assets/Resources/Settings/GeneralSettings.asset";
                        // AssetDatabaseHelpers.CreateAssetMkdir(_GeneralSettings, path);
#else
 				//		throw new Exception("Global settings could not be loaded");
#endif
                    }
                }

                return _GeneralSettings;
            }
        }
        
        public static GeneralSettings Get()
        {
            return generalSettings;
        }
        
        public bool IsInDebugMode = false;
        
        public float LoadingDuration = 2f;
        
        public float IntroSceneWaitDuration = 2f;
        
        public float LoadingFadeInDuration = 0.8f;

        public float ElevatorDuration = 1;
        
        

        #region InGame

        public float PlayerSpeed = 1f;

        #endregion



    }
}
