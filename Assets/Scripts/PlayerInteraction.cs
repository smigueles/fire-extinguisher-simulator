using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 7f;
    public Transform holdPoint;

    void Update()

    {
        if (Camera.main == null || !Camera.main.gameObject.activeInHierarchy)
        { return; }

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

            if (Physics.Raycast(ray, out hit, interactDistance))
            {

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Interactuaste con: " + hit.collider.name);
                    FireExtinguisher extinguisher =
                    hit.collider.GetComponent<FireExtinguisher>();
                    Debug.Log("Extintor:" + extinguisher.name);
                    if (extinguisher != null)
                    {
                        extinguisher.transform.SetParent(holdPoint);

                        extinguisher.transform.localPosition =
                            Vector3.zero;

                        extinguisher.transform.localRotation =
                            Quaternion.identity;

                        extinguisher.transform.localPosition =
                            new Vector3(0.3f, -3.05f, 0.5f);

                        extinguisher.transform.localRotation =
                            Quaternion.Euler(-90f, 90f, 180f);
                    }
                }
            }
        }
    }
}
