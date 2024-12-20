using UnityEngine;

public class TargetColisionHandler : MonoBehaviour
{
    private AudioSource audioSource;
    public bool isShot = false;

    void Start()
    {
        // Get all AudioSources attached to the GameObject
        audioSource = GetComponent<AudioSource>();
    }

    public void OnHit()
    {
        // Play the explosion sound using the assigned AudioSource
        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Handle other logic when the robot is hit (e.g., disabling or destroying the robot)
        isShot = true;
        Destroy(gameObject);
    }
}
