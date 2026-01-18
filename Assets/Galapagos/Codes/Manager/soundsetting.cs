using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class soundsettings : MonoBehaviour
{
    public Slider soundSlider;        // 사운드 슬라이더
    public Image soundIcon;           // 사운드 아이콘 이미지
    public Sprite soundOnSprite;      // 소리 켜짐 이미지
    public Sprite soundOffSprite;     // 소리 꺼짐 이미지
    public Sprite soundonnSprite;     // 소리 많이커짐 이미지

    private void Start()
    {
        if (soundSlider == null)
        {
            return;
        }

        // 슬라이더 값이 변경될 때마다 OnSliderValueChanged 호출
        soundSlider.onValueChanged.AddListener(OnSliderValueChanged);

        // 시작할 때 한 번 초기 이미지 설정
        OnSliderValueChanged(soundSlider.value);
    }

    private void OnSliderValueChanged(float value)
    {
        if (value <= 0f)
        {
            soundIcon.sprite = soundOffSprite;   // 값이 0이면 음소거 아이콘으로 변경
        }
        else if(value <=0.5f)
        {
            soundIcon.sprite = soundOnSprite;    // 값이 0보다 크고 0.5보다 작으면 소리 켜짐 아이콘으로 변경
        }
        else
        {
            soundIcon.sprite = soundonnSprite;   //값이 0.5보다 크면 소리 많이 커짐 아이콘으로 변경
        }
    }
}