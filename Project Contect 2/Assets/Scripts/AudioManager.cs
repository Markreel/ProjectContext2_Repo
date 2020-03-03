using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioClip introAudio;
    [SerializeField] AudioClip endAudio;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this) { Destroy(gameObject); }
        }
        else { Instance = this; DontDestroyOnLoad(Instance); }
    }

    private void Start()
    {
        Time.timeScale = 0;
        UIManager.Instance.BlackScreen();

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.ignoreListenerPause = true;

        PlayIntroClip();
    }

    private void PlayIntroClip()
    {
        StartCoroutine(IEPlayIntroClip());
    }

    private IEnumerator IEPlayIntroClip()
    {
        audioSource.clip = introAudio;
        audioSource.Play();
        yield return new WaitForSecondsRealtime(introAudio.length);
        Time.timeScale = 1;

        UIManager.Instance.FadeIn(3);

        yield return null;
    }

    public void PlayOutroClip()
    {
        audioSource.clip = endAudio;
        audioSource.Play();
    }
}
