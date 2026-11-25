using System.Collections;
using Ilumisoft.HealthSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerDeathAndContactDamage : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField, Min(0f)] private float damagePerHit = 10f;
    [SerializeField, Min(0.1f)] private float damageInterval = 1f;
    [SerializeField, Min(0f)] private float restartDelay = 2f;
    private AudioSource audioCollected;
    private float nextDamageTime;
    private bool isEliminated;

    private void Awake()
    {
        audioCollected = GetComponent<AudioSource>();
        if (health == null) health = GetComponent<Health>();
        UpdateHealthText();
    }

    private void OnEnable()
    {
        if (health == null) health = GetComponent<Health>();
        if (health != null) health.OnHealthEmpty += HandleHealthEmpty;
    }

    private void OnDisable()
    {
        if (health != null) health.OnHealthEmpty -= HandleHealthEmpty;
    }

    private void Update()
    {
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (health != null && healthText != null)
            healthText.text = $"Vida: {Mathf.CeilToInt(health.CurrentHealth)}";
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isEliminated || health == null || Time.time < nextDamageTime) return;
        if (!HasEnemyTag(hit.collider.transform)) return;

        health.ApplyDamage(damagePerHit);
        nextDamageTime = Time.time + damageInterval;
        audioCollected.Play();
    }

    private static bool HasEnemyTag(Transform candidate)
    {
        while (candidate != null)
        {
            if (candidate.CompareTag("Enemy")) return true;
            candidate = candidate.parent;
        }
        return false;
    }

    private void HandleHealthEmpty()
    {
        if (isEliminated) return;
        isEliminated = true;

        // Keep CharacterController enabled to avoid Move calls on an inactive controller.
        foreach (MonoBehaviour behaviour in GetComponents<MonoBehaviour>())
        {
            if (behaviour != this && behaviour.GetType().Name == "ThirdPersonController")
            {
                behaviour.enabled = false;
                break;
            }
        }

        StartCoroutine(RestartScene());
    }

    private IEnumerator RestartScene()
    {
        yield return new WaitForSecondsRealtime(restartDelay);
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex >= 0) SceneManager.LoadScene(sceneIndex);
        else SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnGUI()
    {
        if (!isEliminated) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = Mathf.Clamp(Screen.height / 18, 30, 64);
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), "Fuiste eliminado", style);
    }
}