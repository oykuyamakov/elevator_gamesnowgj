using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Events;
using SceneManagement;
using SceneManagement.EventImplementations;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField]
    private List<SceneMusic> m_SceneMusics = new List<SceneMusic>();

    private AudioSource m_AudioSource => GetComponent<AudioSource>();

    private SceneId prevScene;
    
    private Dictionary<SceneId,AudioClip> m_SceneMusicDictionary = new Dictionary<SceneId, AudioClip>();

    private void Awake()
    {
        foreach (var sceneMusic in m_SceneMusics)
        {
            m_SceneMusicDictionary.Add(sceneMusic.sceneId, sceneMusic.music);
        }
        
        GEM.AddListener<ElevatorInEvent>(OnElevatorIn);
    }

    private void OnDisable()
    {
        GEM.RemoveListener<ElevatorInEvent>(OnElevatorIn);
    }

    public void OnElevatorIn(ElevatorInEvent evt)
    {
        var nextScene = evt.nexSceneId;
        
        if(m_SceneMusicDictionary.ContainsKey(nextScene))
        {
            if(m_SceneMusicDictionary[prevScene] != m_SceneMusicDictionary[nextScene])
            {
                m_AudioSource.DOKill();
                
                m_AudioSource.DOFade(0f, 0.1f).OnComplete(() =>
                {
                    m_AudioSource.volume = 1f;
                    m_AudioSource.clip = m_SceneMusicDictionary[nextScene];
                    m_AudioSource.Play();
                });
                
            }
        }
    }
    
    
    
    
}

[Serializable]
public class SceneMusic
{
    public SceneId sceneId;
    public AudioClip music;
}
