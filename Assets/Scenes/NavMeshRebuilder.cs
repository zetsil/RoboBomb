using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshRebuilder : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;    // Reference to the NavMeshSurface component
    public float recalculateInterval = 5f;    // Interval at which NavMesh is recalculated (in seconds)

    private void Awake()
    {
        // Get the NavMeshSurface component
        navMeshSurface = GetComponent<NavMeshSurface>();
        if (navMeshSurface == null)
        {
            Debug.LogError("NavMeshSurface component is missing!");
        }
    }

    private void Start()
    {
        // Start the coroutine to rebuild the NavMesh at intervals
        StartCoroutine(RecalculateNavMesh());
    }

    private IEnumerator RecalculateNavMesh()
    {
        while (true)
        {
            // Wait for the specified interval
            yield return new WaitForSeconds(recalculateInterval);

            // Rebuild the NavMesh if the surface is assigned
            if (navMeshSurface != null)
            {
                navMeshSurface.BuildNavMesh();
                Debug.Log("NavMesh recalculated.");
            }
        }
    }

}
