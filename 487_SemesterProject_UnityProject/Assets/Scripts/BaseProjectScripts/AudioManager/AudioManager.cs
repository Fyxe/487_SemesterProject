using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager> {


    [Header("Audio Manager")]
    public bool isFading = false;
    public AudioSource backgroundSourceA;
    public AudioSource backgroundSourceB;
    public float volumeModifier = 1f;
    public float backgroundVolumeLevel = 1f;
    public float fadeTime = 2f;

    [Header("Prefabs")]
    public PooledObject audioClipPlayer;

    protected override void Initialize()
    {
        name = "AudioManager";    
    }

    /// <summary>
    /// Creates an AudioClipPlayer and plays it in local space.
    /// </summary>
    /// <param name="toPlay">The audio clip to be played.</param>    
    /// <param name="minVolume">The minimum volume of the clip. (To be combined with the audio managers volume modifier)</param>
    /// <param name="maxVolume">The maximum volume of the clip. (To be combined with the audio managers volume modifier)</param>
    /// <param name="minPitch">The minimum pitch of the clip.</param>
    /// <param name="maxPitch">The maximum pitch of the clip.</param>
    /// <param name="minPan">The minimum left to right pan of the clip.</param>
    /// <param name="maxPan">The maximum left to right pan of the clip.</param>
    public bool PlayClipLocalSpace(AudioClip toPlay, float minVolume = 1f, float maxVolume = 1f, float minPitch = 1f, float maxPitch = 1f, float minPan = 0f, float maxPan = 0f)
    {
        if (toPlay == null)
        {
            Debug.LogError("AudioManager cannot play a null clip in local space.");
            return false;
        }
        PooledObject obj = ObjectPoolingManager.instance.CreateObject(audioClipPlayer, Camera.main.transform, -1);
        obj.transform.Reset();
        AudioClipPlayer clipPlayer = obj.GetComponent<AudioClipPlayer>();        
        clipPlayer.PlaySoundRandom(toPlay,minVolume,maxVolume,minPitch,maxPitch,minPan,maxPan);
        return true;
    }

    /// <summary>
    /// Creates an AudioClipPlayer and plays it in a given world space.
    /// </summary>
    /// <param name="toPlay">The audio clip to be played.</param>    
    /// <param name="position">The position in world space to play the audio from.</param>
    /// <param name="minVolume">The minimum volume of the clip. (To be combined with the audio managers volume modifier)</param>
    /// <param name="maxVolume">The maximum volume of the clip. (To be combined with the audio managers volume modifier)</param>
    /// <param name="minPitch">The minimum pitch of the clip.</param>
    /// <param name="maxPitch">The maximum pitch of the clip.</param>
    /// <param name="minPan">The minimum left to right pan of the clip.</param>
    /// <param name="maxPan">The maximum left to right pan of the clip.</param>
    public bool PlayClipWorldSpace(AudioClip toPlay, Vector3 position, float minVolume = 1f, float maxVolume = 1f, float minPitch = 1f, float maxPitch = 1f, float minPan = 0f, float maxPan = 0f)
    {
        if (toPlay == null)
        {
            Debug.LogError("AudioManager cannot play a null clip in the world space position " + position.ToString());
            return false;
        }
        PooledObject obj = ObjectPoolingManager.instance.CreateObject(audioClipPlayer);
        obj.transform.position = position;
        AudioClipPlayer clipPlayer = obj.GetComponent<AudioClipPlayer>();
        clipPlayer.PlaySoundRandom(toPlay, minVolume, maxVolume, minPitch, maxPitch, minPan, maxPan);
        return true;
    }

    /// <summary>
    /// Plays a looped local audioclip. If a clip is already playing it will fade the old one out and fade the new one in.
    /// </summary>
    /// <param name="toPlay">The audio clip to be played.</param>
    /// <param name="difFadeTime">The time in which the audio will be faded. If (-1) the default fade time will be used.</param>
    /// <returns>Returns true if the clip was played.</returns>
    public bool PlayClipLocalLooped(AudioClip toPlay, float difFadeTime = -1f)
    {
        if (toPlay == null)
        {
            Debug.Log("AudioManager cannot play a null clip looped in the background.");
            return false;
        }
        else if (isFading)
        {
            // TODO - add this clip to a list that we fade in after this fading is done.
            // TODO - add an option to interrupt the clip with the new fade in.
            return false;
        }
        else
        {
            float endTime = fadeTime;
            if (difFadeTime > 0)
            {
                endTime = difFadeTime;
            }

            StartCoroutine(PlayClipLocalLoopedCoroutine(toPlay, endTime));
            return true;
        }       
    }

    /// <summary>
    /// Fades in a new clip into local space and fades out the current clip.
    /// </summary>
    /// <param name="toPlay">The new clip to play.</param>
    /// <param name="endTime">The length of time that the fade will take.</param>
    /// <returns></returns>
    IEnumerator PlayClipLocalLoopedCoroutine(AudioClip toPlay, float endTime)
    {
        isFading = true;
        if (!backgroundSourceA.isPlaying && !backgroundSourceB.isPlaying)
        {
            backgroundSourceA.clip = toPlay;
            backgroundSourceA.Play();
            float currentTime = 0f;
            while (currentTime < endTime)
            {
                currentTime += Time.deltaTime;
                backgroundSourceA.volume = Mathf.Lerp(backgroundSourceA.volume, backgroundVolumeLevel * volumeModifier, currentTime / endTime);
                yield return null;
            }
            backgroundSourceA.volume = backgroundVolumeLevel * volumeModifier;
        }
        else
        {
            AudioSource fadingIn = backgroundSourceA;
            AudioSource fadingOut = backgroundSourceB;
            if (backgroundSourceA.isPlaying)
            {
                fadingIn = backgroundSourceB;
                fadingOut = backgroundSourceA;
            }

            float currentTime = 0f;
            while (currentTime < (endTime / 2f))
            {
                currentTime += Time.deltaTime;
                fadingOut.volume = Mathf.Lerp(fadingOut.volume, 0f, currentTime / (endTime / 2f));
                yield return null;
            }
            fadingOut.Stop();

            fadingIn.clip = toPlay;
            fadingIn.Play();
            currentTime = 0f;
            while (currentTime < (endTime / 2f))
            {
                currentTime += Time.deltaTime;
                fadingIn.volume = Mathf.Lerp(fadingIn.volume, backgroundVolumeLevel * volumeModifier, currentTime / (endTime / 2f));
                yield return null;
            }
            fadingIn.volume = backgroundVolumeLevel * volumeModifier;
        }
        
        isFading = false;
    }


}
