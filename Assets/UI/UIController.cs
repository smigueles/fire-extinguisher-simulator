using System.Collections;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [Header("Scene Setup")]
    [SerializeField] private PlayerMovement scriptPlayerMovement;
    [SerializeField] private MouseLook scriptMouseLook;
    [SerializeField] private PlayerInteraction scriptPlayerInteraction;
    [SerializeField] private CrosshairController crosshairController;
    [SerializeField] private FireExtinguisher scriptFireExtinguisher;
    [SerializeField] private GameObject menuCamera;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject fire001;
    [SerializeField] private float victoryWaitTime = 1f;

    // UI Elements
    private VisualElement uiBackground;
    private VisualElement pStart, pContext, pFireType, pControllers, pPlay, pDefeat, pVictory;

    // Buttons
    private Button btnStart, btnContext, btnFireType, btnControllers, btnPlay, btnMenu, btnReplay, btnReplayDefeat, btnInicioDefeat;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        uiBackground = root.Q<VisualElement>("UIBackground");
        pStart = root.Q<VisualElement>("menuScreen");
        pContext = root.Q<VisualElement>("contextScreen");
        pFireType = root.Q<VisualElement>("fireTypeScreen");
        pControllers = root.Q<VisualElement>("controllersScreen");
        pPlay = root.Q<VisualElement>("startScreen");
        pDefeat = root.Q<VisualElement>("Pantalla_5_Derrota");
        pVictory = root.Q<VisualElement>("victoryScreen");

        btnStart = root.Q<Button>("startButton");
        btnContext = root.Q<Button>("OKBtnContextScreen");
        btnFireType = root.Q<Button>("OKBtnTypeScreen");
        btnControllers = root.Q<Button>("OKBtnControllersScreen");
        btnPlay = root.Q<Button>("OKBtnStartScreen");
        btnMenu = root.Q<Button>("MenuBtnVictoryScreen");
        btnReplay = root.Q<Button>("ReplayBtnVictoryScreen");
        btnReplayDefeat = root.Q<Button>("Replay");
        btnInicioDefeat = root.Q<Button>("Inicio");

        if (btnStart != null) btnStart.clicked += () => SwitchScreens(pStart, pContext);
        if (btnContext != null) btnContext.clicked += () => SwitchScreens(pContext, pFireType);
        if (btnFireType != null) btnFireType.clicked += () => SwitchScreens(pFireType, pControllers);
        if (btnControllers != null) btnControllers.clicked += () => SwitchScreens(pControllers, pPlay);
        if (btnPlay != null) btnPlay.clicked += StartSimulation;

        if (btnMenu != null)
        {
            btnMenu.clicked += () =>
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                PlayerPrefs.SetInt("SkipMenuOnReload", 0);
                PlayerPrefs.Save();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }

        if (btnReplay != null)
        {
            btnReplay.clicked += () =>
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
                PlayerPrefs.SetInt("SkipMenuOnReload", 1);
                PlayerPrefs.Save();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }

        if (btnReplayDefeat != null)
        {
            btnReplayDefeat.clicked += () =>
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                PlayerPrefs.SetInt("SkipMenuOnReload", 1);
                PlayerPrefs.Save();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }

        if (btnInicioDefeat != null)
        {
            btnInicioDefeat.clicked += () =>
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                PlayerPrefs.SetInt("SkipMenuOnReload", 0);
                PlayerPrefs.Save();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }

        if (PlayerPrefs.GetInt("SkipMenuOnReload", 0) == 1)
        {
            PlayerPrefs.SetInt("SkipMenuOnReload", 0);
            PlayerPrefs.Save();
            StartSimulation();
        }
        else
        {
            PrepareMenuEnvironment();
        }
    }

    private void SwitchScreens(VisualElement current, VisualElement next)
    {
        if (current != null) current.style.display = DisplayStyle.None;
        if (next != null) next.style.display = DisplayStyle.Flex;
    }

    private void PrepareMenuVisuals()
    {
        if (pStart != null) pStart.style.display = DisplayStyle.None;
        if (pContext != null) pContext.style.display = DisplayStyle.None;
        if (pFireType != null) pFireType.style.display = DisplayStyle.None;
        if (pControllers != null) pControllers.style.display = DisplayStyle.None;
        if (pPlay != null) pPlay.style.display = DisplayStyle.None;
        if (pDefeat != null) pDefeat.style.display = DisplayStyle.None;
        if (pVictory != null) pVictory.style.display = DisplayStyle.None;
    }

    private void PrepareMenuEnvironment()
    {
        if (uiBackground != null) uiBackground.style.display = DisplayStyle.Flex;
        if (playerCamera != null) playerCamera.SetActive(false);
        if (menuCamera != null) menuCamera.SetActive(true);
        if (crosshairController != null) crosshairController.SetVisible(false);

        PausePlayer(true);
    }

    private void StartSimulation()
    {
        if (uiBackground != null) uiBackground.style.display = DisplayStyle.None;
        PrepareMenuVisuals();

        if (menuCamera != null) menuCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);

        if (crosshairController != null) crosshairController.SetVisible(true);

        if (fire001 != null)
        {
            fire001.SetActive(true);

            FireController fireController = fire001.GetComponent<FireController>();
            if (fireController == null)
            {
                fireController = fire001.GetComponentInChildren<FireController>();
            }

            if (fireController != null)
            {
                fireController.ResetFire();
            }
        }

        PausePlayer(false);
        Debug.Log("Simulation started successfully!");
    }

    private void PausePlayer(bool pause)
    {
        if (scriptPlayerMovement != null) scriptPlayerMovement.enabled = !pause;
        if (scriptMouseLook != null) scriptMouseLook.enabled = !pause;
        if (scriptPlayerInteraction != null) scriptPlayerInteraction.enabled = !pause;
        if (scriptFireExtinguisher != null) scriptFireExtinguisher.enabled = !pause;

        UnityEngine.Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        UnityEngine.Cursor.visible = pause;
    }

    public void OnFireHealthCounterZero()
    {
        StartCoroutine(WaitAndShowVictory());
    }

    private IEnumerator WaitAndShowVictory()
    {
        yield return new WaitForSeconds(victoryWaitTime);
        ShowVictoryScreen();
    }

    public void ShowVictoryScreen()
    {
        if (uiBackground != null) uiBackground.style.display = DisplayStyle.Flex;
        PrepareMenuVisuals();
        if (pVictory != null) pVictory.style.display = DisplayStyle.Flex;
        if (crosshairController != null) crosshairController.SetVisible(false);
        PausePlayer(true);
    }

    public void OnExtinguisherEmpty()
    {
        if (fire001 == null) return;

        FireController fireController = fire001.GetComponent<FireController>();
        if (fireController == null)
        {
            fireController = fire001.GetComponentInChildren<FireController>();
        }

        bool fireStillActive = fireController != null && fireController.fireHealth > 0f;

        if (fireStillActive)
        {
            ShowDefeatScreen();
        }
    }

    private void ShowDefeatScreen()
    {
        if (uiBackground != null) uiBackground.style.display = DisplayStyle.Flex;
        PrepareMenuVisuals();

        if (pDefeat != null) pDefeat.style.display = DisplayStyle.Flex;

        if (playerCamera != null) playerCamera.SetActive(false);
        if (menuCamera != null) menuCamera.SetActive(true);

        PausePlayer(true);
        if (crosshairController != null) crosshairController.SetVisible(false);
    }


}