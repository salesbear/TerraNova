using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public static AudioManager instance;

    private void Awake()
    {
        //singleton design pattern
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// overloaded method so that you don't always need pitchRange
    /// varies pitch by +- 0.1f when variablePitch is turned on
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="volume"></param>
    public void PlaySound(AudioClip sound, float volume = 1, bool variablePitch = true)
    {
        if (variablePitch)
        {
            PlaySound(sound, new Vector2(-0.1f, 0.1f), volume);
        }
        else
        {
            PlaySound(sound, Vector2.zero, volume);
        }
    }

    /// <summary>
    /// plays the clip provided using the player audio source, volume can be passed in as an optional parameter
    /// </summary>
    /// <param name="audioClip"></param>
    /// <param name="pitchRange"></param>
    /// <param name="volume"></param>
    public void PlaySound(AudioClip audioClip, Vector2 pitchRange, float volume = 1)
    {
        audioSource.pitch = 1 + Random.Range(pitchRange.x, pitchRange.y);
        audioSource.volume = volume;
        audioSource.PlayOneShot(audioClip);
    }
}
