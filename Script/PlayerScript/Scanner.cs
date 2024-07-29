using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Tooltip("��ĵ����")]
    public float scanRange;
    [Tooltip("��ĵ �ش� ���̾�")]
    public LayerMask targetLayer;
    [Tooltip("��ĵ��� �迭")]
    public RaycastHit2D[] targets;
    [Tooltip("���� ����� ���� ��ġ")]
    public Transform nearestTarget;

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGamePaused)
            return;//��ȯ�� ������Ű��

        // ĳ���� ������ġ, ���� ������, ĳ���� ����, ĳ���� ����, ��� ���̾�
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
       
    }

    /// <summary>
    /// Ÿ���� ��ġ�� ã�� �޼���
    /// </summary>
    /// <returns>����� Ÿ����ġ�� ������</returns>
    Transform GetNearest()
    {
        Transform result = null; // ����Ǵ� Ÿ���� ��ġ �ʱ�ȭ
        float diff = float.MaxValue; // �ּ����� �Ÿ�
        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos); // Ÿ���� �Ÿ�
            if (curDiff < diff)
            { // �Ÿ� ������Ʈ ���ǹ�
                diff = curDiff;
                result = target.transform;
            }
        }
        return result;
    }
}
