using UnityEngine;

public class ShootController : MonoBehaviour
{
    public GameObject Player; //El objeto que representa al jugador en la escena
    public float TimeBetweenShooting = 1f;// Tiempo entre disparos
    public float range = 100f; // Rango del disparo

    float timer; // Temporizador para controlar el tiempo entre disparos
    Ray shootRay; // Rayo que representa la dirección del disparo
    RaycastHit shootHit; // Información sobre el objeto impactado por el rayo
    int shootableMask; // Máscara de capa para determinar qué objetos pueden ser disparados (unity lo asigna como int, por eso le ponemos int)
    LineRenderer gunLine; // Componente LineRenderer para dibujar la línea del disparo
    Light gunLight; // Componente Light para iluminar el disparo
    float effectsDisplayTime = 1.2f; // Tiempo que se muestran los efectos del disparo

    private InputSystem_Actions controls; // Variable para manejar los controles de entrada

    void Awake() // Se llama cuando el script se inicializa
    {
        shootableMask = LayerMask.GetMask("Shootable"); // Se obtiene la máscara de capa para los objetos disparables
        gunLine = GetComponent<LineRenderer>(); // Se obtiene el componente LineRenderer del objeto
        gunLight = GetComponent<Light>(); // Se obtiene el componente Light del objeto
        controls = new InputSystem_Actions(); // Se inicializa la variable de controles de entrada
    }

    void OnEnable()
    {
        controls.Enable(); // Se habilitan los controles de entrada cuando el objeto se activa
    }

    void OnDisable()
    {
        controls.Disable(); // Se deshabilitan los controles de entrada cuando el objeto se desactiva
    }

    void Update()
    {
        timer += Time.deltaTime; // Se incrementa el temporizador con el tiempo transcurrido desde el último frame

        if (timer >= TimeBetweenShooting * effectsDisplayTime) //el tiempo transcurrido desde el último disparo es mayor o igual al tiempo entre disparos multiplicado por el tiempo de visualización de efectos
        {
            DisableEffects();
        }

        if (controls.Player.Attack.WasPressedThisFrame()) // Se verifica si se ha presionado el botón de ataque en este frame
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 location = new Vector3( // Se define la posición del disparo
            Player.transform.position.x, //al valor de la posición en x del jugador
            Player.transform.position.y + 1.1f, //al valor de la posición en y del jugador se le suma 1.1 para que el disparo salga desde la altura del personaje
            Player.transform.position.z //al valor de la posición en z del jugador
        );

        timer = 0f; // Se reinicia el temporizador para controlar el tiempo entre disparos
        gunLine.enabled = true; // Se habilita el componente LineRenderer para dibujar la línea del disparo
        gunLight.enabled = true; // Se habilita el componente Light para iluminar el disparo
        shootRay.origin = location; // Se establece el origen del rayo en la posición del disparo
        shootRay.direction = transform.forward; // Se establece la dirección del rayo hacia adelante desde el objeto que contiene este script
        gunLine.SetPosition(0, location); //Se establece la posición inicial de la línea del disparo en la posición del disparo

        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask)) // Se realiza un raycast para detectar si el rayo impacta con algún objeto disparable dentro del rango especificado
        {
            Destroy(shootHit.collider.gameObject); // Se destruye el objeto impactado por el rayo
            gunLine.SetPosition(1, shootHit.point); // Se establece la posición final de la línea del disparo en el punto de impacto del rayo
        }
        else //si el rayo no impacta con ningún objeto disparable dentro del rango especificado
        {
            Debug.Log("No se impacto con ningun objeto"); // Se imprime un mensaje en la consola indicando que no se impactó con ningún objeto
            gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range); // Se establece la posición final de la línea del disparo en el punto más lejano del rayo dentro del rango especificado
        }
    }

    void DisableEffects() //desactiva los efectos visuales del disparo
    {
        gunLine.enabled = false;  //se desactiva el componente LineRenderer para dejar de dibujar la línea del disparo
        gunLight.enabled = false; //se desactiva el componente Light para dejar de iluminar el disparo
    }
}
