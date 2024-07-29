using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [Tooltip("�����麯������ ���� �迭")]
    public GameObject[] prefabs;
    // Ǯ ����� �ϴ� ��ųʸ�
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
    /// ������Ʈ Ǯ���� ���� �ۺ� �޼���
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
    /// ������Ʈ�� ��Ȱ��ȭ�ϰ� ��ųʸ��� �߰��ϴ� �޼���
    /// </summary>
    /// <param name="obj">��Ȱ��ȭ�� ������Ʈ</param>
    public void ReturnToPool(GameObject obj, int index)
    {
        if (!poolDictionary.ContainsKey(index))
        {
            return;
        }
        // ������Ʈ�� ��Ȱ��ȭ�ϱ� ���� �ʿ��� �۾��� �����մϴ�.
        // ���� ���, ������Ʈ �ʱ�ȭ, �ִϸ��̼� ���� ��
        obj.SetActive(false);
        poolDictionary[index].Enqueue(obj);
    }

    /// <summary>
    /// ���� ���� ���¸� �����ϴ� �޼���
    /// </summary>
    /// <param name="gameOver">Ʈ�翩�� ����</param>
    public void SetGameOver(bool gameOver)
    {
        isGameOver = gameOver;
    }
}
