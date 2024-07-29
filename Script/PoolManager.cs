using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Tooltip("프리펩변수들을 담은 배열")]
    public GameObject[] prefabs;
    // 풀 담당을 하는 딕셔너리
    private Dictionary<int, Queue<GameObject>> poolDictionary;
    private Dictionary<int, GameObject> prefabDictionary;

    private bool isGameOver = false;

    private void Awake()
    {
        poolDictionary = new Dictionary<int, Queue<GameObject>>();
        prefabDictionary = new Dictionary<int, GameObject>();

        for (int index = 0; index < prefabs.Length; index++)
        {
            prefabDictionary[index] = prefabs[index];
            poolDictionary[index] = new Queue<GameObject>();
        }
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.isGamePaused)
        {
            return;
        }
    }

    /// <summary>
    /// 오브젝트 풀링을 위한 퍼블릭 메서드
    /// </summary>
    /// <returns></returns>
    public GameObject Get(int index)
    {
        if (isGameOver || !poolDictionary.ContainsKey(index))
        {
            return null;
        }

        if (poolDictionary[index].Count > 0)
        {
            GameObject select = poolDictionary[index].Dequeue();
            select.SetActive(true);
            return select;
        }

        GameObject newObject = Instantiate(prefabDictionary[index]);
        return newObject;
    }

    /// <summary>
    /// 오브젝트를 비활성화하고 딕셔너리에 추가하는 메서드
    /// </summary>
    /// <param name="obj">비활성화할 오브젝트</param>
    public void ReturnToPool(GameObject obj, int index)
    {
        if (!poolDictionary.ContainsKey(index))
        {
            return;
        }
        // 오브젝트를 비활성화하기 전에 필요한 작업을 수행합니다.
        // 예를 들어, 컴포넌트 초기화, 애니메이션 정지 등
        obj.SetActive(false);
        poolDictionary[index].Enqueue(obj);
    }

    /// <summary>
    /// 게임 오버 상태를 설정하는 메서드
    /// </summary>
    /// <param name="gameOver">트루여야 멈춤</param>
    public void SetGameOver(bool gameOver)
    {
        isGameOver = gameOver;
    }
}
