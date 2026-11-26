using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicFadeIn : MonoBehaviour
{
    [SerializeField] private float targetVolume = 0.4f;
    [SerializeField] private float fadeTime = 3f;
    [SerializeField] private float startDelay = 0f;

    private AudioSource source;

    IEnumerator Start()
    {
        source = GetComponent<AudioSource>();
        source.volume = 0f;
        source.loop = true;

        if (startDelay > 0f) yield return new WaitForSeconds(startDelay);

        source.Play();

        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, targetVolume, t / fadeTime);
            yield return null;
        }

        source.volume = targetVolume;
    }
}