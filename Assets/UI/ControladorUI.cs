using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ControladorUI : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [SerializeField] private GameObject globalVolumeBlur;
    [SerializeField] private PlayerMovement scriptPlayerMovement;
    [SerializeField] private MouseLook scriptMouseLook;
    [SerializeField] private GameObject camaraMenu;
    [SerializeField] private GameObject camaraJugador;
    [SerializeField] private GameObject Fire001;

    // Elementos de la UI
    private VisualElement fondoOscuro;
    private VisualElement p1Inicio, p2Contexto, p3Controles, p4Empezar, p5Derrota, p6Victoria;

    // Botones
    private Button btnIniciar, btnOkContexto, btnOkControles, btnEmpezar;

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

        // Asignar clics
        if (btnIniciar != null) btnIniciar.clicked += () => CambiarPantalla(p1Inicio, p2Contexto);
        if (btnOkContexto != null) btnOkContexto.clicked += () => CambiarPantalla(p2Contexto, p3Controles);
        if (btnOkControles != null) btnOkControles.clicked += () => CambiarPantalla(p3Controles, p4Empezar);
        if (btnEmpezar != null) btnEmpezar.clicked += IniciarSimulacion;

        // Al arrancar, nos aseguramos de que solo la cámara del menú esté viendo
        if (camaraJugador != null) camaraJugador.SetActive(false);
        if (camaraMenu != null) camaraMenu.SetActive(true);

        // Al arrancar el menú, pausamos al jugador
        PausarJugador(true);
    }

    private void CambiarPantalla(VisualElement actual, VisualElement siguiente)
    {
        if (actual != null) actual.style.display = DisplayStyle.None;
        if (siguiente != null) siguiente.style.display = DisplayStyle.Flex;
    }

    private void IniciarSimulacion()
    {
        if (fondoOscuro != null) fondoOscuro.style.display = DisplayStyle.None;

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

    public void MostrarPantallaDerrota()
    {
        ActivarMenuFinal(p5Derrota);
    }

    public void MostrarPantallaVictoria()
    {
        ActivarMenuFinal(p6Victoria);
    }

    private void ActivarMenuFinal(VisualElement pantallaFinal)
    {
        if (globalVolumeBlur != null) globalVolumeBlur.SetActive(true);
        if (fondoOscuro != null) fondoOscuro.style.display = DisplayStyle.Flex;

        if (p1Inicio != null) p1Inicio.style.display = DisplayStyle.None;
        if (p2Contexto != null) p2Contexto.style.display = DisplayStyle.None;
        if (p3Controles != null) p3Controles.style.display = DisplayStyle.None;
        if (p4Empezar != null) p4Empezar.style.display = DisplayStyle.None;
        if (p5Derrota != null) p5Derrota.style.display = DisplayStyle.None;
        if (p6Victoria != null) p6Victoria.style.display = DisplayStyle.None;

        if (pantallaFinal != null) pantallaFinal.style.display = DisplayStyle.Flex;
        PausarJugador(true);
    }
}