using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    [Tooltip("스캔범위")]
    public float scanRange;
    [Tooltip("스캔 해당 레이어")]
    public LayerMask targetLayer;
    [Tooltip("스캔결과 배열")]
    public RaycastHit2D[] targets;
    [Tooltip("가장 가까운 적의 위치")]
    public Transform nearestTarget;

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGamePaused)
            return;//소환도 정지시키기

        // 캐스팅 시작위치, 원의 반지름, 캐스팅 방향, 캐스팅 길이, 대상 레이어
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTarget = GetNearest();
       
    }

    /// <summary>
    /// 타겟의 위치를 찾는 메서드
    /// </summary>
    /// <returns>저장된 타겟위치를 내보냄</returns>
    Transform GetNearest()
    {
        Transform result = null; // 저장되는 타겟의 위치 초기화
        float diff = float.MaxValue; // 최소한의 거리
        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos); // 타겟의 거리
            if (curDiff < diff)
            { // 거리 업데이트 조건문
                diff = curDiff;
                result = target.transform;
            }
        }
        return result;
    }
}
