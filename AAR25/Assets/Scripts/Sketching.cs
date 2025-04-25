using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.Input;

//Code for sketching on the whiteboard with index finger --doesn't work but can you see why?

// public class Sketching : MonoBehaviour
// {
//     public OVRHand rightHand;
//     public GameObject linePrefab;
//     public AudioSource audioSource; 
//     public AudioClip drawStartSound; 

//     private LineRenderer lineRenderer;
//     private GameObject currentLine;
//     private bool isDrawing = false;

//     private void Update()
//     {
//         if (rightHand == null) 
//         {
//             Debug.LogError("[ERROR] RightHand (OVRHand) is NULL! Assign it in the Inspector.");
//             return;
//         }

//         Vector3 fingerTipPos = rightHand.PointerPose.position;
//         Debug.Log($"[DEBUG] Synthetic Index Finger Position: {fingerTipPos}");

//         if (isDrawing)
//         {
//             AddPoint(fingerTipPos);
//         }
//     }

//     private void StartLine(Vector3 startPos)
//     {
//         currentLine = Instantiate(linePrefab, transform);
//         lineRenderer = currentLine.GetComponent<LineRenderer>();

//         if (lineRenderer == null)
//         {
//             Debug.LogError("[ERROR] LineRenderer not found on linePrefab!");
//             return;
//         }

//         lineRenderer.positionCount = 1;
//         lineRenderer.SetPosition(0, startPos);

//         AnimationCurve widthCurve = new AnimationCurve();
//         widthCurve.AddKey(0f, 0.1f);
//         widthCurve.AddKey(1f, 0.1f);
//         lineRenderer.widthCurve = widthCurve;

//         Debug.Log("[DEBUG] New Line Started at Position: " + startPos);
//     }

//     private void AddPoint(Vector3 position)
//     {
//         if (lineRenderer == null) return;

//         lineRenderer.positionCount++;
//         lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);

//         Debug.Log("[DEBUG] Point Added: " + position);
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Whiteboard"))
//         {
//             if (!isDrawing) 
//             {
//                 if (audioSource != null && drawStartSound != null)
//                 {
//                     audioSource.PlayOneShot(drawStartSound);
//                     Debug.Log("[DEBUG] Draw Start Sound Played!");
//                 }
//                 else
//                 {
//                     Debug.LogError("[ERROR] AudioSource or DrawStartSound is not assigned!");
//                 }
//             }

//             isDrawing = true;
//             Vector3 startPos = rightHand.PointerPose.position;
//             StartLine(startPos);
//         }
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Whiteboard"))
//         {
//             isDrawing = false;
//             Debug.Log("[DEBUG] Stopped Drawing.");
//         }
//     }
// }

public class Sketching : MonoBehaviour
{
    
    public Transform headRay; 
    public OVRHand rightHand; 
    public GameObject linePrefab; 
    public AudioSource audioSource; 
    public AudioClip drawStartSound; 

    private LineRenderer lineRenderer;
    private GameObject currentLine;
    private bool isDrawing = false;
    private bool wasPinching = false; 

    private void Update()
    {
        if (headRay == null || rightHand == null) return;


        Ray ray = new Ray(headRay.position, headRay.forward);
        RaycastHit hit;
        bool hitWhiteboard = Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Whiteboard");


        bool isPinching = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

        if (hitWhiteboard && isPinching && !wasPinching)
        {
            StartLine(hit.point);
            if (audioSource != null && drawStartSound != null)
            {
                audioSource.PlayOneShot(drawStartSound);
            }
        }

        if (isDrawing && isPinching)
        {
            AddPoint(hit.point);
        }

        if (!isPinching && isDrawing)
        {
            EraseDrawing();
        }

        wasPinching = isPinching; 
        Debug.Log($"[DEBUG] Ray Hit: {hitWhiteboard}, Pinching: {isPinching}, Drawing: {isDrawing}");
    }

    private void StartLine(Vector3 startPos)
    {
        currentLine = Instantiate(linePrefab, transform);
        lineRenderer = currentLine.GetComponent<LineRenderer>();

        if (lineRenderer == null)
        {
            Debug.LogError("[ERROR] LineRenderer not found on linePrefab!");
            return;
        }

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, startPos);
        isDrawing = true;

        AnimationCurve widthCurve = new AnimationCurve();
        widthCurve.AddKey(0f, 0.005f); 
        widthCurve.AddKey(1f, 0.005f);
        lineRenderer.widthCurve = widthCurve;
    }

    private void AddPoint(Vector3 position)
    {
        if (lineRenderer == null) return;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
    }

    private void EraseDrawing()
    {
        if (currentLine != null)
        {
            Destroy(currentLine);
            currentLine = null;
            lineRenderer = null;
        }
        isDrawing = false;
    }
}