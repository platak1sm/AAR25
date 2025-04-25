using UnityEngine;

public class FloatingMenu : MonoBehaviour
{
    public Transform leftHand; // Assign the left hand transform
    public Vector3 menuOffset = new Vector3(0.3f, -0.1f, -0.1f); // Offset for positioning

    private void Update()
    {
        if (leftHand != null)
        {
            // Set position relative to the left hand
            transform.position = leftHand.position + leftHand.right * menuOffset.x + leftHand.up * menuOffset.y + leftHand.forward * menuOffset.z;

            // Make the menu face the user
            Vector3 lookDirection = transform.position - Camera.main.transform.position;
            lookDirection.y = 0; // Keep it upright
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}