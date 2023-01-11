using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstDispRay : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] Transform robot;
    [SerializeField] GameObject proyectil;
    float alcance;
    float intervalo;


    // Start is called before the first frame update
    void Start()
    {
        intervalo = 0.3f;
        alcance = 10;
    }

    // Update is called once per frame
    void Update()
    {
        Detector();
    }

    void Detector()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, alcance);
        if (hit.collider.gameObject.tag == "Player")
        {
            StartCoroutine("Iniciar");
            anim.SetBool("Shoot", true);
            anim.SetBool("Idle", false);
        }

        else
        {
            StopCoroutine("Iniciar");
            anim.SetBool("Shoot", false);
            anim.SetBool("Idle", true);
        }

    }

    IEnumerator Iniciar()
    {
        while (true)
        {

            Instantiate(proyectil, transform.position, transform.rotation);

            yield return new WaitForSeconds(intervalo);

        }
    }
}