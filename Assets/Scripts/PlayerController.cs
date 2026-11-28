using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using Ilumisoft.HealthSystem;
public class PlayerController : MonoBehaviour
{
    private const float HealingDuration = 3f;
    private const float HealingCooldown = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private InputSystem_Actions controls;
    public float speed;
    private Rigidbody rb;
    private Vector2 moveInput;
    public Transform particles;
    private ParticleSystem particlesSystem;
    private Vector3 position;
    private int counter;
    // variable para mostrar el contador en la UI
    private Animator anim;
    public GameObject power;
    public TextMeshProUGUI textCounter; 
    // Particulas de curación
    public GameObject healingEffect;
    public GameObject healingParticles;
    private bool isHealing;
    private float nextHealTime;
    // Partículas al destruir objetos
    public GameObject objectDestroyParticles;
    // Partículas al destruir enemigos
    public GameObject enemyDestroyParticles;
    [SerializeField] private ParticleSystem shootParticles;
    void Start()
    {
        //se obtiene el componente de la esfera
        //rb = GetComponent<Rigidbody>();
        //se obtiene el componente del sistema de particulas
        particlesSystem = particles.GetComponent<ParticleSystem>();
        //este metodo permite detener la emision de particulas al inicio del juego
        particlesSystem.Stop();
        // se inicializa el texto del contador en la UI
        healingEffect.SetActive(false);
        healingParticles.SetActive(false);

        textCounter.text = "Counter: " + counter.ToString(); 
        
    }
    // Update is called once per frame
    void Update()
    {
        if (controls.Player.Attack.WasPressedThisFrame())
        {   
            Animar();
            PlayShootParticles();
        }
        if (controls.Player.Heal.WasPressedThisFrame())
        {
            Curar();
        }
    }
    void FixedUpdate()
    {
        /*
        Vector3 movement = new Vector3(moveInput.x, 0.0f, moveInput.y); //movement input ayuda pa que se genere una fuerza en la esfera
        rb.AddForce(movement * speed);//llama metodo Addforce (añadir fuerza a la direccion del movimiento) del metodo rb (rigidbody) */
    }
        void Awake()
    {
        InitializeControls();
        anim = GetComponent<Animator>();
    }

    private void InitializeControls()
    {
        if (controls != null) return;

        controls = new InputSystem_Actions();
        controls.Player.Attack.ApplyBindingOverride("<Keyboard>/leftCtrl");
        controls.Player.Heal.ApplyBindingOverride("<Keyboard>/e");
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    void OnEnable()
    {
        InitializeControls();
        controls.Enable();
    }

    void OnDisable()
    {
        if (controls != null) controls.Disable();
    }

public IEnumerator StopParticles(ParticleSystem particleSystem)
    {
        yield return new WaitForSeconds(1f); // espera 1 segundo antes de detener el sistema de particulas
        particlesSystem.Stop();
    }
    public IEnumerator StopDestroyParticles(GameObject particleObject)
{
    yield return new WaitForSeconds(1f);

    if (particleObject != null)
    {
        ParticleSystem[] particleSystems =
            particleObject.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
}
    void Animar()
    {
        
        StartCoroutine(RestartAnimation());
    }

    void Curar()
    {
        if (!isHealing && Time.time >= nextHealTime)
            StartCoroutine(RestartHealingAnimation());
    }

        public IEnumerator RestartAnimation()
    {
        anim.SetBool("isSendingMagic", true);
        yield return new WaitForSecondsRealtime(1.0f); // espera 1 segundo y medio antes de reiniciar la animacion
        power.transform.position = transform.position;
        power.SendMessage("Shoot");
        anim.SetBool("isSendingMagic", false);  
    }
    public void PlayShootParticles()
    {
        if (shootParticles != null)
        {
            StopCoroutine(nameof(StopShootParticles));

            shootParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            shootParticles.Play();

            StartCoroutine(StopShootParticles());
        }
    }

private IEnumerator StopShootParticles()
{
    yield return new WaitForSeconds(1.3f);

    if (shootParticles != null)
    {
        shootParticles.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );
    }
}   public IEnumerator RestartHealingAnimation()
    {
        if (isHealing || Time.time < nextHealTime)
            yield break;

        Health health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError(
                "PlayerController needs a Health component to heal.",
                this
            );
            yield break;
        }

        if (!health.IsAlive)
            yield break;

        nextHealTime = Time.time + HealingCooldown;
        isHealing = true;
        anim.SetBool("isHealing", true);

        healingEffect.SetActive(true);
        healingParticles.SetActive(true);

        float elapsed = 0f;
        while (elapsed < HealingDuration && health.IsAlive)
        {
            float deltaTime = Time.unscaledDeltaTime;
            float remainingDuration = HealingDuration - elapsed;
            float missingHealth = health.MaxHealth - health.CurrentHealth;

            health.AddHealth(missingHealth * (deltaTime / remainingDuration));
            elapsed += deltaTime;
            yield return null;
        }

        if (health.IsAlive)
            health.SetHealth(health.MaxHealth);

        healingEffect.SetActive(false);
        healingParticles.SetActive(false);

        anim.SetBool("isHealing", false);
        isHealing = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (HasTagInHierarchy(other.transform, "Lava"))
        {
            Health health = GetComponent<Health>();
            if (health == null)
            {
                Debug.LogError(
                    "PlayerController needs a Health component to handle lava.",
                    this
                );
                return;
            }

            if (health.IsAlive)
                health.ApplyDamage(health.CurrentHealth);
            return;
        }

        if (other.gameObject.CompareTag("Collectable"))
        {
            // obtiene la posicion del objeto recolectable
            position = other.gameObject.transform.position; 
            particles.position = position; // se asigna la posicion del objeto recolectable a la posicion del sistema de particulas
            particlesSystem = particles.GetComponent<ParticleSystem>(); // se obtiene el componente del sistema de particulas
            particlesSystem.Play(); // se activa el sistema de particulas
            counter++; // incrementa el contador de objetos recolectados
            textCounter.text = "Counter: " + counter.ToString(); // actualiza el texto del contador en la UI
            // desactiva el objeto recolectable (lo oculta de la escena)  
            other.gameObject.SetActive(false);
            StartCoroutine(StopParticles(particlesSystem)); 
        }
        else // el objeto no es recolectable
        {
           
        }   
    }
    

    private static bool HasTagInHierarchy(Transform candidate, string tag)
    {
        while (candidate != null)
        {
            if (candidate.CompareTag(tag)) return true;
            candidate = candidate.parent;
        }

        return false;
    }

    


}
