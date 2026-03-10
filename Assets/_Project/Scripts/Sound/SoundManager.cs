using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_musicSource;

    private Coroutine m_changeMusicCoroutine;

    #region Singleton
    
    private static SoundManager m_instance;
    public static SoundManager Instance => m_instance;

    private void InitSingleton()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }
    }
    #endregion
    
    private void Awake()
    {
        InitSingleton();
    }

    public void PlayMusic(AudioClip newMusic)
    {
        if(!m_musicSource) return;
        if(m_musicSource.clip == newMusic) return;

        StartChangeMusic(newMusic);
    }

    public void StopMusic()
    {
        if(!m_musicSource || !m_musicSource.clip) return;

        StartChangeMusic(null);
    }

    private void StartChangeMusic(AudioClip newMusic)
    {
        StopChangeMusic();
        
        m_changeMusicCoroutine = StartCoroutine(ChangeMusic(newMusic));
    }
    
    private IEnumerator ChangeMusic(AudioClip newMusic)
    {
        m_musicSource.Stop();
        
        m_musicSource.clip = null;

        if (newMusic)
        {
            m_musicSource.clip = newMusic;
            m_musicSource.Play();
        }
        
        yield return null;
    }

    private void StopChangeMusic()
    {
        if (m_changeMusicCoroutine != null)
        {
            StopCoroutine(m_changeMusicCoroutine);
            m_changeMusicCoroutine = null;
        }
    }
    
    [SerializeField] private AudioClip m_testMusicClip;

    [Button]
    public void TestPlayMusic() => PlayMusic(m_testMusicClip);
    
    [Button]
    public void TestStopMusic() => StopMusic();
}
