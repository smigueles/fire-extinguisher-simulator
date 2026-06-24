using UnityEngine;
using UnityEngine.UIElements;
using System.Collections; // <-- Necesario para usar Corrutinas (IEnumerator)

public class ControladorUI : MonoBehaviour
{
    [Header("Configuración de Escena")]
    //[SerializeField] private GameObject globalVolumeBlur;
    [SerializeField] private PlayerMovement scriptPlayerMovement;
    [SerializeField] private MouseLook scriptMouseLook;
    [SerializeField] private GameObject camaraMenu;
    [SerializeField] private GameObject camaraJugador;
    [SerializeField] private GameObject Fire001;
    [SerializeField] private float tiempoEsperaVictoria = 3.0f; // Segundos de espera al ganar

    // Elementos de la UI
    private VisualElement fondoOscuro;
    private VisualElement p1Inicio, p2Contexto, p3Controles, p4Empezar, p5Derrota, p6Victoria;

    // Botones
    private Button btnIniciar, btnOkContexto, btnOkControles, btnEmpezar, btnMenu, btnReplay;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Buscar contenedores (del UI Builder)
        fondoOscuro = root.Q<VisualElement>("FondoOscuro");
        p1Inicio = root.Q<VisualElement>("Pantalla_1_Menu_Principal");
        p2Contexto = root.Q<VisualElement>("Pantalla_2_Contexto");
        p3Controles = root.Q<VisualElement>("Pantalla_3_Controles");
        p4Empezar = root.Q<VisualElement>("Pantalla_4_Empezar");
        p5Derrota = root.Q<VisualElement>("Pantalla_5_Derrota");
        p6Victoria = root.Q<VisualElement>("Pantalla_6_Victoria");

        // Buscar botones (del UI Builder)
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

        // Al volver al menú, reseteamos el estado visual general antes de mostrar P1
        if (btnMenu != null) btnMenu.clicked += VolverAlMenuPrincipal;

        // Replay inicia directo la simulación salteándose la intro
        if (btnReplay != null) btnReplay.clicked += IniciarSimulacion;

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
        // Limpiamos pantallas para que no se superpongan
        if (p1Inicio != null) p1Inicio.style.display = DisplayStyle.None;
        if (p2Contexto != null) p2Contexto.style.display = DisplayStyle.None;
        if (p3Controles != null) p3Controles.style.display = DisplayStyle.None;
        if (p4Empezar != null) p4Empezar.style.display = DisplayStyle.None;
        if (p5Derrota != null) p5Derrota.style.display = DisplayStyle.None;
        if (p6Victoria != null) p6Victoria.style.display = DisplayStyle.None;
    }

    private void PrepararEntornoMenu()
    {
        //if (globalVolumeBlur != null) globalVolumeBlur.SetActive(false);
        if (fondoOscuro != null) fondoOscuro.style.display = DisplayStyle.Flex;

        if (camaraJugador != null) camaraJugador.SetActive(false);
        if (camaraMenu != null) camaraMenu.SetActive(true);

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
        // Apagamos desenfoques y fondos de interfaz
        //if (globalVolumeBlur != null) globalVolumeBlur.SetActive(false);
        if (fondoOscuro != null) fondoOscuro.style.display = DisplayStyle.None;
        PreparaVisualesMenus();

        // Hacemos el cambiazo de cámaras manual y obligado
        if (camaraMenu != null) camaraMenu.SetActive(false);
        if (camaraJugador != null) camaraJugador.SetActive(true);

        // Encendemos el fuego del monitor
        if (Fire001 != null) Fire001.SetActive(true);

        PausarJugador(false);
        Debug.Log("¡A apagar el incendio!");
    }

    private void PausarJugador(bool pausar)
    {
        if (scriptPlayerMovement != null)
            scriptPlayerMovement.enabled = !pausar;

        if (scriptMouseLook != null)
            scriptMouseLook.enabled = !pausar;

        UnityEngine.Cursor.lockState = pausar ? CursorLockMode.None : CursorLockMode.Locked;
        UnityEngine.Cursor.visible = pausar;
    }

    // --- MÉTODOS DE FIN DE PARTIDA ---

    // Comentado temporalmente por falta de dinámica de derrota
    public void MostrarPantallaDerrota()
    {
        // ActivarMenuFinal(p5Derrota);
    }

    // Este es el método que llamarán tus compañeras cuando el contador llegue a 0
    public void OnContadorLlamasCero()
    {
        Debug.Log($"¡Fuego extinguido! Esperando {tiempoEsperaVictoria} segundos antes de la victoria...");
        StartCoroutine(EsperarYMostrarVictoria());
    }

    private IEnumerator EsperarYMostrarVictoria()
    {
        // Espera el tiempo configurado en los campos de Unity
        yield return new WaitForSeconds(tiempoEsperaVictoria);
        MostrarPantallaVictoria();
    }

    public void MostrarPantallaVictoria()
    {
        ActivarMenuFinal(p6Victoria);
    }

    private void ActivarMenuFinal(VisualElement pantallaFinal)
    {
        //if (globalVolumeBlur != null) globalVolumeBlur.SetActive(true);
        if (fondoOscuro != null) fondoOscuro.style.display = DisplayStyle.Flex;

        PreparaVisualesMenus();

        if (pantallaFinal != null) pantallaFinal.style.display = DisplayStyle.Flex;
        PausarJugador(true);
    }
}