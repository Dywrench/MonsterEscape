using UnityEngine;
using System.Collections;
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
    [SerializeField] private Transform LeftHandShootPoint;
    [SerializeField] private int wavePoints = 20;
    [SerializeField] private float waveAmplitude = 0.08f;
    [SerializeField] private float waveFrequency = 5f;

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
        Vector3 location = LeftHandShootPoint.position;

        timer = 0f;
        gunLine.enabled = true;
        gunLight.enabled = true;

        shootRay.origin = location;
        shootRay.direction = transform.forward;

        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            DrawMagicBeam(location, shootHit.point);

            PlayerController playerController = Player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                GameObject particles = null;

                if (shootHit.collider.CompareTag("Enemy"))
                {
                    particles = playerController.enemyDestroyParticles;
                }
                else
                {
                    particles = playerController.objectDestroyParticles;
                }

                if (particles != null)
                {
                    GameObject effect = Instantiate(
                        particles,
                        shootHit.point,
                        Quaternion.identity
                    );

                    StartCoroutine(
                        playerController.StopDestroyParticles(effect)
                    );
                }
            }
            if (shootHit.collider.CompareTag("Enemy"))
            {
                Destroy(shootHit.collider.gameObject);
            }
            else
            {
                StartCoroutine(DestroyObjectWithDelay(shootHit.collider.gameObject));
            }
        }
        else
        {
            DrawMagicBeam(
                location,
                shootRay.origin + shootRay.direction * range
            );
        }
    }
    IEnumerator DestroyObjectWithDelay(GameObject obj)
    {
        Animator objectAnimator = obj.GetComponent<Animator>();

        if (objectAnimator != null)
        {
            objectAnimator.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        Destroy(obj);
        
    }
    
    void DrawMagicBeam(Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

        gunLine.positionCount = wavePoints;

        for (int i = 0; i < wavePoints; i++)
        {
            float t = i / (float)(wavePoints - 1);

            Vector3 position = Vector3.Lerp(start, end, t);

            float wave = Mathf.Sin(t * waveFrequency * Mathf.PI * 2f)
                        * waveAmplitude;

            position += perpendicular * wave;

            gunLine.SetPosition(i, position);
        }
    }
    void DisableEffects() //desactiva los efectos visuales del disparo
    {
        gunLine.enabled = false;  //se desactiva el componente LineRenderer para dejar de dibujar la línea del disparo
        gunLight.enabled = false; //se desactiva el componente Light para dejar de iluminar el disparo
    }
}
