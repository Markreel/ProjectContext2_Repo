using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] bool executeStartBehaviour = true;
    [SerializeField] Hovercraft_V4 player;
    [SerializeField] UnityEvent onStartEvent;

    [SerializeField] GameObject mainMenuCamera;
    [SerializeField] GameObject startCamera;
    [SerializeField] DomeBehaviour domeA;

    private bool isInMenu = true;

    private void Awake()
    {
        Instance = Instance ?? this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        startCamera.SetActive(false);
        UIManager.Instance.BehaviourOnStart();
        player.BehaviourOnStart();
        onStartEvent.Invoke();



    }

    private void StartBehaviour()
    {
        isInMenu = false;
        StartCoroutine(IEStartBehaviour());
    }

    private IEnumerator IEStartBehaviour()
    {
        UIManager.Instance.FadeOut(3);

        yield return new WaitForSeconds(3);
        UIManager.Instance.DisableMainMenu();
        mainMenuCamera.SetActive(false);
        startCamera.SetActive(true);
        yield return new WaitForSeconds(1.5f);

        domeA.ActivateAnimator();
        UIManager.Instance.FadeIn(3);
        UIManager.Instance.SetActiveLetterbox(true);

        CameraController.Instance.BehaviourOnStart(AudioManager.Instance.BehaviourOnStart());


        yield return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Time.timeScale >= 1) { Time.timeScale++; }
            else { Time.timeScale += 0.1f; }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (Time.timeScale > 1) { Time.timeScale--; }
            else { Time.timeScale -= 0.1f; }

        }
        if (Input.GetKeyDown(KeyCode.Space)) { Time.timeScale = 1; }
        if (Input.GetKeyDown(KeyCode.E) && isInMenu) { StartBehaviour(); }
    }

    public void DeactivateGameObject(GameObject _obj)
    {
        _obj.SetActive(false);
    }

    public void ActivateGameObject(GameObject _obj)
    {
        _obj.SetActive(true);
    }
}
