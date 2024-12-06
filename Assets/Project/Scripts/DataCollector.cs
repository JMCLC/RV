using UnityEngine;
using System.IO; // For file operations
using System.Text; // For string building
using System;

public class UserExperienceDataCollector : MonoBehaviour
{
    private float sessionStartTime;

    private string dataFilePath;

    void Start()
    {
        // Start this when the scene is loaded
        sessionStartTime = Time.time;

        // Define the file path for the CSV file
        dataFilePath = Path.Combine(Application.persistentDataPath, "UserExperienceData.csv");

        // If the file doesn't exist, create it and add headers
        if (!File.Exists(dataFilePath))
        {
            File.WriteAllText(dataFilePath, "SessionDuration,Points,Accuracy\n");
        }

        Debug.Log("Data file path: " + dataFilePath);
    }

    // Call this method to end the session and save data
    public void EndSessionAndSaveData(float points, float accuracy)
    {
        float sessionDuration = Time.time - sessionStartTime;
        sessionDuration = Mathf.Round(sessionDuration * 100f) / 100f; // Round to 2 decimal places

        // Build a CSV row for the session
        StringBuilder csvRow = new StringBuilder();
        csvRow.AppendFormat("{0},{1},{2}\n", 
                            sessionDuration, 
                            points, 
                            accuracy);

        // Append the row to the CSV file
        File.AppendAllText(dataFilePath, csvRow.ToString());

        Debug.Log("User experience data saved to CSV: " + csvRow.ToString());
    }
}
