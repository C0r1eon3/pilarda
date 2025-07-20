using UnityEngine;

public class PocketTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            Rigidbody2D rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Topu yok etmeden önce CueController'a haber ver
                FindObjectOfType<CueController>().OnBallPotted(rb);
            }
        }
    }
}
