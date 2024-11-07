using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSoundManager : MonoBehaviour
{
    private AudioSource src;

    private void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, float delay, float pitchAdded, bool randomPitch, float spatialBlend)
    {
        StartCoroutine(Play(clip, delay, pitchAdded, randomPitch, spatialBlend));
    }

    private IEnumerator Play(AudioClip clip, float delay, float pitch, bool randomPitch, float spatialBlend)
    {
        if (!clip) yield return null;
        yield return new WaitForSeconds(delay);
        src.spatialBlend = spatialBlend;
        float pitchAdded = randomPitch ? Random.Range(-pitch, pitch) : pitch;
        src.pitch = 1 + pitchAdded;
        src.PlayOneShot(clip);
        yield return null;
    }
}
