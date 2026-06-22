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

            if (Physics.Raycast(
                nozzlePoint.position,
                nozzlePoint.forward,
                out hit,
                extinguishDistance))
            {
                Debug.DrawRay(
                    nozzlePoint.position,
                    nozzlePoint.forward * extinguishDistance,
                    Color.green
                );

                FireController fire =
                    hit.collider.GetComponent<FireController>();

                if (fire != null)
                {
                    Debug.Log("Se activa el extintor");
                    fire.Extinguish(
                        extinguishPower * Time.deltaTime
                    );
                }
            }
        }
    }
}