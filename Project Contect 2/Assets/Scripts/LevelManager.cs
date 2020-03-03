using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [SerializeField] float fadeDuration;

    private Coroutine loadSceneRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this) { Destroy(gameObject); }
        }
        else { Instance = this; DontDestroyOnLoad(Instance); }
    }

    public void LoadScene(int _index)
    {
        if (loadSceneRoutine != null) StopCoroutine(loadSceneRoutine);
        loadSceneRoutine = StartCoroutine(IELoadNewScene(_index));
    }

    private IEnumerator IELoadNewScene(int _index)
    {
        UIManager.Instance.FadeOut(fadeDuration);

        yield return new WaitForSeconds(fadeDuration);

        CinematicCameraHandler.Instance.gameObject.SetActive(false);
        SceneManager.LoadScene(_index);
        POIHandler.Instance.CurrentPOI = null;
        UIManager.Instance.FadeIn(fadeDuration);

        yield return null;
    }
}
