using System.Collections;
using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    [Header("Pickup Settings")]
    public bool canBePickedUp = true;
    
    [Header("Hand Follow Settings")]
    public float followSpeed = 20f;
    public float rotationSpeed = 15f;
    public Vector3 handOffset = new Vector3(0.1f, 0f, 0.1f); // Adjust in Inspector

    private bool isHeld = false;
    private Vector3 originalScale;
    private Rigidbody rb;
    private Collider col;
    private Transform handTransform;

    void Start()
    {
        originalScale = transform.localScale;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        col = GetComponent<Collider>();
        if (col == null)
            Debug.LogWarning("PickupableObject needs a Collider: " + name);
    }

    public bool IsHeld() => isHeld;

    public void Pickup(Transform handPosition)
    {
        if (!canBePickedUp || isHeld) return;

        isHeld = true;
        canBePickedUp = false;
        handTransform = handPosition;

        // Stop physics
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
        rb.useGravity = false;

        if (col != null) col.isTrigger = true;

        transform.localScale = originalScale;
        Debug.Log("Picked up: " + name);
    }

    public void Release()
    {
        if (!isHeld) return;

        isHeld = false;
        canBePickedUp = true;
        handTransform = null;

        // Restore physics
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (col != null)
        {
            col.isTrigger = true;
            StartCoroutine(ReenableColliderNextFrame());
        }

        transform.localScale = originalScale;
        Debug.Log("Released: " + name);
    }

    IEnumerator ReenableColliderNextFrame()
    {
        yield return null;
        yield return new WaitForSeconds(0.05f);
        if (col != null) col.isTrigger = false;
    }

    // Smooth follow the animated hand
    void LateUpdate()
    {
        if (isHeld && handTransform != null)
        {
            // Calculate target position with offset
            Vector3 targetPosition = handTransform.position + handTransform.TransformDirection(handOffset);
            Quaternion targetRotation = handTransform.rotation;

            // Smooth follow with interpolation
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}