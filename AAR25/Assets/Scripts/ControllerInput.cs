// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.XR.Interaction.Toolkit;

// public class ControllerInput : MonoBehaviour
// {
//     public CanvasDrawing canvas;
//     public GameObject leftController;
//     private XRController rightController;
//     private bool isErasing;
//     private Color[] colors = { Color.red, Color.blue, Color.black };
//     private string[] colorNames = { "Red", "Blue", "Black" };
//     private int currentColorIndex = 0;
//     public TMPro.TextMeshProUGUI colorText;

//     void Start()
//     {
//         rightController = GetComponent<XRController>();
//         UpdateColorUI();
//     }

//     void Update()
//     {
//         if (canvas == null)
//         {
//             var canvasObj = CanvasPlacement.GetCanvas();
//             if (canvasObj != null) canvas = canvasObj.GetComponent<CanvasDrawing>();
//             return;
//         }

//         if (rightController.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed) && triggerPressed && !isErasing)
//         {
//             RaycastHit hit;
//             if (Physics.Raycast(rightController.transform.position, rightController.transform.forward, out hit))
//             {
//                 if (hit.collider.gameObject == canvas.gameObject)
//                 {
//                     if (canvas.currentLine == null) canvas.StartDrawing(hit.point);
//                     else canvas.AddPoint(hit.point);
//                 }
//             }
//         }
//         else
//         {
//             canvas.StopDrawing();
//         }

//         var leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
//         if (leftDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
//         {
//             currentColorIndex = 0;
//             UpdateColor();
//         }
//         if (leftDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed) && bPressed)
//         {
//             currentColorIndex = 1;
//             UpdateColor();
//         }
//         if (leftDevice.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool xPressed) && xPressed)
//         {
//             currentColorIndex = 2;
//             UpdateColor();
//         }

//         if (rightController.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed) && gripPressed)
//         {
//             isErasing = true;
//             // TODO: Erase logic
//         }
//         else
//         {
//             isErasing = false;
//         }
//     }

//     void UpdateColor()
//     {
//         canvas.SetColor(colors[currentColorIndex]);
//         UpdateColorUI();
//     }

//     void UpdateColorUI()
//     {
//         colorText.text = $"Current Color: {colorNames[currentColorIndex]}\nA: Red, B: Blue, X: Black";
//     }
// }
