using UnityEngine;
using UnityEngine.Events;

public class FireExtinguisher : MonoBehaviour
{
    [Header("Extinguisher Components")]
    [SerializeField] private ParticleSystem extinguisherParticles;
    [SerializeField] private Transform nozzlePoint;

    [Header("Extinguisher Settings")]
    [SerializeField] private float extinguishDistance = 10f;
    [SerializeField] private float extinguishPower = 20f;

    [Header("Tank Settings")]
    [SerializeField] private float maxCapacity = 100f;
    [SerializeField] private float currentCapacity = 100f;
    [SerializeField] private float consumptionRate = 10f; // unidades por segundo de uso

    [Header("Audio Setup")]
    [SerializeField] private AudioSource extinguisherAudioSource;

    public UnityEvent OnTankEmpty;

    private Camera mainCam;
    private bool isFiring = false;
    private bool isHeld = false;
    private bool emptyEventFired = false;

    public float CurrentCapacity => currentCapacity;
    public float MaxCapacity => maxCapacity;
    public bool IsEmpty => currentCapacity <= 0f;

    void Start()
    {
        mainCam = Camera.main;
        currentCapacity = maxCapacity;
    }

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
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (mainCam == null) return;

        HandleFiringInput();

        if (isFiring)
        {
            ConsumeTank();
            PerformExtinguishRaycast();
        }
    }

    private void HandleFiringInput()
    {
        if (!isHeld) return;

        if (Input.GetMouseButtonDown(0) && !IsEmpty)
        {
            isFiring = true;
            if (extinguisherParticles != null) extinguisherParticles.Play();

            if (extinguisherAudioSource != null && !extinguisherAudioSource.isPlaying)
            {
                extinguisherAudioSource.Play();
            }
        }

        if (Input.GetMouseButtonUp(0) || (isFiring && IsEmpty))
        {
            StopFiring();
        }
    }

    private void StopFiring()
    {
        isFiring = false;

        if (extinguisherParticles != null) extinguisherParticles.Stop();

        if (extinguisherAudioSource != null && extinguisherAudioSource.isPlaying)
        {
            extinguisherAudioSource.Stop();
        }
    }

    private void ConsumeTank()
    {
        currentCapacity -= consumptionRate * Time.deltaTime;
        currentCapacity = Mathf.Clamp(currentCapacity, 0f, maxCapacity);

        if (currentCapacity <= 0f && !emptyEventFired)
        {
            emptyEventFired = true;
            OnTankEmpty?.Invoke();
        }
    }

    private void PerformExtinguishRaycast()
    {
        if (nozzlePoint == null) return;

        Vector3 origin = nozzlePoint.position;
        Vector3 direction = mainCam.transform.forward;

        Debug.DrawRay(origin, direction * extinguishDistance, Color.red);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, extinguishDistance))
        {
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

    public void Refill()
    {
        currentCapacity = maxCapacity;
        emptyEventFired = false;
    }

    public void SetHeld(bool held)
    {
        isHeld = held;

        if (!held)
        {
            StopFiring();
        }
    }
}