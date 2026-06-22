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
        fireHealth -= amount;

        Debug.Log("Vida fuego: " + fireHealth);

        if (fireHealth <= 0)
        {
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Stop();
            }
        }
    }
}