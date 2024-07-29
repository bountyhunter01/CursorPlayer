using UnityEngine;

/// <summary>
/// ���� �����ϴ� ������ �ϴ� ��ũ��Ʈ�Դϴ�.
/// </summary>
public class Spawner : MonoBehaviour
{
    // ���� ��ġ �迭
    public Transform[] spawnPoints;
    // ���� ������ �迭
    public SpawnData[] spawnDatas;
    // ���� Ÿ�̸�
    private float timer;

    /// <summary>
    /// Awake�� ��ũ��Ʈ �ν��Ͻ��� �ε�� �� ȣ��˴ϴ�.
    /// </summary>
    private void Awake()
    {
        // �ڽ� ������Ʈ���� Transform ������Ʈ�� ��� �����ɴϴ�.
        spawnPoints = GetComponentsInChildren<Transform>();
    }

    /// <summary>
    /// �� �����Ӹ��� ȣ��Ǵ� Update �޼����Դϴ�.
    /// </summary>
    private void Update()
    {
        // GameManager�� Ǯ �Ŵ����� ���� ��� ������Ʈ�� �����մϴ�.
        if (GameManager.Instance == null || GameManager.Instance.pool == null)
        {
            return;
        }

        // Ÿ�̸Ӹ� ��� �ð���ŭ ������ŵ�ϴ�.
        timer += Time.deltaTime;
        // ���� ���� �ð��� ���� ������ ����մϴ�.
        int level = CalculateLevel(GameManager.Instance.gameTime);

        // Ÿ�̸Ӱ� ���� �ð����� ũ�� ���� �����մϴ�.
        if (timer > spawnDatas[level].spawnTime)
        {
            timer = 0;
            Spawn(level);
        }
    }

    /// <summary>
    /// ���� �ð��� ������� ���� ������ ����մϴ�.
    /// </summary>
    /// <param name="gameTime">���� ���� �ð�</param>
    /// <returns>���� ����</returns>
    private int CalculateLevel(float gameTime)
    {
        float levelInterval = GameManager.Instance.gameTime / (float)spawnDatas.Length;
        return Mathf.Clamp(Mathf.FloorToInt(gameTime / levelInterval), 0, spawnDatas.Length - 1);
    }

    /// <summary>
    /// �־��� ������ ���� ���� �����մϴ�.
    /// </summary>
    /// <param name="level">���� ����</param>
    private void Spawn(int level)
    {
        // �� ���� �����͸� ��ȸ�մϴ�.
        foreach (var spawnData in spawnDatas)
        {
            // ���� ������ ���� �������� �䱸 �������� ū ��쿡�� �����մϴ�.
            if (GameManager.Instance.level >= spawnData.requiredLevel)
            {
                // Ǯ �Ŵ������� �� ������Ʈ�� �����ɴϴ�.
                GameObject enemy = GameManager.Instance.pool.Get(spawnData.spriteType);
                if (enemy == null)
                {
                    continue;
                }

                // ������ ���� ��ġ�� �����մϴ�.
                Transform spawnPointTransform = spawnPoints[Random.Range(1, spawnPoints.Length)];
                enemy.transform.position = spawnPointTransform.position;

                // �� ������Ʈ�� �ʱ�ȭ�մϴ�.
                if (enemy.TryGetComponent<Enemy>(out Enemy enemyComponent))
                {
                    enemyComponent.Init(spawnData);
                }
            }
        }
    }
}

/// <summary>
/// ���� ������ Ŭ������, ������ ���� ������ ��� �ֽ��ϴ�.
/// </summary>
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
    public float damage;
    public int requiredLevel;
}
