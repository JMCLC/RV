using UnityEngine;

public class RobotAudioController : MonoBehaviour
{
    private AudioSource audioSource;

    void Awake()
    {
        // Get the AudioSource component attached to the robot
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        // Start playing the audio when the robot becomes active
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    void OnDisable()
    {
        // Stop the audio when the robot is disabled
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
