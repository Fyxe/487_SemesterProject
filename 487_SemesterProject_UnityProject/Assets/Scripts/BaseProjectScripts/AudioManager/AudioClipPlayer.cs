using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlayer : PooledObject {

    [Header("Audio Clip Player")]
    public AudioSource source;

    /// <summary>
    /// Plays an audio clip within the given parameters.
    /// </summary>
    /// <param name="clip">The audio clip to be played.</param>
    /// <param name="minVolume">The minimum volume of the clip. (To be combined with the audio managers volume modifier)</param>
    /// <param name="maxVolume">The maximum volume of the clip. (To be combined with the audio managers volume modifier)</param>
    /// <param name="minPitch">The minimum pitch of the clip.</param>
    /// <param name="maxPitch">The maximum pitch of the clip.</param>
    /// <param name="minPan">The minimum left to right pan of the clip.</param>
    /// <param name="maxPan">The maximum left to right pan of the clip.</param>
    public void PlaySoundRandom(AudioClip clip, float minVolume, float maxVolume, float minPitch, float maxPitch, float minPan, float maxPan)
    {
        name = "Source randomized for: " + clip.name;
        source.clip = clip;
        source.pitch = Random.Range(minPitch, maxPitch);
        source.volume = Random.Range(minVolume, maxVolume) * AudioManager.instance.volumeModifier;
        source.panStereo = Random.Range(minPan, maxPan);
        StartCoroutine(PlaySoundCoroutine());
    }

    /// <summary>
    /// Plays an audio clip with the given parameters.
    /// </summary>
    /// <param name="clip">The audio clip to be played.</param>
    /// <param name="volume">The volume of the clip. (To be combined with the audio managers volume modifier)</param>
    /// <param name="pitch">The pitch of the clip.</param>
    /// <param name="pan">The left to right pan of the clip.</param>
    public void PlaySound(AudioClip clip, float volume, float pitch, float pan)
    {
        name = "Source for: " + clip.name;
        source.clip = clip;
        source.pitch = pitch;
        source.volume = volume * AudioManager.instance.volumeModifier;
        source.panStereo = pan;
        StartCoroutine(PlaySoundCoroutine());
    }

    /// <summary>
    /// Plays a sound and 'destroys' this object upon it's completion.
    /// </summary>
    /// <returns></returns>
    IEnumerator PlaySoundCoroutine()
    {
        source.Play();

        while(source.isPlaying)
        {
            yield return null;
        }

        DestroyThisObject();
    }
}
