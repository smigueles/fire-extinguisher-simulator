using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    public ParticleSystem extinguisherParticles;
    public Transform nozzlePoint;

    public float extinguishDistance = 10f;
    public float extinguishPower = 20f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            extinguisherParticles.Play();
        }

        if (Input.GetMouseButtonUp(0))
        {
            extinguisherParticles.Stop();
        }

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Camera mainCam = Camera.main;

            if (mainCam != null)
            {
                // El rayo nace 1.2 metros adelante de la cámara para no chocarse con el Player
                Vector3 origenRayo = mainCam.transform.position + mainCam.transform.forward * 1.2f;

                // Dispara hacia adelante, directo a donde apunta el centro de tu pantalla
                if (Physics.Raycast(origenRayo, mainCam.transform.forward, out hit, extinguishDistance))
                {
                    Debug.Log("¡RAYO SINCRONIZADO! Chocando con: " + hit.collider.gameObject.name);

                    // SOLUCIÓN AL CONTADOR: Busca el script en el objeto O en sus padres (por si le pega a un hijo de Fire001)
                    FireController fire = hit.collider.GetComponent<FireController>();
                    if (fire == null)
                    {
                        fire = hit.collider.GetComponentInParent<FireController>();
                    }

                    if (fire != null)
                    {
                        // Esto va a hacer que por fin aparezca el contador en la consola
                        fire.Extinguish(extinguishPower * Time.deltaTime);
                    }
                }
            }
        }
    }
}