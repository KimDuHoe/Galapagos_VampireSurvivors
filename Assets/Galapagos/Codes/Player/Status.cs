using UnityEngine;
[System.Serializable]
public class Status
{
    [Header("base&cur Value")]
    [SerializeField] float baseValue;
    [SerializeField] float currentValue;

    public float BaseValue { get { return baseValue; } } // same public float BaseValue => baseValue;
    public float CurrentValue { get { return currentValue; } } // same public float currentvalue => currentValue;


    public void SetValue(float value) // 스텟 초기 설정
    {
        baseValue = value;
        currentValue = value;
    }

    public void CurrentModify(float value) // 현재값 변경 (데미지로 인한 체력 감소 등..)
    {
        currentValue += value;
    }
    public void BaseModify(float value) // 최댓값 변경 (최대체력 변경 등..)
    {
        baseValue += value;
    }

    public void CurrentReset() // 현재스텟 초기화
    {
        currentValue = baseValue;
    }
}
