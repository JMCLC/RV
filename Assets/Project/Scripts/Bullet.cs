using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a RobotCollisionHandler component
        RobotColisionHandler robotHandler = collision.gameObject.GetComponent<RobotColisionHandler>();
        if (robotHandler != null && collision.gameObject.activeSelf)
        {
            // Call the OnHit method if a RobotCollisionHandler is present
            robotHandler.OnHit();

            // Increment the robot shot count if the robot was active before OnHit
            SimpleShoot simpleShoot = FindObjectOfType<SimpleShoot>();
            if (simpleShoot != null)
            {
                simpleShoot.IncrementRobotsShot();
            }
        }

        // Destroy the bullet after collision
        Destroy(gameObject);
    }
}
