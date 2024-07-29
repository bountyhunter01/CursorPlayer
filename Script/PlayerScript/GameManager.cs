using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform uiJoy;
    [Tooltip("�����ʿ� timer�� ���� ����")]
    public float gameTime;
    [Tooltip("�������� ��ȯ �ð�")]
    public float bossSpawnInterval = 60f;
    private float nextBossSpawnTime = 140f;
    [Header("���ӸŴ����� �����Ű�� ���� �ۺ� ����")]
    public PoolManager pool;
    private Animator anim;
    public PlayerMove player;
    public Enemy enemy;
    public SkillPanel uiLevelUp;
    [Header("�÷��̾� ������ �� ����ġ ����")]
    public int level;
    public int kill;
    public int exp;
    public float health;
    public float maxHealth = 100;
    [Tooltip("�������� �ʿ��� ����ġ��")]
    public int[] nextExp = { 3, 10, 60, 100, 150, 210, 280, 360, 450, 600 };
    [Tooltip("�������� ���� �ִ� ü�� ������")]
    public float[] nextMaxHealth = { 100, 105, 110, 115, 120, 125, 130, 135, 140, 145 };

    [Tooltip("���� ���� ���¸� ������ ����")]
    public bool isGameOver = false;
    [Tooltip("������ ������Ű�ų� ����ϱ� ����")]
    public bool isGamePaused = false;
    public bool bossSpawned = false; // ���� ��ȯ ���θ� üũ�� ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        Application.targetFrameRate = -1;
    }

    private void Start()
    {
        uiLevelUp = FindObjectOfType<SkillPanel>();
        anim = GetComponent<Animator>();
        health = maxHealth;
        if (player == null)
        {
            player = FindObjectOfType<PlayerMove>();
        }
        AudioManager.Instance.PlayBgm(true);
    }

    private void FixedUpdate()
    {
        if (isGamePaused)
        {
            gameTime += Time.fixedDeltaTime;

            if (gameTime >= nextBossSpawnTime)
            {
                SpawnBoss();
                nextBossSpawnTime += bossSpawnInterval;
            }
        }
    }

    public void GetExp()
    {
        exp++;
        if (exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            level++;
            exp = 0;
            uiLevelUp?.Show();

            if (level < nextMaxHealth.Length)
            {
                maxHealth = nextMaxHealth[level];
                health = maxHealth;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            isGameOver = true;
            player.Dead();
        }
    }

    private void SpawnBoss()
    {
        Vector3 spawnPosition = player.transform.position + new Vector3(Random.Range(-4f, 4f), Random.Range(-4f, 4f), 0);
        GameObject bossObj = pool.Get(3);
        if (bossObj != null)
        {
            bossObj.transform.position = spawnPosition;
            if (bossObj.TryGetComponent<Enemy>(out Enemy bossEnemy))
            {
                bossEnemy.isBoss = true;
            }
        }
        bossSpawned = true;
    }

    public void Stop()
    {
        isGamePaused = false;
        player.ResetInput();
        Time.timeScale = 0;
        uiJoy.localScale = Vector3.zero;
    }

    public void Resume()
    {
        isGamePaused = true;
        Time.timeScale = 1;
        uiJoy.localScale = new Vector3(5, 5, 5);
    }
}
