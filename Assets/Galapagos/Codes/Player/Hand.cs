using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;
    public SpriteRenderer spriter;

    SpriteRenderer player;

    Vector3 rightPos = new Vector3(0.7f, -0.2f, 0);
    Vector3 rightPosReverse = new Vector3(-0.1f, -0.2f, 0);
    Quaternion leftRot = Quaternion.Euler(0, 0,0);
    Quaternion leftRotRerverse = Quaternion.Euler(0, 0, 0);
    private void Awake() 
    {
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }

    private void LateUpdate()
    {
        bool isReverse = player.flipX;

        if(isLeft) { //왼손에 있는 무기
            transform.localRotation = isReverse ? leftRotRerverse : leftRot;
            spriter.flipY = isReverse;
            spriter.sortingOrder = isReverse ? 1 : 1;
        }
        else //오른손에 있는 무기
        {
            transform.localPosition = isReverse ? rightPosReverse : rightPos;
            spriter.sortingOrder = isReverse ? 1 : 1;
        }
    }
}
