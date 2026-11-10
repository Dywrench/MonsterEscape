using UnityEngine;

public class CubeController : MonoBehaviour
{
     Material material;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<Renderer>().material;
        material.color = Color.black; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RotateCube ()
    {
        // Rota el cubo 45 grados en los ejes X, Y y Z (45 grados en cada eje)
        
        transform.Rotate(new Vector3(45, 45, 45));
    }
    public void scaleCube (float value)
    {
        // Escala el cubo al doble de su tamaño original en los ejes X, Y y Z
        transform.localScale = new Vector3(value, value, value);
    }

    public void CambiarColor (int opcion)
    {
        Debug.Log("parametro: " + opcion);
        switch (opcion)
        {
            case 0:
                Debug.Log("Opcion 1");
                material.color = Color.black;
                break;
            case 1:
                Debug.Log("Opcion 2");  
                material.color = Color.red;
                break;
            case 2:
                Debug.Log("Opcion 3");
                material.color = Color.yellow;
                break;
            default:
                Debug.Log("Opción inválida");
                break;
        }
    }
}
