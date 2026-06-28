using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("Scene Setup")]
    [SerializeField] private PlayerMovement scriptPlayerMovement;
    [SerializeField] private MouseLook scriptMouseLook;
    [SerializeField] private PlayerInteraction scriptPlayerInteraction;
    [SerializeField] private GameObject menuCamera;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject fire001;
    [SerializeField] private float victoryWaitTime = 2f;

    // UI Elements
    private VisualElement uiBackground;
    private VisualElement p1Start, p2Context, p3Controllers, p4Play, p5Defeat, p6Victory;

    // Buttons
    private Button btnStart, btnContext, btnControllers, btnPlay, btnMenu, btnReplay;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Search for containers and buttons
        uiBackground = root.Q<VisualElement>("UIBackground");
        p1Start = root.Q<VisualElement>("menuScreen");
        p2Context = root.Q<VisualElement>("contextScreen");
        p3Controllers = root.Q<VisualElement>("controllersScreen");
        p4Play = root.Q<VisualElement>("startScreen");
        p5Defeat = root.Q<VisualElement>("Pantalla_5_Derrota");
        p6Victory = root.Q<VisualElement>("victoryScreen");

        btnStart = root.Q<Button>("startButton");
        btnContext = root.Q<Button>("OKBtnContextScreen");
        btnControllers = root.Q<Button>("OKBtnControllersScreen");
        btnPlay = root.Q<Button>("OKBtnStartScreen");
        btnMenu = root.Q<Button>("MenuBtnVictoryScreen");
        btnReplay = root.Q<Button>("ReplayBtnVictoryScreen");

        // Assign clicks
        if (btnStart != null) btnStart.clicked += () => SwitchScreens(p1Start, p2Context);
        if (btnContext != null) btnContext.clicked += () => SwitchScreens(p2Context, p3Controllers);
        if (btnControllers != null) btnControllers.clicked += () => SwitchScreens(p3Controllers, p4Play);

        if (btnPlay != null) btnPlay.clicked += StartSimulation;

        // Menu Button (Normal reload back to start)
        if (btnMenu != null)
        {
            btnMenu.clicked += () => {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                // Aseguramos que inicie normal en el menú
                PlayerPrefs.SetInt("SkipMenuOnReload", 0);
                PlayerPrefs.Save();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }

        // Replay Button (Smart reload bypassing menus)
        if (btnReplay != null)
        {
            btnReplay.clicked += () => {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                // Activamos el token de salteo
                PlayerPrefs.SetInt("SkipMenuOnReload", 1);
                PlayerPrefs.Save();

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }

        // CHECK RELOAD STATE: Decide whether to go to menu or straight to action
        if (PlayerPrefs.GetInt("SkipMenuOnReload", 0) == 1)
        {
            // Consume token and start instantly
            PlayerPrefs.SetInt("SkipMenuOnReload", 0);
            PlayerPrefs.Save();

            StartSimulation();
        }
        else
        {
            // Standard start on menu
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
        if (p1Start != null) p1Start.style.display = DisplayStyle.None;
        if (p2Context != null) p2Context.style.display = DisplayStyle.None;
        if (p3Controllers != null) p3Controllers.style.display = DisplayStyle.None;
        if (p4Play != null) p4Play.style.display = DisplayStyle.None;
        if (p5Defeat != null) p5Defeat.style.display = DisplayStyle.None;
        if (p6Victory != null) p6Victory.style.display = DisplayStyle.None;
    }

    private void PrepareMenuEnvironment()
    {
        if (uiBackground != null) uiBackground.style.display = DisplayStyle.Flex;

        if (playerCamera != null) playerCamera.SetActive(false);
        if (menuCamera != null) menuCamera.SetActive(true);

        PausePlayer(true);
    }

    public void ReturnToMainMenu()
    {
        PrepareMenuEnvironment();
        PrepareMenuVisuals();
        if (p1Start != null) p1Start.style.display = DisplayStyle.Flex;
    }

    private void StartSimulation()
    {
        if (uiBackground != null) uiBackground.style.display = DisplayStyle.None;
        PrepareMenuVisuals();

        if (menuCamera != null) menuCamera.SetActive(false);
        if (playerCamera != null) playerCamera.SetActive(true);

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
        Debug.Log("Simulation started / restarted successfully!");
    }

    private void PausePlayer(bool pause)
    {
        if (scriptPlayerMovement != null) scriptPlayerMovement.enabled = !pause;
        if (scriptMouseLook != null) scriptMouseLook.enabled = !pause;
        if (scriptPlayerInteraction != null) scriptPlayerInteraction.enabled = !pause;

        UnityEngine.Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
        UnityEngine.Cursor.visible = pause;
    }

    public void OnFireHealthCounterZero()
    {
        Debug.Log($"Fire extinguished! Waiting {victoryWaitTime} seconds before victory screen...");
        StartCoroutine(WaitAndShowVictory());
    }

    private IEnumerator WaitAndShowVictory()
    {
        yield return new WaitForSeconds(victoryWaitTime);
        ShowVictoryScreen();
    }

    public void ShowVictoryScreen()
    {
        ActivateFinalMenu(p6Victory);
    }

    private void ActivateFinalMenu(VisualElement finalScreen)
    {
        if (uiBackground != null) uiBackground.style.display = DisplayStyle.Flex;
        PrepareMenuVisuals();

        if (finalScreen != null) finalScreen.style.display = DisplayStyle.Flex;
        PausePlayer(true);
    }
}