using UnityEngine;
using UnityEngine.InputSystem;


public class playerMovement : MonoBehaviour
{
    [SerializeField] private playerstat playerStat; 

    Rigidbody2D rb;    
    SpriteRenderer spriteRenderer;  
    public SpriteRenderer ripple;

    [Header("MoveValue")]
    public Vector2 moveDirValue; 

    [Header("mouseposition")]
    [SerializeField] Vector3 mousePos;
    [SerializeField] Vector2 stopDashMousePos;

    [Header("bool")]
    [SerializeField] bool canDash = true;
    [SerializeField] bool isDashing = false;

    [Header("Skill CoolDown")]
    [SerializeField] float cooldown = 3;
    [SerializeField] float cdTimer;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(!GameManager.instance.isLive) return;


        mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
        float direction = mousePos.x - transform.position.x;
        stopDashMousePos = mousePos - transform.position;

        
        if (direction > 0)
        {
            spriteRenderer.flipX = true;
           if (ripple != null)
           {
                ripple.transform.localPosition = new Vector3(-0.086f, -0.335f, 0);
                ripple.flipX = true;
           }
        }
        else if (direction < 0)
        {
            spriteRenderer.flipX = false;
            if (ripple != null) 
            {
                ripple.transform.localPosition = new Vector3(0.091f, -0.335f, 0);
                ripple.flipX = false;
            }
        }


    }

    void FixedUpdate()
    {
        if(!GameManager.instance.isLive) return;

        GetComponent<Rigidbody2D>().linearVelocity = moveDirValue * playerStat.SpeedPoint.CurrentValue * Time.fixedDeltaTime;
        if (isDashing)
        {
            if(GetComponent<Rigidbody2D>().linearVelocity != Vector2.zero )
            {
                GetComponent<Rigidbody2D>().AddForce(moveDirValue.normalized * playerStat.SpeedPoint.CurrentValue, ForceMode2D.Impulse); // �̵� �� �뽬 
            }
            else
            {
                GetComponent<Rigidbody2D>().AddForce(stopDashMousePos.normalized * playerStat.SpeedPoint.CurrentValue, ForceMode2D.Impulse); // ������ ��� ���콺 ��ġ�� �̵� 
            }
            isDashing = false; 
        }
        
        cdTimer -= Time.deltaTime;  

        if (cdTimer <= 0) 
        {
            cdTimer = 0;    
            canDash = true; 
        }
    }
    void OnMove(InputValue inputvalue) 
    {
        moveDirValue = inputvalue.Get<Vector2>(); 
    }

    void OnDash(InputValue inputValue)  
    {
        if (!canDash || isDashing)
            return;

        canDash = false;
        isDashing = true;
        cdTimer = cooldown;
    }

}
