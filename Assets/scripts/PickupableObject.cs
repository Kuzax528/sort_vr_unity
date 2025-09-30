using UnityEngine;

public class PickupableObject : MonoBehaviour
{
    [Header("Pickup Settings")]
    public bool canBePickedUp = true;
    
    private bool isHeld = false;
    private Vector3 originalScale;
    
    void Start()
    {
        originalScale = transform.localScale;
        
        if (GetComponent<Rigidbody>() == null)
            gameObject.AddComponent<Rigidbody>();
    }
    
    public bool IsHeld() { return isHeld; }
    
    public void Pickup(Transform handPosition)
    {
        isHeld = true;
        canBePickedUp = false;
        
        transform.SetParent(handPosition);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().isTrigger = true;
        
        Debug.Log("Picked up: " + gameObject.name);
    }
    
    public void Release()
    {
        isHeld = false;
        canBePickedUp = true;
        
        transform.SetParent(null);
        transform.localScale = originalScale;
        
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Collider>().isTrigger = false;
        
        Debug.Log("Released: " + gameObject.name);
    }
    
    public void ThrowTowards(Vector3 direction, float force = 5f)
    {
        Release();
        GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.VelocityChange);
    }
}