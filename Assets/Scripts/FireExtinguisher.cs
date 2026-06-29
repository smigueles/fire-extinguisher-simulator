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

    private Camera mainCam;
    private bool isFiring = false;

    public float CurrentCapacity => currentCapacity;
    public float MaxCapacity => maxCapacity;
    public bool IsEmpty => currentCapacity <= 0f;
    public UnityEvent OnTankEmpty;

    private bool emptyEventFired = false;

    void Start()
    {
        mainCam = Camera.main;
        currentCapacity = maxCapacity;
    }

    void Update()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null) return;
        }

        HandleFiringInput();

        if (isFiring)
        {
            ConsumeTank();
            PerformExtinguishRaycast();
        }
    }

    private void HandleFiringInput()
    {
        if (Input.GetMouseButtonDown(0) && !IsEmpty)
        {
            isFiring = true;
            if (extinguisherParticles != null) extinguisherParticles.Play();
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

    public void Refill()
    {
        currentCapacity = maxCapacity;
    }
}