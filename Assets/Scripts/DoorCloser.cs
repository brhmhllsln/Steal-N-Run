using UnityEngine;

public class DoorCloser : MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindAnyObjectByType<PuzzleManager>().CloseAndLockDoors();
            Destroy(gameObject); 
        }
    }
}
