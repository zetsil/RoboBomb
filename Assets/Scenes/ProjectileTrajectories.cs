using UnityEngine;

public class ProjectileTrajectoryDrawer : MonoBehaviour
{
    public Transform launchPoint; // The position from where the projectile is launched
    public Transform target; // The target position (optional, or you can use a direction)
    public float launchForce = 10f; // The force to launch the projectile
    public float launchAngle = 45f; // The launch angle in degrees
    public int trajectoryResolution = 30; // Number of points to calculate for the trajectory
    public LineRenderer lineRenderer; // The LineRenderer component to draw the trajectory
    public float gravity = 9.81f; // Gravity

    public float time = 0.6f; // Starting time
    public float timeResetValue = 0.6f; // Value to reset the timer to
    public GameObject bombPrefab;        // The bomb prefab to instantiate

    public float maxLaunchForce = 120f;
    public float minLaunchForce = 20f;




    private void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        FindTarget();
    }


    private void FindTarget()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'Player' found in the scene.");
        }
    }

    private void Update()
    {
        // Decrease the time
        time -= Time.deltaTime;

         // Check if time reaches zero or below
        if (time <= 0f)
        {

        // Randomly choose a "Floor" object as the new target
        GameObject[] floorObjects = GameObject.FindGameObjectsWithTag("Floor");

        if (floorObjects.Length > 0)
        {
            // Randomly select one object from the floorObjects array
            int randomIndex = Random.Range(0, floorObjects.Length);
            target = floorObjects[randomIndex].transform; // Set the selected object as the target

            float optimalForce = CalculateOptimalLaunchForce(launchPoint.position, target.position);
            launchForce = Mathf.Clamp(optimalForce, minLaunchForce, maxLaunchForce);

            // Calculate the launch angle based on the new target and launch force
            launchAngle = CalculateLaunchAngle(launchPoint.position, target.position, optimalForce);

            // Launch the projectile
            LaunchObject();
            time = timeResetValue;
        }

        }
    
        //  DrawTrajectory();
    }

    void DrawTrajectory()
    {
        if(target == null) return;
        // Calculate the initial velocity components based on the launch angle
        float gravity = Physics.gravity.magnitude;
        
        // Convert angle to radians
        float angleRad = Mathf.Deg2Rad * launchAngle;

        // Calculate initial velocity components
        float horizontalSpeed = launchForce * Mathf.Cos(angleRad); // Horizontal component of velocity
        float verticalSpeed = launchForce * Mathf.Sin(angleRad); // Vertical component of velocity

        // Initial velocity vector
        Vector3 velocity = (target.position - launchPoint.position).normalized;
        velocity = new Vector3(velocity.x * horizontalSpeed, verticalSpeed, velocity.z * horizontalSpeed);

        // Trajectory points
        Vector3[] trajectoryPoints = new Vector3[trajectoryResolution];
        float timeStep = 0.1f; // Time increment for each trajectory point

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i * timeStep;
            trajectoryPoints[i] = GetTrajectoryPoint(launchPoint.position, velocity, gravity, t);
        }

        // Update LineRenderer to draw the trajectory path
        lineRenderer.positionCount = trajectoryResolution;
        lineRenderer.SetPositions(trajectoryPoints);
    }

    private float CalculateOptimalLaunchForce(Vector3 start, Vector3 end)
    {
        Vector3 displacement = end - start;
        float yOffset = displacement.y;
        displacement.y = 0f;
        float xzDistance = displacement.magnitude;

        float angleRad = 45 * Mathf.Deg2Rad; // Launch at 45 degrees for max range

        // Formula for optimal velocity (derived from projectile motion equations)
        float velocity = Mathf.Sqrt((gravity * xzDistance * xzDistance) / (2 * Mathf.Cos(angleRad) * (xzDistance * Mathf.Sin(angleRad) + yOffset * Mathf.Cos(angleRad))));

        return velocity;
    }

   void LaunchObject()
   {
        // Instantiate the bomb prefab at the launch position
        GameObject bombInstance = Instantiate(bombPrefab, launchPoint.position, Quaternion.identity);

        // Ensure the bomb instance has a Rigidbody
        Rigidbody rb = bombInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("The bomb prefab requires a Rigidbody component.");
            return;
        }

        // Get gravity magnitude
        float gravity = Physics.gravity.magnitude;

        // Convert angle to radians
        float angleRad = Mathf.Deg2Rad * launchAngle;

        // Calculate initial velocity components based on launch angle and force
        float horizontalSpeed = launchForce * Mathf.Cos(angleRad); // Horizontal component of velocity
        float verticalSpeed = launchForce * Mathf.Sin(angleRad);   // Vertical component of velocity

        // Calculate direction to target in 2D (ignore vertical for now)
        Vector3 directionToTarget = (target.position - launchPoint.position).normalized;

        // Final launch velocity vector
        Vector3 launchVelocity = new Vector3(
            directionToTarget.x * horizontalSpeed, // Horizontal X component
            verticalSpeed,                         // Vertical Y component
            directionToTarget.z * horizontalSpeed  // Horizontal Z component
        );

         // Check for NaN components and replace with random values
        if (float.IsNaN(launchVelocity.x) || float.IsNaN(launchVelocity.y) || float.IsNaN(launchVelocity.z))
        {
            Debug.LogWarning("Calculated launch velocity contained NaN values! Using random velocity.");
            launchVelocity = new Vector3(Random.Range(-5f, 5f), Random.Range(5f, 10f), Random.Range(-5f, 5f)); // Provide reasonable default values
        }

        // Apply the launch velocity to the bomb's Rigidbody
        rb.velocity = launchVelocity;

        // Optional: Log launch details for debugging
        Debug.Log("Bomb launched with velocity: " + launchVelocity);
    }


    // Function to calculate the position of the projectile at time 't'
    Vector3 GetTrajectoryPoint(Vector3 launchPosition, Vector3 velocity, float gravity, float time)
    {
        Vector3 displacement = velocity * time + 0.5f * new Vector3(0, -gravity, 0) * time * time;
        return launchPosition + displacement;
    }


     float CalculateLaunchAngle(Vector3 start, Vector3 target, float speed)
    {
        float deltaX = target.x - start.x;
        float deltaZ = target.z - start.z;
        float deltaY = target.y - start.y;

        float distanceHorizontal = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);

        float v2 = speed * speed;
        float v4 = v2 * v2;
        float g = gravity;
        float x = distanceHorizontal;
        float y = deltaY;

        // Calculate the angle using the quadratic formula solution for launch angle (HIGHER ANGLE)
        float angle = Mathf.Atan((v2 + Mathf.Sqrt(v4 - g * (g * x * x + 2 * y * v2))) / (g * x));

        // Check for unreachable target
        if (float.IsNaN(angle))
        {
            Debug.LogWarning("Target is unreachable with the given launch speed! Using max range angle.");

            // Calculate the maximum range angle (45 degrees if yOffset is 0)
            float maxRangeAngle = Mathf.Atan(v2 / (g * x));

            if (float.IsNaN(maxRangeAngle))
            {
                Debug.LogWarning("Target is unreachable even in max range. returning 0");
                return 0;
            }
            return maxRangeAngle * Mathf.Rad2Deg;
        }

        return angle * Mathf.Rad2Deg;
    }
}
