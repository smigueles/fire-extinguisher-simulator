using UnityEngine;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Extinguisher Components")]
    [SerializeField] private ParticleSystem extinguisherParticles;
    [SerializeField] private Transform nozzlePoint;

    [Header("Extinguisher Settings")]
    [SerializeField] private float extinguishDistance = 10f;
    [SerializeField] private float extinguishPower = 20f;

    [Header("Audio Setup")]
    [SerializeField] private AudioSource extinguisherAudioSource;

    void OnDisable()
    {
        if (extinguisherParticles != null) extinguisherParticles.Stop();

        if (extinguisherAudioSource != null && extinguisherAudioSource.isPlaying)
        {
            extinguisherAudioSource.Stop();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (extinguisherParticles != null) extinguisherParticles.Play();

            if (extinguisherAudioSource != null && !extinguisherAudioSource.isPlaying)
            {
                extinguisherAudioSource.Play();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (extinguisherParticles != null) extinguisherParticles.Stop();

            if (extinguisherAudioSource != null && extinguisherAudioSource.isPlaying)
            {
                extinguisherAudioSource.Stop();
            }
        }

        if (Input.GetMouseButton(0))
        {
            Camera mainCam = Camera.main;

            if (mainCam != null)
            {
                Debug.DrawRay(nozzlePoint.position, mainCam.transform.forward * extinguishDistance, Color.red);

                RaycastHit hit;

                if (Physics.Raycast(nozzlePoint.position, mainCam.transform.forward, out hit, extinguishDistance))
                {
                    Debug.Log("Raycast hitting: " + hit.collider.gameObject.name);

                    FireController fire = hit.collider.GetComponent<FireController>();
                    if (fire == null)
                    {
                        fire = hit.collider.GetComponentInParent<FireController>();
                    }

                    if (fire != null)
                    {
                        fire.Extinguish(extinguishPower * Time.deltaTime);
                    }
                }
            }
        }
    }
}