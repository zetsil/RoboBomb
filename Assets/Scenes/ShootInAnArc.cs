using UnityEngine;

public class ShootInAnArc : MonoBehaviour
{
    public Transform launchPoint; // The position from where the projectile is launched
    [SerializeField] private Transform target; // The target position (optional, or you can use a direction)
    public float launchForce = 10f; // The force to launch the projectile
    public float launchAngle = 45f; // The launch angle in degrees
    public int trajectoryResolution = 30; // Number of points to calculate for the trajectory
    public float gravity = 9.81f; // Gravity

    public float time = 0.6f; // Starting time
    public float timeResetValue = 0.6f; // Value to reset the timer to
    public GameObject bombPrefab;        // The bomb prefab to instantiate




    private void Start()
    {

        // Find the target with the "Player" tag
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Decrease the time
        time -= Time.deltaTime;

         // Check if time reaches zero or below
        if (time <= 0f)
        {
        // Randomly select a launch force between 20 and 40
        // launchForce = Random.Range(20, 40);

        // Randomly choose a "Floor" object as the new target
        // GameObject[] floorObjects = GameObject.FindGameObjectsWithTag("Floor");

        // if (floorObjects.Length > 0)
        // {
           
        // Calculate the launch angle based on the new target and launch force
        launchAngle = CalculateLaunchAngle(launchPoint.position, target.position, launchForce);
        // transform.rotation = AimTurret(target.position, launchAngle);

        // Launch the projectile
        LaunchObject();
        time = timeResetValue;

        //}

        }
    
         DrawTrajectory();
    }


    private Quaternion AimTurret(Vector3 targetPosition, float launchAngle)
    {

        // Calculate the direction to the target
        Vector3 directionToTarget = (targetPosition - launchPoint.position).normalized;

        // Create a rotation that includes both the direction and the launch angle
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget) * Quaternion.Euler(0, 0, -launchAngle); 

        // Rotate the turret to face the target with the launch angle applied
        return targetRotation;
        
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
        // lineRenderer.positionCount = trajectoryResolution;
        // lineRenderer.SetPositions(trajectoryPoints);
    }

   void LaunchObject()
   {


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
        transform.rotation = Quaternion.LookRotation(launchVelocity);

        // Instantiate the bomb prefab at the launch position
        GameObject bombInstance = Instantiate(bombPrefab, launchPoint.position, Quaternion.identity);

        // Ensure the bomb instance has a Rigidbody
        Rigidbody rb = bombInstance.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("The bomb prefab requires a Rigidbody component.");
            return;
        }

        // Apply the launch velocity to the bomb's Rigidbody
        rb.velocity = launchVelocity;
        

        // Optional: Log launch details for debugging
        Debug.Log("Bomb launched with velocity: " + launchVelocity);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 20f); // Draw a blue ray 2 units long
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
        float angle = Mathf.Atan((v2 + Mathf.Sqrt(v4 - g * (g * x * x + 2 * y * v2))) / (g * x)); // Changed - to +

        // Check for unreachable target
        if (float.IsNaN(angle))
        {
            Debug.LogWarning("Target is unreachable with the given launch speed!");
            return float.NaN;
        }

        return angle * Mathf.Rad2Deg;
    }
}
