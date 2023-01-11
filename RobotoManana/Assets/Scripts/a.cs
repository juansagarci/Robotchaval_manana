using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProMove : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] Transform turret;

    // Start is called before the first frame update
    void Start()
    {
        speed = 13f;

        if (gameObject.tag == "Ray")
        {
            speed = 8f;
        }

        Invoke("Destruir", 3);
    }

    // Update is called once per frame
    void Update()
    {
        Mover();
    }

    void Mover()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destruir();
    }

    void Destruir()
    {
        Destroy(gameObject);
    }

}