// using UnityEngine;
// using UnityEngine.XR;
// using UnityEngine.XR.Management;
// using System.Collections;

// public class UserPresenceHandler : MonoBehaviour
// {
//     public bool isUserPresent { get; private set; } = true;
//     public bool waitingForNewUser { get; private set; } = false;
//     private Vector3 lastCameraPosition;
//     private Quaternion lastCameraRotation;
//     private Coroutine sessionCheckCoroutine;
//     private bool isXrInitialized = false;
//     private Camera mainCamera;

//     void Awake()
//     {
//         // Ensure this persists across scenes
//         DontDestroyOnLoad(gameObject);
//     }

//     void Start()
//     {
//         // Cache the main camera
//         mainCamera = Camera.main;

//         // Wait for XR to initialize before starting user presence monitoring
//         StartCoroutine(WaitForXrInitialization());
//     }

//     IEnumerator WaitForXrInitialization()
//     {
//         // Wait until XR session is active
//         while (XRGeneralSettings.Instance == null || XRGeneralSettings.Instance.Manager == null ||
//                XRGeneralSettings.Instance.Manager.activeLoader == null ||
//                XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>() == null ||
//                XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>().running == false)
//         {
//             yield return null;
//         }

//         isXrInitialized = true;
//         Debug.Log("UserPresenceHandler: XR session initialized, starting user presence monitoring");

//         // Store initial camera position and rotation
//         StoreCameraTransform();

//         // Start monitoring user presence
//         sessionCheckCoroutine = StartCoroutine(MonitorUserPresence());
//     }

//     void OnDestroy()
//     {
//         if (sessionCheckCoroutine != null)
//         {
//             StopCoroutine(sessionCheckCoroutine);
//         }
//     }

//     IEnumerator MonitorUserPresence()
//     {
//         while (true)
//         {
//             if (!isXrInitialized)
//             {
//                 yield return null;
//                 continue;
//             }

//             // Check user presence using OVRPlugin
//             bool currentUserPresent = OVRPlugin.userPresent;

//             if (isUserPresent && !currentUserPresent)
//             {
//                 // Headset removed
//                 Debug.Log("UserPresenceHandler: Headset removed, waiting for new user...");
//                 isUserPresent = false;
//                 waitingForNewUser = true;

//                 // Prevent session from stopping
//                 StartCoroutine(MaintainSession());
//             }
//             else if (!isUserPresent && currentUserPresent && waitingForNewUser)
//             {
//                 // Headset worn by new user
//                 Debug.Log("UserPresenceHandler: Headset worn by new user, resuming...");
//                 isUserPresent = true;
//                 waitingForNewUser = false;

//                 // Restore camera transform and ensure rendering resumes
//                 RestoreCameraTransform();
//                 ResumeRendering();

//                 // Resume the game flow
//                 ResumeGame();
//             }

//             yield return new WaitForSeconds(0.1f);
//         }
//     }

//     IEnumerator MaintainSession()
//     {
//         // Prevent XR session from stopping
//         if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
//         {
//             XRGeneralSettings.Instance.Manager.DeinitializeLoader();
//             yield return null; // Wait for deinitialization
//         }

//         // Keep the app running without fully stopping the XR session
//         while (waitingForNewUser)
//         {
//             yield return new WaitForSeconds(0.1f);
//         }

//         // Reinitialize the XR session when the new user is detected
//         if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
//         {
//             yield return StartCoroutine(XRGeneralSettings.Instance.Manager.InitializeLoader());
//             if (XRGeneralSettings.Instance.Manager.activeLoader != null)
//             {
//                 XRGeneralSettings.Instance.Manager.StartSubsystems();

//                 // Wait for the session to be fully active
//                 yield return StartCoroutine(WaitForSessionActive());

//                 isXrInitialized = true;
//                 Debug.Log("UserPresenceHandler: XR session resumed.");
//             }
//             else
//             {
//                 Debug.LogError("UserPresenceHandler: Failed to reinitialize XR session.");
//             }
//         }
//     }

//     IEnumerator WaitForSessionActive()
//     {
//         // Wait until the XR session is fully active
//         while (XRGeneralSettings.Instance.Manager.activeLoader.GetLoadedSubsystem<XRInputSubsystem>()?.running == false)
//         {
//             yield return null;
//         }

//         // Additional delay to ensure rendering stabilizes
//         yield return new WaitForSeconds(0.5f);
//         Debug.Log("UserPresenceHandler: XR session fully active, rendering should resume.");
//     }

//     void StoreCameraTransform()
//     {
//         if (mainCamera != null)
//         {
//             lastCameraPosition = mainCamera.transform.position;
//             lastCameraRotation = mainCamera.transform.rotation;
//             Debug.Log($"UserPresenceHandler: Stored camera transform - Position: {lastCameraPosition}, Rotation: {lastCameraRotation}");
//         }
//         else
//         {
//             Debug.LogWarning("UserPresenceHandler: Main camera not found, cannot store transform.");
//         }
//     }

//     void RestoreCameraTransform()
//     {
//         if (mainCamera != null)
//         {
//             mainCamera.transform.position = lastCameraPosition;
//             mainCamera.transform.rotation = lastCameraRotation;
//             Debug.Log($"UserPresenceHandler: Restored camera transform - Position: {lastCameraPosition}, Rotation: {lastCameraRotation}");

//             // Ensure tracking origin is not reset
//             XRInputSubsystem inputSubsystem = XRGeneralSettings.Instance?.Manager.activeLoader?.GetLoadedSubsystem<XRInputSubsystem>();
//             if (inputSubsystem != null)
//             {
//                 inputSubsystem.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
//                 Debug.Log("UserPresenceHandler: Tracking origin set to Device mode to prevent recentering.");
//             }
//         }
//         else
//         {
//             Debug.LogWarning("UserPresenceHandler: Main camera not found, cannot restore transform.");
//         }
//     }

//     void ResumeRendering()
//     {
//         if (mainCamera != null)
//         {
//             // Ensure the camera is enabled
//             mainCamera.enabled = false;
//             mainCamera.enabled = true;
//             Debug.Log("UserPresenceHandler: Re-enabled main camera to resume rendering.");

//             // Force a refresh of the XR display subsystem
//             XRGeneralSettings.Instance?.Manager.activeLoader?.GetLoadedSubsystem<XRDisplaySubsystem>()?.Start();
//         }
//         else
//         {
//             Debug.LogWarning("UserPresenceHandler: Main camera not found, cannot resume rendering.");
//         }
//     }

//     void ResumeGame()
//     {
//         if (TimeManager.Instance != null && TimeManager.drawingCompleted)
//         {
//             GameObject promptUI = TimeManager.Instance.promptUI;
//             if (promptUI != null)
//             {
//                 promptUI.SetActive(true);
//                 Debug.Log("UserPresenceHandler: Resumed game, ensured promptUI is active.");
//             }
//         }
//     }

//     void OnApplicationPause(bool pauseStatus)
//     {
//         if (pauseStatus && waitingForNewUser)
//         {
//             Debug.Log("UserPresenceHandler: Application paused, but keeping session alive for new user.");
//         }
//     }
// }