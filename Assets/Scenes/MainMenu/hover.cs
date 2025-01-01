using UnityEngine;

public class Hover : MonoBehaviour
{
    public float moveSpeed = 2f; 
    public float amplitude = 1f; 

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; 
    }

    void Update()
    {
        // Move the object up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}