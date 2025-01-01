using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public float rotationSpeed = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {    
        // Rotește obiectul în jurul axei Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
