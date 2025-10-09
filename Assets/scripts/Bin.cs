using System.Collections;
using UnityEngine;

public class Bin : MonoBehaviour
{
    [Header("Bin Settings")]
    public Transform dropPoint;
    
    [Header("Visual Feedback")]
    public Color highlightColor = Color.green;
    public float highlightIntensity = 2f;
    
    private Renderer[] binRenderers;
    private Material[] originalMaterials;
    private bool isHighlighted = false;

    void Start()
    {
        // Get all renderers in the bin
        binRenderers = GetComponentsInChildren<Renderer>();
        
        // Store original materials
        originalMaterials = new Material[binRenderers.Length];
        for (int i = 0; i < binRenderers.Length; i++)
        {
            originalMaterials[i] = binRenderers[i].material;
        }
    }

    public void Highlight(bool enable)
    {
        if (isHighlighted == enable) return;
        isHighlighted = enable;

        foreach (var renderer in binRenderers)
        {
            if (enable)
            {
                // Add highlight effect
                renderer.material.SetColor("_EmissionColor", highlightColor * highlightIntensity);
                renderer.material.EnableKeyword("_EMISSION");
            }
            else
            {
                // Remove highlight
                renderer.material.DisableKeyword("_EMISSION");
            }
        }
    }

    public void ReceiveObject(GameObject obj)
    {
        if (obj == null) return;

        // Start drop animation
        StartCoroutine(DropAnimation(obj));
        
        Debug.Log(obj.name + " placed in bin: " + name);
    }

    IEnumerator DropAnimation(GameObject obj)
    {
        Vector3 startPos = obj.transform.position;
        Vector3 targetPos = dropPoint != null ? dropPoint.position : transform.position;
        
        float duration = 0.5f;
        float elapsed = 0f;

        // Animate the object falling into the bin
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Smooth curve (ease in)
            t = t * t;
            
            obj.transform.position = Vector3.Lerp(startPos, targetPos, t);
            
            // Optional: add rotation while falling
            obj.transform.Rotate(Vector3.up, 360f * Time.deltaTime);
            
            yield return null;
        }

        // Final position
        obj.transform.position = targetPos;

        // Disable physics so it stays in bin
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Mark as not pickable anymore
        PickupableObject pickup = obj.GetComponent<PickupableObject>();
        if (pickup != null)
            pickup.canBePickedUp = false;

        // Optional: scale down or fade out after a delay
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(ScaleDown(obj));
    }

    IEnumerator ScaleDown(GameObject obj)
    {
        Vector3 originalScale = obj.transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            obj.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        // Destroy or hide the object
        obj.SetActive(false);
        // Or: Destroy(obj);
    }

    // Visual helper in Scene view
    void OnDrawGizmos()
    {
        if (dropPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(dropPoint.position, 0.2f);
        }
    }
}