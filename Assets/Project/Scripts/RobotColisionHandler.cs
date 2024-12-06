using UnityEngine;

public class RobotColisionHandler : MonoBehaviour
{
    [Header("Explosion Prefab")]
    public GameObject explosionPrefab; // Reference to the explosion prefab
    [Tooltip("Specify time to destroy the explosion object")] [SerializeField] private float destroyTimer = 2f;
    private AudioSource explosionAudioSource;

    void Start()
    {
        // Get all AudioSources attached to the GameObject
        AudioSource[] audioSources = GetComponents<AudioSource>();

        // Assuming the second AudioSource is for the explosion sound
        if (audioSources.Length > 1)
        {
            explosionAudioSource = audioSources[1];
        }
        else
        {
            // Add an AudioSource if none exist (fallback scenario)
            explosionAudioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnHit()
    {
        if (explosionPrefab)
        {
            // Create the explosion effect
            GameObject tempExplosion = Instantiate(explosionPrefab, transform.position + new Vector3(0, 1.2f, 0), transform.rotation);
            
            // Destroy the explosion effect after a set time
            Destroy(tempExplosion, destroyTimer);
        }
        // Play the explosion sound using the assigned AudioSource
        if (explosionAudioSource != null)
        {
            // explosionAudioSource.Play();
        }

        // Handle other logic when the robot is hit (e.g., disabling or destroying the robot)
        Destroy(gameObject);
    }
}
