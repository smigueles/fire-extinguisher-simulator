using UnityEngine;

public class FireController : MonoBehaviour
{
    [Header("Fire Settings")]
    public float fireHealth = 100f;

    [Header("Audio Setup")]
    [SerializeField] private AudioSource fireAudioSource;

    private ParticleSystem[] fireParticleSystems;

    void Awake()
    {
        fireParticleSystems = GetComponentsInChildren<ParticleSystem>();

        if (fireAudioSource == null)
        {
            fireAudioSource = GetComponent<AudioSource>();
        }

        StopFireParticles();
        StopFireAudio();
    }

    public void Extinguish(float amount)
    {
        if (fireHealth <= 0) return;

        fireHealth -= amount;
        Debug.Log($"Current Fire Health: {fireHealth}");

        if (fireHealth <= 0)
        {
            fireHealth = 0;

            Debug.Log("Fire reached 0. Extinguishing particles and audio...");
            StopFireParticles();
            StopFireAudio();

            // Notifies UI for end of simulation
            UIController uiController = Object.FindFirstObjectByType<UIController>();
            if (uiController != null)
            {
                uiController.OnFireHealthCounterZero();
            }
            else
            {
                Debug.LogError("UIController not found in the hierarchy!");
            }
        }
    }

    public void ResetFire()
    {
        fireHealth = 100f;
        PlayFireParticles();
        PlayFireAudio();
        Debug.Log($"Fire has been reset to {fireHealth} for a new game session.");
    }

    private void StopFireParticles()
    {
        if (fireParticleSystems == null) return;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Stop();
        }
    }

    private void PlayFireParticles()
    {
        if (fireParticleSystems == null) return;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Play();
        }
    }

    private void PlayFireAudio()
    {
        if (fireAudioSource != null && !fireAudioSource.isPlaying)
        {
            fireAudioSource.Play();
        }
    }

    private void StopFireAudio()
    {
        if (fireAudioSource != null && fireAudioSource.isPlaying)
        {
            fireAudioSource.Stop();
        }
    }
}