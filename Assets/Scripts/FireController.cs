using UnityEngine;

public class FireController : MonoBehaviour
{
    [Header("Fire Settings")]
    public float fireHealth = 100f;

    private ParticleSystem[] fireParticleSystems;

    void Awake()
    {
        // Cache all particle systems in children components
        fireParticleSystems = GetComponentsInChildren<ParticleSystem>();

        StopFireParticles();
    }

    //void Start()
    //{
        // Ensure particles start stopped if the object is enabled by default
       // StopFireParticles();
    //}

    public void Extinguish(float amount)
    {
        // If fire is already extinguished, exit immediately
        // This prevents the health counter from dropping infinitely and saturating the UI
        if (fireHealth <= 0) return;

        // Subtracts fire health
        fireHealth -= amount;
        Debug.Log("Current Fire Health: " + fireHealth);

        // Checks if the fire was destroyed in this frame
        if (fireHealth <= 0)
        {
            fireHealth = 0; // Forces clamp to exact zero

            Debug.Log("Fire reached 0. Extinguishing particles...");
            StopFireParticles();

            // NOTIFY UI: Look for the UIController to trigger the victory sequence once
            UIController uiController = Object.FindFirstObjectByType<UIController>();
            if (uiController != null)
            {
                uiController.OnFireHealthCounterZero();
            }
            else
            {
                Debug.LogError("UIController object not found in the hierarchy. Cannot trigger victory sequence!");
            }
        }
    }

    public void ResetFire()
    {
        fireHealth = 100f; // Resets health back to initial state
        PlayFireParticles();
        Debug.Log("Fire has been reset to " + fireHealth + " for a new game session.");
    }

    private void StopFireParticles()
    {
        if (fireParticleSystems != null)
        {
            foreach (ParticleSystem ps in fireParticleSystems)
            {
                ps.Stop();
            }
        }
    }

    private void PlayFireParticles()
    {
        if (fireParticleSystems != null)
        {
            foreach (ParticleSystem ps in fireParticleSystems)
            {
                ps.Play();
            }
        }
    }
}