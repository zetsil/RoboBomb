using UnityEngine;

public class ComplexRotation : MonoBehaviour
{
    // Customizable rotation speeds for each axis
    public float rotationSpeedX = 30f;
    public float rotationSpeedY = 60f;
    public float rotationSpeedZ = 90f;

    // Randomize rotation speeds?
    public bool randomizeSpeeds = false;

    // Randomize starting rotation?
    public bool randomizeStartingRotation = false;

    // Internal variables to store random offsets
    private float randomOffsetX, randomOffsetY, randomOffsetZ;

    void Start()
    {
        // If enabled, randomize the rotation speeds
        if (randomizeSpeeds)
        {
            rotationSpeedX = Random.Range(10f, 50f);
            rotationSpeedY = Random.Range(10f, 50f);
            rotationSpeedZ = Random.Range(10f, 50f);
        }

        // If enabled, randomize the starting rotation
        if (randomizeStartingRotation)
        {
            randomOffsetX = Random.Range(0f, Mathf.PI * 2f);
            randomOffsetY = Random.Range(0f, Mathf.PI * 2f);
            randomOffsetZ = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    void Update()
    {
        // Calculate the rotation angles based on time, speed, and random offset
        float rotationX = Time.time * rotationSpeedX + randomOffsetX;
        float rotationY = Time.time * rotationSpeedY + randomOffsetY;
        float rotationZ = Time.time * rotationSpeedZ + randomOffsetZ;

        // Apply the calculated rotation to the object
        transform.rotation = Quaternion.Euler(rotationX, rotationY, rotationZ);
    }
}