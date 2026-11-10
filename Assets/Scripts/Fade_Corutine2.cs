using System.Collections;
using UnityEngine;

public class Fade_Corutine2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private MeshRenderer meshRenderer;
    private Collider cubeCollider;

    private Color[] colors =
    {
        Color.green,
        Color.black,
        Color.red,
        Color.white,
        Color.blue
    };

    private int colorIndex = 0; // Variable para rastrear el índice del color actual
    private bool collected = false;// Variable para rastrear si el cubo ha sido recolectado

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        cubeCollider = GetComponent<Collider>();

        StartCoroutine(ChangeCubeVisibility());
    }

        public IEnumerator ChangeCubeVisibility()
    {
        while (!collected)
        {
            // El cubo desaparece durante 5 segundos
            meshRenderer.enabled = false;
            cubeCollider.enabled = false;

            yield return new WaitForSeconds(5);

            if (collected)
                yield break;

            // Cambia el color y vuelve a aparecer
            meshRenderer.material.color = colors[colorIndex];

            colorIndex = (colorIndex + 1) % colors.Length;

            meshRenderer.enabled = true;
            cubeCollider.enabled = true;

            // El cubo permanece visible durante 3 segundos
            yield return new WaitForSeconds(3f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            collected = true;

            StopAllCoroutines();

            // Desaparece al recolectarlo
            meshRenderer.enabled = false;
            cubeCollider.enabled = false;
        }
    }
}
