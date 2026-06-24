using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.SceneManagement;

public class ControladorUI : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [SerializeField] private PlayerMovement scriptPlayerMovement;
    [SerializeField] private MouseLook scriptMouseLook;
    [SerializeField] private GameObject camaraMenu;
    [SerializeField] private GameObject camaraJugador;
    [SerializeField] private GameObject Fire001;
    [SerializeField] private float tiempoEsperaVictoria = 1.0f;

    // Elementos de la UI
    private VisualElement fondoOscuro;
    private VisualElement p1Inicio, p2Contexto, p3Controles, p4Empezar, p5Derrota, p6Victoria;

    // Botones
    private Button btnIniciar, btnOkContexto, btnOkControles, btnEmpezar, btnMenu, btnReplay;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Buscar contenedores
        fondoOscuro = root.Q<VisualElement>("FondoOscuro");
        p1Inicio = root.Q<VisualElement>("Pantalla_1_Menu_Principal");
        p2Contexto = root.Q<VisualElement>("Pantalla_2_Contexto");
        p3Controles = root.Q<VisualElement>("Pantalla_3_Controles");
        p4Empezar = root.Q<VisualElement>("Pantalla_4_Empezar");
        p5Derrota = root.Q<VisualElement>("Pantalla_5_Derrota");
        p6Victoria = root.Q<VisualElement>("Pantalla_6_Victoria");

        // Buscar botones
        btnIniciar = root.Q<Button>("Boton_Inicio");
        btnOkContexto = root.Q<Button>("Boton_OK_p2");
        btnOkControles = root.Q<Button>("Boton_OK_p3");
        btnEmpezar = root.Q<Button>("Boton_Empezar_p4");
        btnMenu = root.Q<Button>("Boton_Menu_p6");
        btnReplay = root.Q<Button>("Boton_Replay_p6");

        // Asignar clics
        if (btnIniciar != null) btnIniciar.clicked += () => CambiarPantalla(p1Inicio, p2Contexto);
        if (btnOkContexto != null) btnOkContexto.clicked += () => CambiarPantalla(p2Contexto, p3Controles);
        if (btnOkControles != null) btnOkControles.clicked += () => CambiarPantalla(p3Controles, p4Empezar);

        if (btnEmpezar != null) btnEmpezar.clicked += IniciarSimulacion;

        // Al volver al menú, resetea todo el entorno visual antes de mostrar P1
        if (btnMenu != null)
        {
            btnMenu.clicked += () => {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                // Recarga la escena para que empiece en el menú desde cero
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }

        if (btnReplay != null)
        {
            btnReplay.clicked += () => {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                // Carga de nuevo la escena que está abierta en este momento
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }

        // Configuración inicial del arranque
        PrepararEntornoMenu();
    }

    private void CambiarPantalla(VisualElement actual, VisualElement siguiente)
    {
        if (actual != null) actual.style.display = DisplayStyle.None;
        if (siguiente != null) siguiente.style.display = DisplayStyle.Flex;
    }

    private void PreparaVisualesMenus()
    {
        if (p1Inicio != null) p1Inicio.style.display = DisplayStyle.None;
        if (p2Contexto != null) p2Contexto.style.display = DisplayStyle.None;
        if (p3Controles != null) p3Controles.style.display = DisplayStyle.None;
        if (p4Empezar != null) p4Empezar.style.display = DisplayStyle.None;
        if (p5Derrota != null) p5Derrota.style.display = DisplayStyle.None;
        if (p6Victoria != null) p6Victoria.style.display = DisplayStyle.None;
    }

    private void PrepararEntornoMenu()
    {
        if (fondoOscuro != null) fondoOscuro.style.display = DisplayStyle.Flex;

        if (camaraJugador != null) camaraJugador.SetActive(false);
        if (camaraMenu != null) camaraMenu.SetActive(true);

        // FUEGO APAGADO EN EL MENU
        if (Fire001 != null)
        {
            Fire001.SetActive(false);
        }

        PausarJugador(true);
    }

    private void VolverAlMenuPrincipal()
    {
        PrepararEntornoMenu();
        PreparaVisualesMenus();
        if (p1Inicio != null) p1Inicio.style.display = DisplayStyle.Flex;
    }

    private void IniciarSimulacion()
    {
        // Limpieza de interfaces
        if (fondoOscuro != null) fondoOscuro.style.display = DisplayStyle.None;
        PreparaVisualesMenus();

        // Intercambio de cámaras
        if (camaraMenu != null) camaraMenu.SetActive(false);
        if (camaraJugador != null) camaraJugador.SetActive(true);

        // Forzar el encendido y reseteo del fuego
        if (Fire001 != null)
        {
            Fire001.SetActive(true);

            // Buscamos el script en el objeto para resetear su vida y partículas
            FireController controllerFuego = Fire001.GetComponent<FireController>();
            if (controllerFuego == null)
            {
                controllerFuego = Fire001.GetComponentInChildren<FireController>();
            }

            if (controllerFuego != null)
            {
                controllerFuego.ResetearFuego();
            }
        }

        // Devuelve el control de movimiento y mouse al jugador
        PausarJugador(false);
        Debug.Log("¡Simulación Iniciada / Reiniciada con éxito!");
    }

    private void PausarJugador(bool pausar)
    {
        if (scriptPlayerMovement != null) scriptPlayerMovement.enabled = !pausar;
        if (scriptMouseLook != null) scriptMouseLook.enabled = !pausar;

        UnityEngine.Cursor.lockState = pausar ? CursorLockMode.None : CursorLockMode.Locked;
        UnityEngine.Cursor.visible = pausar;
    }

    public void OnContadorLlamasCero()
    {
        Debug.Log($"¡Fuego extinguido! Esperando {tiempoEsperaVictoria} segundos antes de la victoria...");
        StartCoroutine(EsperarYMostrarVictoria());
    }

    private IEnumerator EsperarYMostrarVictoria()
    {
        yield return new WaitForSeconds(tiempoEsperaVictoria);
        MostrarPantallaVictoria();
    }

    public void MostrarPantallaVictoria()
    {
        ActivarMenuFinal(p6Victoria);
    }

    private void ActivarMenuFinal(VisualElement pantallaFinal)
    {
        if (fondoOscuro != null) fondoOscuro.style.display = DisplayStyle.Flex;
        PreparaVisualesMenus();

        if (pantallaFinal != null) pantallaFinal.style.display = DisplayStyle.Flex;
        PausarJugador(true);
    }
}