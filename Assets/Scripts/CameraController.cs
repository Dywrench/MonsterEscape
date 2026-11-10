using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Referencia al jugador.
    public GameObject player;

    // Guarda la coordenada en 3 dimensiones (X, Y, Z).
    private Vector3 offset;

    // Se ejecuta una sola vez al iniciar.
    void Start()
    {
        // Obtiene la posición de la cámara y la resta con la del jugador
        // para guardar la distancia entre ambos.
        offset = transform.position - player.transform.position;
    }

    // Se ejecuta una vez por cada frame.
    void Update()
    {
        
    }
    void LateUpdate()
    {
        // Actualiza la posición de la cámara a la del jugador más el offset.
        transform.position = player.transform.position + offset;
    }
}