using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;


[RequireComponent(typeof(Rigidbody))]
public class PhysicsBasedPathFollower : MonoBehaviour
{
    private Transform target;                  // The target to navigate toward
    public float moveSpeed = 1f;             // Force applied to move toward target
    public float waypointTolerance = 0.5f;    // Distance threshold to consider a waypoint reached
    public float recalculatePathInterval = 0.5f; // Interval to recalculate path (in seconds)
    public float rotationSpeed = 1f;

    private NavMeshPath path;
    private Rigidbody rb;
    private int currentWaypoint = 0;
    private float lastPathCalculationTime = 0f;
    public NavMeshSurface navMeshSurface; // Assign this to a specific NavMeshSurface component


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        path = new NavMeshPath();
        // Find the player by tag
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found! Make sure the player object is tagged as 'Player'.");
        }
    }

    void Update()

    {
        RotateTowardsTarget();
        // Recalculate the path at set intervals or if path has reached the end
        if (Time.time - lastPathCalculationTime > recalculatePathInterval)
        {
            CalculatePathToTarget();
            lastPathCalculationTime = Time.time;
        }
        
        // Move toward the next waypoint if the path is available
        if (path != null && path.corners.Length > 0)
        {
            MoveAlongPath();
        }
    }

    void CalculatePathToTarget()
    {
        // Calculate the path to the target position
        if (target != null && NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
        {
            currentWaypoint = 0; // Reset to the start of the path
        }
    }

   void MoveAlongPath()
    {
        // Check if there are remaining waypoints
        if (currentWaypoint < path.corners.Length)
        {
            Vector3 targetPosition = path.corners[currentWaypoint];
            Vector3 direction = (targetPosition - transform.position).normalized;

            // Move towards the target position
            float step = moveSpeed * Time.deltaTime; // Calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // Check if we have reached the current waypoint
            if (Vector3.Distance(transform.position, targetPosition) < waypointTolerance)
            {
                currentWaypoint++; // Move to the next waypoint
            }
        }
    }

    void OnDrawGizmos()
    {
        // Visualize the calculated path in the editor
        if (path != null && path.corners.Length > 1)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
            }
        }
    }

    void RotateTowardsTarget()
    {
        // Calculate the direction to the player
        Vector3 directionToPlayer = target.position - transform.position;

        // Normalize the direction vector (important for consistent rotation)
        directionToPlayer.Normalize();

        // Create the rotation to make the X-axis face the player
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer, Vector3.up) * Quaternion.Euler(0, -90, 0);

        // Smoothly rotate towards the player using Slerp
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
               
    }
}
