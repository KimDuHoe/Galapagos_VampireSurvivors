using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Weapon : MonoBehaviour
{
    playerMovement player;

    [Header("Weapon Id")]
    public int id; //Weapon's unique id
    public int prefabId; //prefab ID to load the weapon
    public float damage; //Attack damage
    public float speed; //Attack cooltime
    float timer; //Attack cooltime checking timer


    [Header("Only Range Variable")]
    public int count; //Number of projectiles fired at a time
    public int consecutive_count; // Number of consecutive projectile launches
    public float consecutiveInterval = 0.1f; // Launch interval timer
    public int penetrating_power; //Attack penetrating power
    bool isBursting; //Variables for whether you are firing

    [Header("Only Meele Variable")]
    public Vector2 range = new Vector2(1f, 1f); // Range of attack
    public RaycastHit2D[] targets; //scaner to monster in world
    public Transform attackPoint; //attack reference point

    [Header("condition for weapon")]
    public bool isbothhands = false;



    [Header("Base Setting")]
    public LayerMask targetLayer; //Tag only monsters to target



    //Mouse Pointer Direction
    Vector3 mouseScreenPos; //mouse coordinate in screen
    Vector3 mouseWorldPos; //Player coordinate in world
    Vector3 dir; // Vector for the direction the player points to

    Vector3 flip_postion;
    Vector3 flip_postion_reverse;

    SpriteRenderer sprite;
    public SpriteRenderer rightHand;
    public SpriteRenderer leftHand;

    private void Start()
    {
        player = GameManager.instance.player;
        sprite = GetComponent<SpriteRenderer>();

        switch (id) //Weapon flip postion
        {
            case 0: //Meele Weapon
                flip_postion = new Vector3(-0.126f, -0.1f, 0f);
                flip_postion_reverse = new Vector3(0.1f, -0.1f, 0f);
                break;
            case 1: //Range Weapon
                flip_postion = new Vector3(-0.193f, -0.1f, 0f);
                flip_postion_reverse = new Vector3(0.17f, -0.1f, 0f);
                break;
        }
    }

    void Update()
    {
        //if game stop, don't move the weapon
        if (!GameManager.instance.isLive) return;

        Repostion_weapon();

        switch (id)
        {
            case 0:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    Melee_attack();
                    timer = 0;
                }

                break;
            case 1:
                timer += Time.deltaTime;

                if (!isBursting && timer > speed)
                {
                    StartCoroutine(BurstFire());
                    timer = 0;
                }
                break;

            default:
                break;
        }

        //.. Test code..
        if (Input.GetButtonDown("Jump"))
        {
            GameManager.instance.GetExp();
        }

    }

    void OnSwap_Weapon(InputValue inputvalue)
    {
        if (isbothhands) return;


        switch (id) //Weapon id 0 : Meele / id 1 : Range
        {
            case 0: //Meele Weapon
                GameManager.instance.Range.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
                break;
            case 1: //Range Weapon
                GameManager.instance.Meele.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
                break;
        }
    }

    public void Levelup(float damage, int count, float speed, int penetrating_power)
    {
        this.damage = damage;
        this.count = count;
        this.speed = speed;
        this.penetrating_power = penetrating_power;
        //if(id == 0) Batch();
    }

    public void Init(ItemData data)
    {
        // Basic Set
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // Property set
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;
        penetrating_power = data.basePenetrating_power;
        range = data.baseRange;
        targetLayer = LayerMask.GetMask("Enemy");

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 1f;
                Batch();
                break;
            case 1:
                speed = 1f;
                break;
            default:
                break;
        }
    }

    void Batch()
    {
        for (int i = 0; i < 1; i++)
        {
            Transform bullet;

            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform;
            }


            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // Vector3 rotVec = Vector3.forward * 360 * i / count;
            // bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero); // -1 is Infinity Per.
            attackPoint = bullet.transform;
        }
    }

    void Melee_attack()
    {
        // attackPoint가 없으면 무기의 위치를 사용
        Vector3 attackPosition = attackPoint != null ? attackPoint.position : transform.position;


        // BoxCast의 크기를 Vector2로 사용하고, 무기의 회전 각도를 float로 변환
        float angle = transform.rotation.eulerAngles.z;
        targets = Physics2D.BoxCastAll(attackPosition, range, angle, Vector2.zero, 0, targetLayer);

        // 감지된 적들에게 피해를 줌
        foreach (RaycastHit2D hit in targets)
        {
            if (!hit.collider.CompareTag("Enemy")) continue;

            // 각 Enemy 스크립트의 OnHit 함수를 호출하여 넉백 및 효과를 적용
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.OnHit(damage);
                continue; // 다음 타겟으로
            }

            BombCrab bombCrab = hit.collider.GetComponent<BombCrab>();
            if (bombCrab != null)
            {
                bombCrab.OnHit(damage);
                continue; // 다음 타겟으로
            }

            DrillCrab drillCrab = hit.collider.GetComponent<DrillCrab>();
            if (drillCrab != null)
            {
                drillCrab.OnHit(damage);
                continue; // 다음 타겟으로
            }

            // 위에서 OnHit을 호출하지 못한 경우 (예: 스크립트가 없는 경우), 기존 방식대로 체력만 감소
            EnemyStats enemyStats = hit.collider.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.hp_reduce(damage);
            }
        }
    }

    void OnDrawGizmos()
    {
        // BoxCast의 범위를 시각적으로 표시
        Gizmos.color = Color.red;
        float angle = transform.rotation.eulerAngles.z;

        // attackPoint가 있으면 해당 위치에, 없으면 무기 위치에 그립니다
        Vector3 drawPosition = attackPoint != null ? attackPoint.position : transform.position;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(drawPosition, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(range.x, range.y, 0.1f));
    }

    void Fire()
    {
        Vector3 baseDir = dir.normalized;
        int n = Mathf.Max(1, count);

        // 반각(half spread): 2발은 ±30°, 3발 이상은 점진적으로 넓혀 최대 ±60°까지
        // 반각을 수정하여 탄 퍼짐 각도를 수정
        float halfSpread = 5f + 5f * (n - 2); // n=2 -> 30, n=3 -> 45
        halfSpread = Mathf.Clamp(halfSpread, 0f, 60f);

        float step = n > 1 ? (halfSpread * 2f) / (n - 1) : 0f;

        for (int i = 0; i < n; i++)
        {
            float offsetDeg;
            if ((n & 1) == 1)
            {
                // 홀수: 중앙 포함
                offsetDeg = (i - (n - 1) * 0.5f) * step;
            }
            else
            {
                // 짝수: 중앙 비우고 좌우 대칭
                offsetDeg = (i - (n / 2 - 0.5f)) * step;
            }

            Vector3 shotDir = (Quaternion.Euler(0, 0, offsetDeg) * baseDir).normalized;

            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, shotDir);
            bullet.GetComponent<Bullet>().Init(damage, penetrating_power, shotDir);
        }
    }

    IEnumerator BurstFire()
    {
        isBursting = true;

        int shots = Mathf.Max(1, consecutive_count);
        for (int i = 0; i < shots; i++)
        {
            Fire();
            if (i < shots - 1) yield return new WaitForSeconds(consecutiveInterval);
        }

        timer = 0f;
        isBursting = false;
    }

    void Repostion_weapon()
    {
        //Point the weapon to the mouse pointer
        mouseScreenPos = Input.mousePosition;
        mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f; // if 2D, z'value(coordinates) fix

        dir = mouseWorldPos - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

        //If the mouse goes over half the screen
        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
        float mouseDirectionFromCenter = mouseWorldPos.x - screenCenter.x;


        if (sprite != null) //Flip weapon for repostion
        {
            if (mouseDirectionFromCenter > 0)
            {
                sprite.flipX = true;
                rightHand.flipY = true;
                leftHand.flipY = true;
                transform.localPosition = flip_postion;
            }
            else if (mouseDirectionFromCenter < 0)
            {
                sprite.flipX = false;
                rightHand.flipY = false;
                leftHand.flipY = false;
                transform.localPosition = flip_postion_reverse;
            }
        }
    }
}
