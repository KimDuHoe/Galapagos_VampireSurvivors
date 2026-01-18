using UnityEngine;
using static Status;
public class playerstat : MonoBehaviour
{
    [Header("PlayerStatus")]
    [SerializeField] Status speedPoint;
    [SerializeField] Status healthPoint;
    [SerializeField] Status defensePoint;
    public Status SpeedPoint => speedPoint;
    public Status HealthPoint => healthPoint;
    public Status DefensePoint => defensePoint;

    bool islive = true;

    void Awake()
    {
        SpeedPoint.SetValue(300.0f);
        HealthPoint.SetValue(100.0f);
        defensePoint.SetValue(30.0f);
    }

    void Update()
    {
        if (HealthPoint.CurrentValue <= 0)
        {
            islive = false;

        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Enemy"))
        {
            HealthPoint.CurrentModify((-1) * collision.gameObject.GetComponent<EnemyStats>().AttackPoint.CurrentValue);
        }
    }

    public void hp_reduce(float damage)
    {
        healthPoint.CurrentModify(-damage);

        if (healthPoint.CurrentValue > 0)
        {
            // Live, Hit Action
            //성주가 코드 주면 채워넣기 (몬스터 피격 관련 코드)
        }
        else
        {
            // .. die
            Dead();
        }
    }

    void Dead()
    {
        gameObject.SetActive(false);
        GameManager.instance.Stop();
    }
}
