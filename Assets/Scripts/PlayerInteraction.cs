using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 5f;

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0)
        );
        Debug.DrawRay(
          ray.origin,
          ray.direction * interactDistance,
          Color.red
        );

        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E presionada");
        }

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            Debug.Log("Golpeando: " + hit.collider.name);

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Interactuaste con: " + hit.collider.name);
            }
        }
    }
}