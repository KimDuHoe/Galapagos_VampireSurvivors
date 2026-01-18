using UnityEngine;

public class FrameManager : MonoBehaviour
{
    [Header("FramePerSecond")]
    [SerializeField] int frame = 60;

    void Start()
    {
        QualitySettings.vSyncCount = 0; //vsync off
        Application.targetFrameRate = frame; //frame limit
    }
}
