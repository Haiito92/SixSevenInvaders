using System;
using System.Collections;
using NaughtyAttributes;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_musicSource;
    [SerializeField, Range(0.1f, 100.0f)] private float  m_musicFadeOutSpeed = 1.0f;
    [SerializeField, Range(0.1f, 100.0f)] private float  m_musicFadeInSpeed = 1.0f;
    
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
        
        if(!m_musicSource) return;
        m_musicSource.volume = 0.0f;
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
        if (m_musicSource.clip)
        {
            while (m_musicSource.volume > 0.0f)
            {
                m_musicSource.volume = Math.Max(m_musicSource.volume -  Time.deltaTime / m_musicFadeOutSpeed, 0.0f);
                yield return null;
            }
        
            m_musicSource.Stop();
            m_musicSource.clip = null;
        }

        if (newMusic)
        {
            m_musicSource.clip = newMusic;
            m_musicSource.Play();

            while (m_musicSource.volume < 1.0f)
            {
                m_musicSource.volume = Math.Min(m_musicSource.volume + Time.deltaTime / m_musicFadeOutSpeed, 1.0f);
                yield return null;
            }
        }
    }

    private void StopChangeMusic()
    {
        if (m_changeMusicCoroutine != null)
        {
            StopCoroutine(m_changeMusicCoroutine);
            m_changeMusicCoroutine = null;
        }
    }
    
    [Header("Tests")]
    [SerializeField] private AudioClip m_testMusicClip;
    [SerializeField] private AudioClip m_testMusicClip2;

    [Button]
    public void TestPlayMusic() => PlayMusic(m_testMusicClip);
    [Button]
    public void TestPlayMusic2() => PlayMusic(m_testMusicClip2);
    [Button]
    public void TestStopMusic() => StopMusic();

}
