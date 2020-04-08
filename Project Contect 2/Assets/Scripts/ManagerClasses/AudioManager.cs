using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioClip clipToPlayOnAwake;
    [SerializeField] AudioClip musicClipA;
    [SerializeField] AudioClip creditsMusic;
    [SerializeField] AudioClip hoverBikeClip;

    [Header("Panic Music: ")]
    [SerializeField] AudioClip panicMusicStart;
    [SerializeField] AudioClip panicMusicLoop;
    [SerializeField] AudioClip panicMusicEnd;

    [Header("Music Settings: ")]
    [SerializeField] float fadeMusicDuration = 1.5f;
    private float defaultMusicVolume;

    private AudioSource musicSource;
    private AudioSource effectSource;
    private AudioSource hoverBikeSource;

    private Coroutine fadeMusicRoutine;
    private Coroutine panicMusicRoutine;

    private void Update()
    {
        musicSource.pitch = effectSource.pitch = Time.timeScale;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this) { Destroy(gameObject); }
        }
        else { Instance = this; DontDestroyOnLoad(Instance); }

        musicSource = gameObject.GetComponent<AudioSource>();
        effectSource = gameObject.AddComponent<AudioSource>();
        hoverBikeSource = gameObject.AddComponent<AudioSource>();

        musicSource.ignoreListenerPause = true;
        musicSource.loop = true;

        hoverBikeSource.loop = true;
        hoverBikeSource.volume = 0;
        hoverBikeSource.clip = hoverBikeClip;
        hoverBikeSource.Play();

        defaultMusicVolume = musicSource.volume;

    }

    private void FadeMusic(AudioClip _newMusicClip = null)
    {
        if (fadeMusicRoutine != null) StopCoroutine(fadeMusicRoutine);
        fadeMusicRoutine = StartCoroutine(IEFadeMusic(_newMusicClip));
    }

    private IEnumerator IEFadeMusic(AudioClip _newMusicClip)
    {
        float _startVolume = musicSource.volume;
        AnimationCurve _curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        float _lerpTime = 0;
        while (_lerpTime < 1)
        {
            _lerpTime += Time.deltaTime / fadeMusicDuration;
            float _lerpKey = _curve.Evaluate(_lerpTime);

            musicSource.volume = Mathf.Lerp(_startVolume, 0, _lerpKey);
            yield return null;
        }

        musicSource.Stop();

        if (_newMusicClip != null)
        {
            musicSource.clip = _newMusicClip;
            musicSource.Play();
            _lerpTime = 0;

            while (_lerpTime < 1)
            {
                _lerpTime += Time.deltaTime / fadeMusicDuration;
                float _lerpKey = _curve.Evaluate(_lerpTime);

                musicSource.volume = Mathf.Lerp(0, defaultMusicVolume, _lerpKey);
                yield return null;
            }
        }
        else { musicSource.volume = defaultMusicVolume; }

        yield return null;
    }

    public float BehaviourOnStart()
    {
        PlayMusicClip(musicClipA);
        if (clipToPlayOnAwake != null)
        {
            UIManager.Instance.DisplaySubtitle(SubtitleManager.Instance.Subtitles[0]);
            return PlayClipAndReturnDuration(clipToPlayOnAwake, 2);
        }
        else { return 0; }
    }

    public void PlayMusicClip(AudioClip _clip)
    {
        musicSource.loop = true;

        if (musicSource.isPlaying) { FadeMusic(_clip); }
        else
        {
            musicSource.clip = _clip;
            musicSource.Play();
        }
    }

    public void PlayClip(AudioClip _clip)
    {
        effectSource.PlayOneShot(_clip);
    }

    public void PlayClip(AudioClip _clip, float _volume)
    {
        effectSource.PlayOneShot(_clip, _volume);
    }

    public float PlayClipAndReturnDuration(AudioClip _clip, float _volume = 1)
    {
        PlayClip(_clip, _volume);
        return _clip.length;
    }

    public void StartPanicMusic(float _delay)
    {
        if (panicMusicRoutine != null) StopCoroutine(panicMusicRoutine);
        panicMusicRoutine = StartCoroutine(IEPanicMusic(_delay));
    }

    public void EndPanicMusic()
    {
        if (panicMusicRoutine != null) StopCoroutine(panicMusicRoutine);
        musicSource.Stop();
        musicSource.loop = false;
        musicSource.clip = panicMusicEnd;
        musicSource.Play();
        Invoke("LowerMusicVolumeHack", 2);
    }

    private void LowerMusicVolumeHack()
    {
        musicSource.volume /= 10;
    }

    private IEnumerator IEPanicMusic(float _delay)
    {
        if(_delay != 0) { yield return new WaitForSeconds(_delay); }

        musicSource.Stop();
        musicSource.volume *= 10;
        musicSource.clip = panicMusicStart;
        musicSource.Play();

        yield return new WaitForSeconds(panicMusicStart.length);

        musicSource.clip = panicMusicLoop;
        musicSource.Play();

        yield return null;
    }

    public void SetHoverBikeSoundPitch(float _pitch, bool _engineOff = false)
    {
        hoverBikeSource.pitch = _pitch;

        if(_engineOff && hoverBikeSource.volume > 0)
        {
            hoverBikeSource.volume = Mathf.Lerp(hoverBikeSource.volume, 0, Time.deltaTime);
        }

        if(!_engineOff && hoverBikeSource.volume < 0.2f)
        {
            hoverBikeSource.volume = Mathf.Lerp(hoverBikeSource.volume, 0.2f, Time.deltaTime);
        }
    }

    public void PlayCreditsMusic()
    {
        musicSource.volume *= 5f;
        PlayMusicClip(creditsMusic);
    }
}
