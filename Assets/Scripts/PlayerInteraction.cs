using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactDistance = 7f;
    public Transform holdPoint;
    public Camera playerCamera;

    void Update()
    {
        if (Camera.main == null || !Camera.main.gameObject.activeInHierarchy) return;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                FireExtinguisher extinguisher = hit.collider.GetComponent<FireExtinguisher>();
                if (extinguisher != null)
                {
                    extinguisher.transform.SetParent(holdPoint);
                    extinguisher.transform.localPosition = new Vector3(0.3f, -3.05f, 0.5f);
                    extinguisher.transform.localRotation = Quaternion.Euler(-90f, 90f, 180f);
                    extinguisher.SetHeld(true);
                }
            }
        }
    }
}