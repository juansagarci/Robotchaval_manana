using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator anim;
    InputActions inputActions;

    float leftStick;
    bool saltando;
    bool rodando;
    float andando;

    [SerializeField] float fuerzaS;
    [SerializeField] float fuerzaR;

    Rigidbody2D rb;
    public float CooldownDuration = 2f;

    [SerializeField] float speedW;
    [SerializeField] float speedR;

    bool run;
    bool grounded;
    bool crouch;

    private SpriteRenderer mySpriteRenderer;
    bool derecha;

    BoxCollider2D coll;

    public void Awake()
    {
        inputActions = new InputActions();

        inputActions.Player.Move.performed += ctx =>
        {
            leftStick = ctx.ReadValue<float>();
        };
        inputActions.Player.Move.canceled += _ =>
        {
            leftStick = 0;
        };

        inputActions.Player.Run.performed += ctx =>
        {
            if (saltando)
            {
                return;
            }
            run = true;
        };
        inputActions.Player.Run.canceled += _ =>
        {
            run = false;
            anim.SetBool("run", false);
        };

        inputActions.Player.Crouch.performed += ctx =>
        {
            if (saltando == true || rodando == true)
            {
                return;

            }
            crouch = true;
            anim.SetBool("crouch", true);
            anim.SetBool("run", false);
            run = false;

            coll.offset = new Vector2(0.432f, 0.905f);
            coll.size = new Vector2(1.514f, 1.491f);

        };
        inputActions.Player.Crouch.canceled += _ =>
        {
            crouch = false;
            anim.SetBool("crouch", false);

            coll.offset = new Vector2(0.04f, 1.17f);
            coll.size = new Vector2(0.73f, 2.02f);
        };

        inputActions.Player.Jump.started += _ => Saltar();
        inputActions.Player.Roll.started += _ => Rodar();

    }

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        anim = gameObject.GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();

        rodando = false;
        saltando = false;

        fuerzaS = 75;
        fuerzaR = 55;

        run = false;
        crouch = false;

        derecha = true;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("walk", Mathf.Abs(leftStick));


        anim.SetFloat("fall", rb.velocity.y);

        speedR = -20 * (leftStick / -Mathf.Abs(leftStick));

        Move();
        Flip();

        if (rb.velocity.y != 0)
        {
            anim.SetBool("IsGrounded", false);
            grounded = false;
        }
    }

    void Saltar()
    {
        if (!saltando)
        {
            rb.AddForce(Vector3.up * fuerzaS, ForceMode2D.Impulse);
            anim.SetBool("IsGrounded", false);
            grounded = false;
            anim.SetTrigger("jump");
            saltando = true;
        }
    }

    void Rodar()
    {
        if (rodando || leftStick == 0 || grounded == false)
        {
            return;
        }
        anim.SetTrigger("roll");
        rb.AddForce(Vector3.left * fuerzaR * (leftStick / -Mathf.Abs(leftStick)), ForceMode2D.Impulse);

        StartCoroutine(CooldownR());
    }

    void Move()
    {
        if (leftStick != 0)
        {
            if (!run)
            {
                {
                    if (crouch)
                    {
                        speedW = 5 * leftStick;
                        run = false;
                        anim.SetBool("run", false);
                    }
                    transform.Translate(Vector2.right * speedW * Time.deltaTime);
                    anim.SetBool("run", false);
                    speedW = 10 * leftStick;
                }
            }

            else if (run)
            {
                transform.Translate(Vector2.right * speedR * Time.deltaTime);
                anim.SetBool("run", true);
                anim.SetBool("walk", false);
            }
        }
        else
        {
            anim.SetBool("run", false);
        }
    }

    void Flip()
    {
        if (derecha == true && leftStick < 0)
        {
            mySpriteRenderer.flipX = true;
            derecha = false;
        }

        if (derecha == false && leftStick > 0)
        {
            mySpriteRenderer.flipX = false;
            derecha = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Suelo")
        {
            saltando = false;
        }

        if (collision.gameObject.tag == "Turret" && rb.velocity.y == 0)
        {
            saltando = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Suelo")
        {
            anim.SetBool("IsGrounded", true);
            grounded = true;
        }

        if (collision.gameObject.tag == "Turret" && rb.velocity.y == 0)
        {
            anim.SetBool("IsGrounded", true);
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Suelo")
        {
            saltando = true;
        }

        if (collision.gameObject.tag == "Turret" && rb.velocity.y == 0)
        {
            saltando = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Pro")
        {
            anim.SetTrigger("die");
            inputActions.Disable();
        }

        if (collision.gameObject.tag == "Ray")
        {
            anim.SetTrigger("die");
            inputActions.Disable();
        }
    }


    public IEnumerator CooldownR()
    {
        rodando = true;
        yield return new WaitForSeconds(CooldownDuration);
        rodando = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
