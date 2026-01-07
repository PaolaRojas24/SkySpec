using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float thrust = 30f;
    private Rigidbody rb;

    [Header("Hélices")]
    public GameObject[] propellers;
    public float spinSpeed = 1000f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Aplicar fuerza vertical
        if (Input.GetKey(KeyCode.V))
        {
            rb.AddForce(Vector3.up * thrust);

            // Girar hélices
            foreach (GameObject prop in propellers)
            {
                prop.transform.Rotate(Vector3.forward * spinSpeed * Time.deltaTime);
            }
        }

        // Flechas para moverlo
        if (Input.GetKey(KeyCode.W)) rb.AddForce(Vector3.forward * thrust);
        if (Input.GetKey(KeyCode.S)) rb.AddForce(Vector3.back * thrust);
        if (Input.GetKey(KeyCode.A)) rb.AddForce(Vector3.left * thrust);
        if (Input.GetKey(KeyCode.D)) rb.AddForce(Vector3.right * thrust);
    }
}
