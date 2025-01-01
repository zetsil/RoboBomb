using UnityEngine;
using ActionString = System.Action<string>;

public class PowerUp : MonoBehaviour
{
    public float moveSpeed = 2f;   // Speed of up and down movement
    public float rotateSpeed = 50f; // Speed of rotation
    public float amplitude = 0.5f;  // Amplitude of the up and down movement
    private PlayerBomb playerBombPower;

    private Vector3 startPosition;

    private AudioSource audioSource;
    private event ActionString onNarrationSignal;
    
    void Awake(){
        audioSource = gameObject.GetComponent<AudioSource>();
        onNarrationSignal = Narator.Instance.TriggerNarration;
    }
    void Start()
    {
        // Store the initial position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Move the object up and down
        float newY = startPosition.y + Mathf.Sin(Time.time * moveSpeed) * amplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Rotate the object
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
           playerBombPower = other.gameObject.GetComponent<PlayerBomb>();
           playerBombPower.setActive();
           Destroy(gameObject,3f);
           HideMesh();
           onNarrationSignal?.Invoke("you_will_be_juge");
        }
           
    }

    void HideMesh(){
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false; // Hide the bomb visually
        }
        audioSource.Play();
        }
}
