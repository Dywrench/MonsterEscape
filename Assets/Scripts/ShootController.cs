using UnityEngine;
using System.Collections;
using TMPro;
public class ShootController : MonoBehaviour
{
    private AudioSource[] audioCollected;
    public TextMeshProUGUI textCounter;
    public TextMeshProUGUI textVaseCounter;
    private int vaseCounter; 
    private int counter;
    public GameObject Player; //El objeto que representa al jugador en la escena
    public float TimeBetweenShooting = 1f;// Tiempo entre disparos
    public float range = 100f; // Rango del disparo
    public GameObject door;

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
        audioCollected = GetComponents<AudioSource>();
        shootableMask = LayerMask.GetMask("Shootable"); // Se obtiene la máscara de capa para los objetos disparables
        gunLine = GetComponent<LineRenderer>(); // Se obtiene el componente LineRenderer del objeto
        gunLight = GetComponent<Light>(); // Se obtiene el componente Light del objeto
        if (controls == null){
        controls = new InputSystem_Actions();
        }
        if (textCounter != null)
            textCounter.text = "Dragones Eliminados: " + counter;
        if (textVaseCounter != null)
            textVaseCounter.text = "Jarrones Destruidos: " + vaseCounter;
    }

    void OnEnable()
    {
        if (controls == null) controls = new InputSystem_Actions();
        controls.Enable();
    }

    void OnDisable()
    {
        if (controls != null) controls.Disable();
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
    if (audioCollected != null && audioCollected.Length > 0)
        audioCollected[0].Play();
    Vector3 location = LeftHandShootPoint != null ? LeftHandShootPoint.position : transform.position;

    timer = 0f;
    if (gunLine != null) gunLine.enabled = true;
    if (gunLight != null) gunLight.enabled = true;

    shootRay.origin = location;
    shootRay.direction = transform.forward;

    if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
    {
        DrawMagicBeam(location, shootHit.point);

        PlayerController playerController = Player != null ? Player.GetComponent<PlayerController>() : null;

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

        // Verificar si el objeto es un dragón o un jarrón
        if (shootHit.collider.CompareTag("Enemy") ||
            shootHit.collider.CompareTag("vase"))
        {
            ObjectResistance objectResistance =
                shootHit.collider.GetComponent<ObjectResistance>();

            if (objectResistance == null)
            {
                objectResistance =
                    shootHit.collider.GetComponentInParent<ObjectResistance>();
            }

            if (objectResistance != null)
            {
                objectResistance.ReceiveHit(shootHit.collider.tag, this);
            }
            else
            {
                StartCoroutine(DestroyObjectWithDelay(shootHit.collider.gameObject, shootHit.collider.tag));
            }
        }
        else
        {
            StartCoroutine(DestroyObjectWithDelay(shootHit.collider.gameObject, shootHit.collider.tag));
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

public void RegisterDestroyedObject(string objectTag)
{
    if (objectTag == "Enemy")
    {
        counter++;
        if (textCounter != null)
            textCounter.text = "Dragones Eliminados: " + counter;
        if (audioCollected != null && audioCollected.Length > 1)
            audioCollected[1].Play();
    }
    else if (objectTag == "vase")
    {
        vaseCounter++;
        if (textVaseCounter != null)
            textVaseCounter.text = "Jarrones Destruidos: " + vaseCounter;

        if (vaseCounter >= 5 && door != null){
            door.SetActive(false);
        }
    }
}

    IEnumerator DestroyObjectWithDelay(GameObject obj, string objectTag)
    {
        Animator objectAnimator = obj.GetComponent<Animator>();

        if (objectAnimator != null)
        {
            objectAnimator.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        RegisterDestroyedObject(objectTag);
        Destroy(obj);
        
    }
    
    void DrawMagicBeam(Vector3 start, Vector3 end)
    {
        if (gunLine == null) return;
        if (wavePoints < 2) wavePoints = 2;

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
        if (gunLine != null) gunLine.enabled = false;  //se desactiva el componente LineRenderer para dejar de dibujar la línea del disparo
        if (gunLight != null) gunLight.enabled = false; //se desactiva el componente Light para dejar de iluminar el disparo
    }
}
