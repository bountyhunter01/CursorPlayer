using UnityEngine;

/// <summary>
/// 적을 스폰하는 역할을 하는 스크립트입니다.
/// </summary>
public class Spawner : MonoBehaviour
{
    // 스폰 위치 배열
    public Transform[] spawnPoints;
    // 스폰 데이터 배열
    public SpawnData[] spawnDatas;
    // 스폰 타이머
    private float timer;

    /// <summary>
    /// Awake는 스크립트 인스턴스가 로드될 때 호출됩니다.
    /// </summary>
    private void Awake()
    {
        // 자식 오브젝트에서 Transform 컴포넌트를 모두 가져옵니다.
        spawnPoints = GetComponentsInChildren<Transform>();
    }

    /// <summary>
    /// 매 프레임마다 호출되는 Update 메서드입니다.
    /// </summary>
    private void Update()
    {
        // GameManager나 풀 매니저가 없는 경우 업데이트를 중지합니다.
        if (GameManager.Instance == null || GameManager.Instance.pool == null)
        {
            return;
        }

        // 타이머를 경과 시간만큼 증가시킵니다.
        timer += Time.deltaTime;
        // 현재 게임 시간에 따른 레벨을 계산합니다.
        int level = CalculateLevel(GameManager.Instance.gameTime);

        // 타이머가 스폰 시간보다 크면 적을 스폰합니다.
        if (timer > spawnDatas[level].spawnTime)
        {
            timer = 0;
            Spawn(level);
        }
    }

    /// <summary>
    /// 게임 시간을 기반으로 현재 레벨을 계산합니다.
    /// </summary>
    /// <param name="gameTime">게임 진행 시간</param>
    /// <returns>현재 레벨</returns>
    private int CalculateLevel(float gameTime)
    {
        float levelInterval = GameManager.Instance.gameTime / (float)spawnDatas.Length;
        return Mathf.Clamp(Mathf.FloorToInt(gameTime / levelInterval), 0, spawnDatas.Length - 1);
    }

    /// <summary>
    /// 주어진 레벨에 따라 적을 스폰합니다.
    /// </summary>
    /// <param name="level">현재 레벨</param>
    private void Spawn(int level)
    {
        // 각 스폰 데이터를 순회합니다.
        foreach (var spawnData in spawnDatas)
        {
            // 현재 레벨이 스폰 데이터의 요구 레벨보다 큰 경우에만 스폰합니다.
            if (GameManager.Instance.level >= spawnData.requiredLevel)
            {
                // 풀 매니저에서 적 오브젝트를 가져옵니다.
                GameObject enemy = GameManager.Instance.pool.Get(spawnData.spriteType);
                if (enemy == null)
                {
                    continue;
                }

                // 무작위 스폰 위치를 선택합니다.
                Transform spawnPointTransform = spawnPoints[Random.Range(1, spawnPoints.Length)];
                enemy.transform.position = spawnPointTransform.position;

                // 적 컴포넌트를 초기화합니다.
                if (enemy.TryGetComponent<Enemy>(out Enemy enemyComponent))
                {
                    enemyComponent.Init(spawnData);
                }
            }
        }
    }
}

/// <summary>
/// 스폰 데이터 클래스로, 스폰될 적의 정보를 담고 있습니다.
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
