using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform playerTransform;

    [Header("Offset Settings")]
    public Vector3 positionOffset;
    public float timeOffset;

    
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            // Calculate the target position with the offset
            Vector3 targetPosition = playerTransform.position + positionOffset;

            // Smoothly move the camera towards the target position using SmoothDamp
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, timeOffset);


            // Valeures recommandées 0.5 X offset Z -10 et 0.5 Time offset
        }
    }
}
