using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Hand[] hands;

    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator animator;


   
    void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        hands = GetComponentsInChildren<Hand>(true);
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }
    void FixedUpdate()
    {
        if(!GameManager.instance.isLive)
            return;

        Vector2 nextVec = inputVec.normalized * speed * Time.deltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if(!GameManager.instance.isLive)
            return;

        animator.SetFloat("speed", inputVec.magnitude);

        if(inputVec.x != 0)
        {
            sprite.flipX = inputVec.x > 0;
        }
    }
}
