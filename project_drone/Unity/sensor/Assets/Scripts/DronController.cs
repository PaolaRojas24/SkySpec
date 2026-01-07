using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float thrust = 30f;
    private Rigidbody rb;

    [Header("Hélices")]
    public GameObject[] propellers;
    public float spinSpeed = 1000f;

    [Header("Sensor de proximidad")]
    public Transform sensorPoint; // Punto de origen del sensor
    public float sensorRange = 20f; // Distancia máxima del sensor
    public LayerMask obstacleLayer; // Capas de objetos a detectar

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

        // Verificar proximidad
        CheckProximity();
    }

    void CheckProximity()
    {
        // Raycast desde el punto del sensor
        Vector3 origin = sensorPoint != null ? sensorPoint.position : transform.position;

        if (Physics.Raycast(origin, transform.forward, out RaycastHit hit, sensorRange, obstacleLayer))
        {
            Debug.Log("Objeto detectado a " + hit.distance + " metros: " + hit.collider.name);
        }

        // Dibujar raycast para debug
        Debug.DrawRay(origin, transform.forward * sensorRange, Color.red);
    }
}