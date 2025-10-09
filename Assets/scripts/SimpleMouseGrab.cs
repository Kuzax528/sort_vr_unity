using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMouseGrab : MonoBehaviour
{
    public float grabDistance = 15f;
    public Transform handPosition; // Assign the "hand" bone from character skeleton

    private PickupableObject heldObject = null;
    private Bin currentHoveredBin = null;
    private int raycastLayerMask = ~0;
    private bool cursorMode = false;

    void Start()
    {
        int playerLayer = LayerMask.NameToLayer("Player");
        raycastLayerMask = (playerLayer != -1) ? ~(1 << playerLayer) : ~0;
        
        // Start with cursor locked
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Toggle cursor mode with E key
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            cursorMode = !cursorMode;
            
            if (cursorMode)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                
                if (currentHoveredBin != null)
                {
                    currentHoveredBin.Highlight(false);
                    currentHoveredBin = null;
                }
            }
        }

        // Handle highlighting when holding an object
        if (heldObject != null && cursorMode)
        {
            CheckBinHighlight();
        }

        if (cursorMode && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (heldObject == null)
            {
                TryGrab();
            }
            else
            {
                TryDropInBin();
            }
        }
    }

    void CheckBinHighlight()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance, raycastLayerMask))
        {
            Bin bin = hit.collider.GetComponentInParent<Bin>();
            
            if (bin != null && bin != currentHoveredBin)
            {
                if (currentHoveredBin != null)
                    currentHoveredBin.Highlight(false);
                
                bin.Highlight(true);
                currentHoveredBin = bin;
            }
            else if (bin == null && currentHoveredBin != null)
            {
                currentHoveredBin.Highlight(false);
                currentHoveredBin = null;
            }
        }
        else if (currentHoveredBin != null)
        {
            currentHoveredBin.Highlight(false);
            currentHoveredBin = null;
        }
    }

    void TryGrab()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        
        RaycastHit[] hits = Physics.RaycastAll(ray, grabDistance, raycastLayerMask);
        
        foreach (RaycastHit hit in hits)
        {
            PickupableObject pickup = hit.collider.GetComponentInParent<PickupableObject>();
            if (pickup != null && pickup.canBePickedUp)
            {
                pickup.Pickup(handPosition);
                heldObject = pickup;
                Debug.Log("Grabbed: " + pickup.name);
                return;
            }
        }
        
        Debug.Log("No pickupable object found");
    }

    void TryDropInBin()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance, raycastLayerMask))
        {
            Bin bin = hit.collider.GetComponentInParent<Bin>();
            if (bin != null)
            {
                bin.Highlight(false);
                currentHoveredBin = null;
                
                // Drop in bin with animation
                bin.ReceiveObject(heldObject.gameObject);
                heldObject = null;
                Debug.Log("Dropped in bin");
                return;
            }
        }

        // Drop normally if not on bin
        if (currentHoveredBin != null)
        {
            currentHoveredBin.Highlight(false);
            currentHoveredBin = null;
        }
        
        heldObject.Release();
        heldObject = null;
        Debug.Log("Dropped object");
    }
}