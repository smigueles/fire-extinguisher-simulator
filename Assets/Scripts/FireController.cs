using System.Collections;
using UnityEngine;

public class FireController : MonoBehaviour
{
    public float fireHealth = 100f;
    public float startDelay = 3f;

    private ParticleSystem[] particleSystems;

    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }

        StartCoroutine(StartFireAfterDelay());
    }

    IEnumerator StartFireAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Play();
        }
    }

    public void Extinguish(float amount)
    {
        // 1. EL CANDADO: Si el fuego ya se apagó, salimos inmediatamente.
        // Esto evita que el contador siga bajando infinitamente y sature la UI.
        if (fireHealth <= 0) return;

        // Restamos la vida del fuego
        fireHealth -= amount;
        Debug.Log("Vida del fuego actual: " + fireHealth);

        // 2. EL QUIEBRE: Evaluamos si murió en este frame
        if (fireHealth <= 0)
        {
            fireHealth = 0; // Forzamos a que quede clavado en 0 justo

            Debug.Log("¡El fuego llegó a 0! Apagando partículas...");

            // Apagamos los sistemas de partículas del fuego para que visualmente desaparezca
            if (particleSystems != null)
            {
                foreach (ParticleSystem ps in particleSystems)
                {
                    ps.Stop();
                }
            }

            // 3. EL LLAMADO A TU INTERFAZ: Le avisa a tu ControladorUI una ÚNICA VEZ
            ControladorUI ui = Object.FindFirstObjectByType<ControladorUI>();
            if (ui != null)
            {
                ui.OnContadorLlamasCero();
            }
            else
            {
                Debug.LogError("No se encontró el objeto ControladorUI en la escena. ¡Asegurate de que esté en la jerarquía!");
            }
        }
    }

        public void ResetearFuego()
        {
            fireHealth = 100f; // O la vida inicial que le hayan puesto (ej. 100)

            // Volvemos a encender visualmente las partículas del fuego
            if (particleSystems != null)
            {
                foreach (ParticleSystem ps in particleSystems)
            {
                ps.Play();
            }
            }
                Debug.Log("¡El fuego ha sido reestablecido a " + fireHealth + " para una nueva partida!");
        }
}