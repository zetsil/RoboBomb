using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class create_game_plane : MonoBehaviour
{
    public GameObject objectToDuplicate; // Assign the object you want to duplicate here

    // Start is called before the first frame update
    void Start()
    {
         // Get the MeshRenderer component
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        // Get the size of the object (in local space)
        Vector3 objectSize = meshRenderer.bounds.size;

         // Get the width and height (size along X and Y axes)
        float width = objectSize.x;
        float height = objectSize.z;

        Debug.Log("Object width: " + width);
        Debug.Log("Object height: " + height);

        // Loop through to create a grid of objects
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                // Instantiate the new object
                GameObject newObject = Instantiate(objectToDuplicate);

                // Calculate the new position based on the grid index and object size
                Vector3 newPosition = objectToDuplicate.transform.position + new Vector3(i * (width + 0.1f), 0, j * (height + 0.1f));

                // Set the position of the new object
                newObject.transform.position = newPosition;
            }
        }
}

        
    

    // Update is called once per frame
    void Update()
    {
         // Check if a key (e.g., spacebar) is pressed to duplicate the object
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Duplicate the object
            GameObject newObject = Instantiate(objectToDuplicate);
            
            // Optionally, set the position of the new object (offset by some value)
            newObject.transform.position = objectToDuplicate.transform.position + new Vector3(1.1f, 0, 0); // Move duplicate 2 units along the X-axis
        }
    
        
    }
}
