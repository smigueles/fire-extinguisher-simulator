using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Extinguisher Components")]
    [SerializeField] private ParticleSystem extinguisherParticles;
    [SerializeField] private Transform nozzlePoint;

    [Header("Extinguisher Settings")]
    [SerializeField] private float extinguishDistance = 10f;
    [SerializeField] private float extinguishPower = 20f;

    void Update()
    {
        // Handle particle system feedback based on input
        if (Input.GetMouseButtonDown(0))
        {
            if (extinguisherParticles != null) extinguisherParticles.Play();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (extinguisherParticles != null) extinguisherParticles.Stop();
        }

        // Handle physical raycast logic while holding down the button
        if (Input.GetMouseButton(0))
        {
            Camera mainCam = Camera.main;

            if (mainCam != null)
            {
                // DIBUJA LA LÍNEA: Nace en la manguera, pero viaja hacia donde mira la cámara
                Debug.DrawRay(nozzlePoint.position, mainCam.transform.forward * extinguishDistance, Color.red);

                RaycastHit hit;

                // CORRECCIÓN DEFINITIVA: El rayo usa la posición del nozzlePoint pero la dirección de la cámara
                if (Physics.Raycast(nozzlePoint.position, mainCam.transform.forward, out hit, extinguishDistance))
                {
                    Debug.Log("Raycast hitting: " + hit.collider.gameObject.name);

                    // Look for the FireController component in the object or its parents
                    FireController fire = hit.collider.GetComponent<FireController>();
                    if (fire == null)
                    {
                        fire = hit.collider.GetComponentInParent<FireController>();
                    }

                    if (fire != null)
                    {
                        // Apply extinguishing damage per second over time
                        fire.Extinguish(extinguishPower * Time.deltaTime);
                    }
                }
            }
        }
    }
}