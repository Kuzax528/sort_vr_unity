using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMouseGrab : MonoBehaviour
{
    public float grabDistance = 10f;
    public Transform handPosition;
    private PickupableObject heldObject = null;

    void Update()
    {
        // Only allow grabbing if cursor is visible
        if (Cursor.visible && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (heldObject == null)
            {
                TryGrab();
            }
            else
            {
                heldObject.Release();
                heldObject = null;
            }
        }
    }

    void TryGrab()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            PickupableObject pickup = hit.collider.GetComponent<PickupableObject>();
            if (pickup != null && pickup.canBePickedUp)
            {
                pickup.Pickup(handPosition);
                heldObject = pickup;
            }
        }
    }
}