using UnityEngine;

public class DisposalBin : MonoBehaviour
{
    [Header("Bin Settings")]
    public string binType = "Disposal Bin";
    public bool deleteItems = true;
    
    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }
    
    void OnTriggerEnter(Collider other)
    {
        PickupableObject item = other.GetComponent<PickupableObject>();
        
        if (item != null && !item.IsHeld())
        {
            Debug.Log(item.name + " disposed in " + binType);
            
            if (deleteItems)
            {
                Destroy(other.gameObject, 0.1f);
            }
        }
    }
}