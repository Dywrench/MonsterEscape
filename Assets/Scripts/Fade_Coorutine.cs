using UnityEngine;
using TMPro;
using System.Collections;

public class Fade_Coorutine : MonoBehaviour
{
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        StartCoroutine(StopShowingCube(GetComponent<MeshRenderer>()));
    }
    public IEnumerator StopShowingCube(MeshRenderer meshRenderer)
    {
        yield return new WaitForSeconds(10f); // espera 10 segundos y desactiva el cubo
        meshRenderer.enabled = false; // desactiva el objeto del cubo
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
}
