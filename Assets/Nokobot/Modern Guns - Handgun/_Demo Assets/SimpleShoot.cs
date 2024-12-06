using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[AddComponentMenu("Nokobot/Modern Guns/Simple Shoot")]
public class SimpleShoot : MonoBehaviour
{
    [Header("Prefab Refrences")]
    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public TextMeshPro robotCounterText; // Reference to TextMeshPro for robot counter

    [Header("Location Refrences")]
    [SerializeField] private Animator gunAnimator;
    [SerializeField] private Transform barrelLocation;
    [SerializeField] private Transform casingExitLocation;

    [Header("Settings")]
    [Tooltip("Specify time to destory the casing object")] [SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")] [SerializeField] private float shotPower = 500f;
    [Tooltip("Casing Ejection Speed")] [SerializeField] private float ejectPower = 150f;
    [Tooltip("Show Trajectory Line")] [SerializeField] private bool showLine = true;

    private LineRenderer lineRenderer;
    private int robotsShot = 0;
    private int robotsTotal = 0;

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        if (gunAnimator == null)
            gunAnimator = GetComponentInChildren<Animator>();

        // Set up the LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f; // Make the line thinner
        lineRenderer.endWidth = 0.02f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Add a basic material
        lineRenderer.startColor = new Color(0.3f, 0.3f, 0.3f); // Darker gray color
        lineRenderer.endColor = new Color(0.3f, 0.3f, 0.3f); // Darker gray color
        lineRenderer.enabled = false;

        // Set up the Robot Counter Text
        UpdateRobotsTotal();
        if (robotCounterText != null)
        {
            UpdateRobotCounterText();
        }
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
        tempCasing.GetComponent<Rigidbody>().AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        //Add torque to make casing spin in random direction
        tempCasing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);

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

    private void UpdateRobotCounterText()
    {
        if (robotCounterText != null)
        {
            robotCounterText.text = $"Robots: {robotsShot}/{robotsTotal}";
        }
    }

    // Call this function whenever a robot is shot
    public void IncrementRobotsShot()
    {
        robotsShot++;
        UpdateRobotCounterText();
    }

    // Update the total number of robots in the scene
    private void UpdateRobotsTotal()
    {
        robotsTotal = GameObject.FindGameObjectsWithTag("Robot").Length;
        UpdateRobotCounterText();
    }
}
