using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoxSlotTrigger : MonoBehaviour
{
    private BoxSlot2 slot;
    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
        slot = GetComponentInParent<BoxSlot2>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.SetNearbySlot2(slot);
            if (player.carriedBoxStatus && !slot.hasBox)
                slot.UpdateMaterial(slot.highlightMat);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ClearNearbySlot2(slot);
            if (!slot.hasBox)
                slot.UpdateMaterial(slot.defaultMat);
        }
    }
}
