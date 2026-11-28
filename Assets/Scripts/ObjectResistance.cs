using UnityEngine;
using System.Collections;

public class ObjectResistance : MonoBehaviour
{
    private int resistance;
    private int currentHits;
    private bool isDestroyed;

    void Awake()
    {
        resistance = Random.Range(2, 6);
        currentHits = 0;

       // Debug.Log(gameObject.name + " necesita " + resistance + " impactos.");
    }

    public void ReceiveHit(string objectTag, ShootController shootController)
    {
        if (isDestroyed) return;

        currentHits++;


        if (currentHits >= resistance)
        {
            isDestroyed = true;
            StartCoroutine(DestroyObject(objectTag, shootController));
        }
    }

    IEnumerator DestroyObject(string objectTag, ShootController shootController)
    {
        Animator objectAnimator = GetComponent<Animator>();

        if (objectAnimator != null)
        {
            objectAnimator.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        if (shootController != null)
        {
            shootController.CompleteObjectHit(gameObject, objectTag);
        }
        else if (objectTag == "vase")
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer != null)
                meshRenderer.enabled = false;

            foreach (Collider objectCollider in GetComponents<Collider>())
                objectCollider.enabled = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
