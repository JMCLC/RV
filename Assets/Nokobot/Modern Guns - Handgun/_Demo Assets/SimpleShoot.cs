using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO; // For file operations
using System.Text; // For string building
using System;
using UnityEngine.SceneManagement;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public TextMeshPro targetCounterText; // Reference to TextMeshPro for target counter

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    [Tooltip("Show Trajectory Line")] [SerializeField] private bool showLine = true;

    [Header("Player")]
    [SerializeField] private GameObject player;

    private LineRenderer lineRenderer;
    private int totalShots = 0;
    private int targetsShot = 0;
    private int targetsTotal = 0;
    private int numberOfTargetsNeededPractice = 3;
    private int numberOfTargetsNeededDemo = 6;
    private int points = 0;
    public int userId = 1;
    private string locomotionType = null;

    private float sessionStartTime;
    private string dataFilePath;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        if (sessionStartTime == null)
            sessionStartTime = Time.time;

        if (dataFilePath == null)
            dataFilePath = Path.Combine(Application.persistentDataPath, "UserExperienceData.csv");

        if (locomotionType == null)
            locomotionType = SceneManager.GetActiveScene().name;
        
        if (!File.Exists(dataFilePath)) {
            File.WriteAllText(dataFilePath, "ID;Locomotion;Points;Accuracy;SessionDuration\n");
            this.userId = 1;
        } else {
            userId = GetLastUserIdFromCsv(dataFilePath) + 1;
        }

        Debug.Log("Data file path: " + dataFilePath);

        // Set up the LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f; // Make the line thinner
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Add a basic material
        lineRenderer.startColor = new Color(0.3f, 0.3f, 0.3f); // Darker gray color
        lineRenderer.endColor = new Color(0.3f, 0.3f, 0.3f); // Darker gray color
        lineRenderer.enabled = false;
        // Set up the target Counter Text
        UpdateTargetsTotal();
        if (targetCounterText != null)
        {
            UpdateTargetCounterText();
        }
    }

    private int GetLastUserIdFromCsv(string filePath){
        try
            {
                string[] lines = File.ReadAllLines(filePath);

                // Skip the header and get the last data row
                if (lines.Length > 1)
                {
                    string lastLine = lines[lines.Length - 1];
                    string[] columns = lastLine.Split(';'); // Split the row by ';'

                    if (columns.Length > 0 && int.TryParse(columns[0], out int lastId))
                    {
                        return lastId; // Return the parsed ID
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error reading the CSV file: " + e.Message);
            }

            return 0; // Return 0 if no valid ID was found
    }

    public void StartShoot() {
        // Remove the previous line if it exists
        if (lineRenderer != null && lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
        }

        gunAnimator.SetTrigger("Fire");
    }

    //This function creates the bullet behavior
    void Shoot()
    {
        // Clear the previous trajectory visualization
        if (lineRenderer != null && lineRenderer.enabled)
        {
            lineRenderer.enabled = false;
        }

        if (muzzleFlashPrefab)
        {
            //Create the muzzle flash
            GameObject tempFlash;
            tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

            //Destroy the muzzle flash effect
            Destroy(tempFlash, destroyTimer);
        }

        //cancels if there's no bullet prefab
        if (!bulletPrefab)
        { return; }

        // Create a bullet and add force on it in direction of the barrel
        GameObject bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(barrelLocation.forward * shotPower);
        totalShots++;

        // Ignore collisions for the first 0.2 seconds
        Collider bulletCollider = bullet.GetComponent<Collider>();
        bulletCollider.enabled = false;
        StartCoroutine(EnableCollisionAfterDelay(bulletCollider, 0.2f));

        // Draw the trajectory for visualization if showLine is true
        if (showLine)
        {
            StartCoroutine(DrawTrajectory(bulletRb));
        }
    }

    //This function creates a casing at the ejection slot
    void CasingRelease()
    {
        //Cancels function if ejection slot hasn't been set or there's no casing
        if (!casingExitLocation || !casingPrefab)
        { return; }

        //Create the casing
        GameObject tempCasing;
        tempCasing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        //Add force on casing to push it out
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(UnityEngine.Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, UnityEngine.Random.Range(100f, 500f), UnityEngine.Random.Range(100f, 1000f)), ForceMode.Impulse);

        //Destroy casing after X seconds
        Destroy(tempCasing, destroyTimer);
    }

    private IEnumerator EnableCollisionAfterDelay(Collider collider, float delay)
    {
        yield return new WaitForSeconds(delay);
        collider.enabled = true;
    }

    private IEnumerator DrawTrajectory(Rigidbody bulletRb)
    {
        lineRenderer.enabled = true;

        float timeStep = 0.05f;
        float elapsedTime = 0f;
        Vector3 currentPosition = barrelLocation.position;
        lineRenderer.SetPosition(0, currentPosition);

        while (elapsedTime < 10f)
        {
            if (bulletRb) {
            // Simulate the bullet trajectory
              Vector3 nextPosition = currentPosition + bulletRb.linearVelocity * timeStep + 0.5f * Physics.gravity * timeStep * timeStep;
              RaycastHit hit;
  
              // Ignore collisions for the first 0.2 seconds
              if (elapsedTime > 0.2f && Physics.Linecast(currentPosition, nextPosition, out hit))
              {
                  // If it hits something, set the end position at the hit point and break
                  lineRenderer.SetPosition(1, hit.point);
                  break;
              }
              else
              {
                  // Otherwise, extend the line to the next position
                  lineRenderer.SetPosition(1, nextPosition);
              }
  
              // Update for next iteration
              currentPosition = nextPosition;
            }
            elapsedTime += timeStep;

            yield return new WaitForSeconds(timeStep);
        }

        // Disable the LineRenderer after the visualization is complete
        yield return new WaitForSeconds(10f - elapsedTime);
        lineRenderer.enabled = false;
    }

    private void UpdateTargetCounterText()
    {
        if (targetCounterText != null)
        {
            if (this.locomotionType.Contains("Practice")) {
                targetCounterText.text = $"Targets: {targetsShot}/{numberOfTargetsNeededPractice}";
            } else {
                targetCounterText.text = $"Targets: {targetsShot}/{numberOfTargetsNeededDemo}";
            }
        }
    }

    public void IncrementTargetsShot(GameObject target, int points)
    {
        // Check if the target has already been shot
        TargetColisionHandler targetBehavior = target.GetComponent<TargetColisionHandler>();
        if (targetBehavior != null && targetBehavior.isShot)
        {
            Debug.LogWarning("Target has already been shot: " + target.name);
            return; // Exit if the target is already processed
        }

        // Mark the target as shot
        if (targetBehavior != null)
        {
            targetBehavior.OnHit();
        }

        // Recalculate the number of remaining active targets
        int activeTargets = GameObject.FindGameObjectsWithTag("Target").Length;

        // Increment targetsShot and update points
        targetsShot++;
        this.points += points;
        UpdateTargetCounterText();

        Debug.Log($"Target shot: {target.name}. Points: {points}. Remaining targets: {activeTargets - 1}");
        if (this.locomotionType == "ArmSwing" && (targetsShot == numberOfTargetsNeededDemo || activeTargets == 3 || targetsTotal - activeTargets == numberOfTargetsNeededDemo)) {
            EndSessionAndSaveData(this.points, ((float)targetsShot / totalShots) * 100);
        }
        // Check if all targets have been shot
        if (this.locomotionType.Contains("Practice")) {
            if (targetsShot == numberOfTargetsNeededPractice || targetsTotal - activeTargets == numberOfTargetsNeededPractice) {
                player.transform.position = new Vector3(60, 18.2f, -3);
                player.transform.rotation = Quaternion.identity; // Reset player rotation
            }
        } else {
            if (targetsShot == numberOfTargetsNeededDemo || targetsTotal - activeTargets == numberOfTargetsNeededDemo) {
                player.transform.position = new Vector3(60, 18.2f, -3);
                player.transform.rotation = Quaternion.identity; // Reset player rotation
                EndSessionAndSaveData(this.points, ((float)targetsShot / totalShots) * 100);
            }
        }
    }


    // Update the total number of targets in the scene
    private void UpdateTargetsTotal()
    {
        targetsTotal = GameObject.FindGameObjectsWithTag("Target").Length;
        UpdateTargetCounterText();
    }

    public void EndSessionAndSaveData(float points, float accuracy)
    {
        float sessionDuration = Time.time - sessionStartTime;
        sessionDuration = Mathf.Round(sessionDuration * 100f) / 100f; // Round to 2 decimal places

        // Build a CSV row for the session
        StringBuilder csvRow = new StringBuilder();
        csvRow.AppendFormat("{0};{1};{2};{3};{4}\n", 
                            userId,
                            locomotionType,
                            points, 
                            accuracy, 
                            sessionDuration);

        // Append the row to the CSV file
        File.AppendAllText(dataFilePath, csvRow.ToString());
        // if (locomotionType == "ArmSwing") {
        //     this.userId++;
        // }
        Debug.Log("User experience data saved to CSV: " + csvRow.ToString());
    }
}
