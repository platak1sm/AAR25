using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class CanvasPlacement : MonoBehaviour
{
    public GameObject canvasPrefab;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private GameObject spawnedCanvas;
    public static GameObject persistentCanvas { get; private set; }

    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (spawnedCanvas == null && persistentCanvas == null)
        {
            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var hits = new System.Collections.Generic.List<ARRaycastHit>();
            if (raycastManager.Raycast(screenCenter, hits, TrackableType.Planes))
            {
                var hit = hits[0];
                spawnedCanvas = Instantiate(canvasPrefab, hit.pose.position, hit.pose.rotation);
                persistentCanvas = spawnedCanvas;
                Vector3 directionToUser = (Camera.main.transform.position - hit.pose.position).normalized;
                spawnedCanvas.transform.rotation = Quaternion.LookRotation(-directionToUser, Vector3.up);
                spawnedCanvas.AddComponent<ARAnchor>();
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(false);
                }
            }
        }
    }

    public static GameObject GetCanvas()
    {
        return persistentCanvas;
    }
}