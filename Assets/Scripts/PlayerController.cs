using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
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
    void Start()
    {
        //se obtiene el componente de la esfera
        //rb = GetComponent<Rigidbody>();
        //se obtiene el componente del sistema de particulas
        particlesSystem = particles.GetComponent<ParticleSystem>();
        //este metodo permite detener la emision de particulas al inicio del juego
        particlesSystem.Stop();
        // se inicializa el texto del contador en la UI
        textCounter.text = "Counter: " + counter.ToString(); 
        
    }
    // Update is called once per frame
    void Update()
    {
        if (controls.Player.Attack.WasPressedThisFrame())
        {   
            Animar();
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
        controls = new InputSystem_Actions();
        anim = GetComponent<Animator>();
        controls.Player.Attack.ApplyBindingOverride("<Keyboard>/leftCtrl"); // se asigna la tecla Control para el ataque
        // enlaza al input de movimiento, es para detectar cuando el jugador mueve o deja de mover el control
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }
    void OnEnable()
    {
        controls.Enable();
    }
    void OnDisable()
    {
        controls.Disable();
    }
    public IEnumerator StopParticles(ParticleSystem particleSystem)
    {
        yield return new WaitForSeconds(1f); // espera 1 segundo antes de detener el sistema de particulas
        particlesSystem.Stop();
    }

    void Animar()
    {
        
        StartCoroutine(RestartAnimation());
    }
    public IEnumerator RestartAnimation()
    {
        anim.SetBool("isSendingMagic", true);
        yield return new WaitForSecondsRealtime(1.0f); // espera 1 segundo y medio antes de reiniciar la animacion
        power.transform.position = transform.position;
        power.SendMessage("Shoot");
        anim.SetBool("isSendingMagic", false);  
    }

    void OnTriggerEnter(Collider other)
    {
          // el objeto es recolectable
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

    


}
