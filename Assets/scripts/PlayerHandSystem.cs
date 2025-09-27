using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandSystem : MonoBehaviour
{
    [Header("Hand Settings")]
    public Transform handPosition;
    public float reachDistance = 2f;
    public KeyCode pickupKey = KeyCode.E;
    
    private Camera playerCam;
    private PickupableObject heldItem;
    private PickupableObject targetItem;
    
    void Start()
    {
        playerCam = Camera.main;
        if (playerCam == null)
            playerCam = GetComponentInChildren<Camera>();
            
        if (handPosition == null)
        {
            GameObject handPoint = new GameObject("HandPosition");
            handPoint.transform.SetParent(playerCam.transform);
            handPoint.transform.localPosition = new Vector3(0.3f, -0.2f, 0.5f);
            handPosition = handPoint.transform;
        }
    }
    
    void Update()
    {
        FindTargetItem();
        
        if (Keyboard.current[Key.E].wasPressedThisFrame)

        {
            if (heldItem == null)
            {
                TryPickupItem();
            }
            else
            {
                TryDropItem();
            }
        }
    }
    
    void FindTargetItem()
    {
        Ray lookRay = new Ray(playerCam.transform.position, playerCam.transform.forward);
        RaycastHit hitInfo;
        
        if (Physics.Raycast(lookRay, out hitInfo, reachDistance))
        {
            PickupableObject item = hitInfo.collider.GetComponent<PickupableObject>();
            
            if (item != null && item.canBePickedUp && !item.IsHeld())
            {
                targetItem = item;
            }
            else
            {
                targetItem = null;
            }
        }
        else
        {
            targetItem = null;
        }
    }
    
    void TryPickupItem()
    {
        if (targetItem != null && handPosition != null)
        {
            heldItem = targetItem;
            heldItem.Pickup(handPosition);
            targetItem = null;
        }
    }
    
    void TryDropItem()
    {
        if (heldItem != null)
        {
            Vector3 dropDirection = playerCam.transform.forward;
            heldItem.ThrowTowards(dropDirection, 2f);
            heldItem = null;
        }
    }
}