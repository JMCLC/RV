using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Get the collided GameObject
        GameObject targetObject = collision.gameObject;

        // Check if the target object has a TargetColisionHandler component
        TargetColisionHandler targetHandler = targetObject.GetComponent<TargetColisionHandler>();

        int points = 0; // Points for the hit object

        if (targetHandler != null)
        {
            // Determine points based on the child object's name or tag
            if (targetObject.name == "1")
            {
                points = 4;
            }
            else if (targetObject.name == "2")
            {
                points = 6;
            }
            else if (targetObject.name == "3")
            {
                points = 8;
            }
            else if (targetObject.name == "4")
            {
                points = 10;
            }
            else if (targetObject.CompareTag("Target"))
            {
                points = 2;
            }

            Debug.Log($"Bullet hit: {targetObject.name}, Awarded Points: {points}");
            // If the name is "1", "2", "3", or "4", switch to the parent object
            if (targetObject.name == "1" || targetObject.name == "2" || targetObject.name == "3" || targetObject.name == "4")
            {
                if (targetObject.transform.parent != null)
                {
                    targetObject = targetObject.transform.parent.gameObject;
                    targetHandler = targetObject.GetComponent<TargetColisionHandler>();
                    Debug.Log("Switched to parent object: " + targetObject.name);
                }
            }
        }

        // If a valid TargetColisionHandler is found, process the hit
        if (targetHandler != null && targetObject.activeSelf)
        {
            // targetHandler.OnHit();
            Debug.Log($"Bullet hit: {targetObject.name}, Awarded Points: {points}");

            // Increment the robot shot count with points
            SimpleShoot simpleShoot = FindObjectOfType<SimpleShoot>();
            if (simpleShoot != null)
            {
                simpleShoot.IncrementTargetsShot(targetObject, points);
            }

            // Destroy the target object if necessary
            Destroy(targetObject);
        }

        // Destroy the bullet after collision
        Destroy(gameObject);
    }
}
