using UnityEngine;

public class SoundComponent : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField, Range(0.0f, 1.0f)] private float volume = 1.0f;
    [SerializeField, Range(-3.0f, 3.0f)] private float pitch = 1.0f;
    
    public void PlayMusic()
    {
        if (!clip || !SoundManager.Instance) return;
        
        SoundManager.Instance.PlayMusic(clip);
    }
    
    public void PlaySFX2D()
    {
        if (!clip || !SoundManager.Instance) return;
        
        SoundManager.Instance.PlaySFX2D(clip, volume, pitch);
    }
    
    public void PlaySFX3D()
    {
        if (!clip || !SoundManager.Instance) return;
        SoundManager.Instance.PlaySFX3D(clip, transform.position, volume, pitch);
    }
}
