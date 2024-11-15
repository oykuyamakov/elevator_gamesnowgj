using SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Button m_PlayButton;

        private bool playButtonPressed;
        
        private void Awake()
        {
            m_PlayButton.onClick.RemoveAllListeners();
            m_PlayButton.onClick.AddListener(OnPlayButton);
            m_PlayButton.enabled = true;
        }
        
        public void OnPlayButton()
        {
            if(playButtonPressed)
                return;
            
            playButtonPressed = true;
            SceneLoader.Instance.ChangeScene(SceneId.Tutorial);
            
            m_PlayButton.enabled = false;
        }
    }
}
