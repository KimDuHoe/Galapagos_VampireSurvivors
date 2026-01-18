using UnityEngine;

public class Bullet : MonoBehaviour
{
   public float damage;
   public int per;

   Rigidbody2D rigid;

   private void Awake()
   {
      rigid = GetComponent<Rigidbody2D>();
   }

   public void Init(float damage, int per, Vector3 dir)
   {
      this.damage = damage;
      this.per = per;

      if (per > -1)
      {
         rigid.linearVelocity = dir * 8f;
      }
   }

   void OnTriggerEnter2D(Collider2D other)
   {
      if (!other.CompareTag("Enemy") || per == -1) return;

      per--;

      if (per < 0)
      {
         rigid.linearVelocity = Vector2.zero;
         gameObject.SetActive(false);
      }
   }

   private void OnTriggerExit2D(Collider2D other)
   {
      if (other.CompareTag("Area")) gameObject.SetActive(false);
   }


}
