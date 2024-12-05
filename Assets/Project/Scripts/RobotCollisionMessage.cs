using UnityEngine;

public class RobotCollisionMessage : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("You bumped into Robot Kyle!");
        }
    }
}







