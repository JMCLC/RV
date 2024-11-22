using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class EventLogger : MonoBehaviour
{
    private string filePath;
    private Camera vrCamera;
    private float logInterval = 1.0f; // Time interval for logging camera position in seconds

    void Start()
    {
        // Define the path to the CSV log file
        filePath = Application.persistentDataPath + "/EventLog.csv";
        
        // Initialize VR Camera
        vrCamera = Camera.main;

        // Create CSV file and add the header
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "ID,Time,PosX,PosY,PosZ,RotX,RotY,RotZ,EventName,Date,DeltaTime\n");
        }

        // Start logging camera position
        StartCoroutine(LogCameraPosition());

        // Register grab event logging for all interactable objects
        RegisterGrabEventsForAllObjects();
    }

    IEnumerator LogCameraPosition()
    {
        int id = 0;
        while (true)
        {
            yield return new WaitForSeconds(logInterval);
            id++;
            string timestamp = System.DateTime.Now.ToString("HH:mm:ss:fff");
            Vector3 position = vrCamera.transform.position;
            Vector3 rotation = vrCamera.transform.eulerAngles;
            string date = System.DateTime.Now.ToString("ddMMyyyy_HHmmss");
            float deltaTime = Time.deltaTime;
            
            // Format data in CSV format
            string logEntry = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}\n", id, timestamp, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, "CameraPosition", date, deltaTime);
            File.AppendAllText(filePath, logEntry);
        }
    }

    public void LogEvent(string eventName)
    {
        int id = 9999; // Arbitrary ID for specific events
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss:fff");
        Vector3 position = vrCamera.transform.position;
        Vector3 rotation = vrCamera.transform.eulerAngles;
        string date = System.DateTime.Now.ToString("ddMMyyyy_HHmmss");
        float deltaTime = Time.deltaTime;

        // Format data in CSV format
        string logEntry = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}\n", id, timestamp, position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, eventName, date, deltaTime);
        File.AppendAllText(filePath, logEntry);
    }

    private void RegisterGrabEventsForAllObjects()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable[] allInteractables = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        foreach (UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable interactable in allInteractables)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        GameObject grabbedObject = args.interactableObject.transform.gameObject;
        if (grabbedObject.activeInHierarchy && this != null)
        {
            LogEvent("ObjectGrabbed: " + grabbedObject.name);
        }
    }
}

// The "EventLogger" script now handles logging for both camera position and grab events for all interactable objects in the scene.
// It uses XR Interaction Toolkit events to handle grabbing logic, allowing all XRGrabInteractable objects to be logged automatically.
