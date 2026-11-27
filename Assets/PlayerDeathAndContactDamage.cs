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
    private CharacterController characterController;
    private StarterAssets.ThirdPersonController thirdPersonController;
    private readonly Collider[] nearbyColliders = new Collider[16];
    private float nextDamageTime;
    private bool isEliminated;

    private void Awake()
    {
        audioCollected = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        thirdPersonController = GetComponent<StarterAssets.ThirdPersonController>();
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
        DamageOverlappingEnemy();
    }

    private void UpdateHealthText()
    {
        if (health != null && healthText != null)
            healthText.text = $"Vida: {Mathf.CeilToInt(health.CurrentHealth)}";
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TrackMovingPlatform(hit);
        TryApplyContactDamage(hit.collider.transform);
    }

    private void DamageOverlappingEnemy()
    {
        if (isEliminated || health == null || characterController == null ||
            Time.time < nextDamageTime)
            return;

        Bounds bounds = characterController.bounds;
        float radius = Mathf.Min(bounds.extents.x, bounds.extents.z) +
            characterController.skinWidth;
        float halfSegment = Mathf.Max(0f, bounds.extents.y - radius);
        Vector3 segmentOffset = Vector3.up * halfSegment;
        int colliderCount = Physics.OverlapCapsuleNonAlloc(
            bounds.center - segmentOffset,
            bounds.center + segmentOffset,
            radius,
            nearbyColliders,
            Physics.AllLayers,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < colliderCount; i++)
        {
            if (nearbyColliders[i] != null)
                TryApplyContactDamage(nearbyColliders[i].transform);

            if (Time.time < nextDamageTime)
                return;
        }
    }

    private void TryApplyContactDamage(Transform candidate)
    {
        if (isEliminated || health == null || Time.time < nextDamageTime) return;
        if (!HasEnemyTag(candidate)) return;

        health.ApplyDamage(damagePerHit);
        nextDamageTime = Time.time + damageInterval;
        if (audioCollected != null) audioCollected.Play();
    }

    private void TrackMovingPlatform(ControllerColliderHit hit)
    {
        if (thirdPersonController == null || hit.normal.y < 0.5f) return;

        PlatformMovementX platform =
            hit.collider.GetComponentInParent<PlatformMovementX>();

        thirdPersonController.SetMovingPlatform(
            platform != null ? platform.transform : null
        );
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