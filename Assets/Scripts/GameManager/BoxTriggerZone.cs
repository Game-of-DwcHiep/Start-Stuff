using UnityEngine;

public class BoxTriggerZone : MonoBehaviour
{
    private BoxHighlighter parentBox;

    void Start()
    {
        parentBox = GetComponentInParent<BoxHighlighter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parentBox.SetHighlight(true);
            var pickup = other.GetComponent<PlayerController>();
            if (pickup != null)
                pickup.SetNearbyBox(parentBox);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parentBox.SetHighlight(false);
            var pickup = other.GetComponent<PlayerController>();
            if (pickup != null)
                pickup.ClearNearbyBox(parentBox);
        }
    }
}
