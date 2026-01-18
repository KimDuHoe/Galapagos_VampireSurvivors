using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Object")]
    public PoolManager pool;
    public playerMovement player;
    public LevelUP uiLevelUp;
    public Weapon Meele;
    public Weapon Range;

    [Header("# Player Info")]
    public int level;
    public int kill;
    public int exp;
    public float health;
    public float maxHealth = 100;
    public bool isLive;
    public int[] nextExp = { 3, 5, 10, 100, 150, 210, 280, 360, 450, 600 };

    [Header("# Game Time")]
    public float gametime;
    public float realgametime;

    void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        health = maxHealth;
        isLive = true;
        uiLevelUp.Select(0);
    }

    private void Update()
    {
        if (!isLive) return;

        gametime += Time.deltaTime;
        if (gametime > realgametime)
        {
            gametime = 0f;
            realgametime++;
        }
    }

    public void GetExp()
    {
        if (!isLive) return;

        exp++;

        if (exp == nextExp[Math.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            // UI 잠시만 비활성화 (테스트 할 때 다시 사용)
            // if (uiLevelUp != null) uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void ResumeTime()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}