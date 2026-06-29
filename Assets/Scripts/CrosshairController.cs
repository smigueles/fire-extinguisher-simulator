using UnityEngine;
using UnityEngine.UIElements;

public class CrosshairController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private float detectDistance = 15f;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.8f);
    [SerializeField] private Color fireDetectedColor = new Color(1f, 0.3f, 0f, 0.9f);

    private VisualElement crosshair;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;

        if (uiDocument != null)
        {
            crosshair = uiDocument.rootVisualElement.Q<VisualElement>("crosshair");
        }
    }

    void Update()
    {
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (crosshair == null || mainCam == null) return;



        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        bool lookingAtFire = Physics.Raycast(ray, out RaycastHit hit, detectDistance)
            && (hit.collider.GetComponent<FireController>() != null
                || hit.collider.GetComponentInParent<FireController>() != null);


        crosshair.style.backgroundColor = lookingAtFire ? fireDetectedColor : normalColor;
    }

    public void SetVisible(bool visible)
    {
        Debug.Log("Start corsshair");
        if (crosshair == null && uiDocument != null)
        {
            crosshair = uiDocument.rootVisualElement.Q<VisualElement>("crosshair");
        }

        if (crosshair != null)
            crosshair.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
}