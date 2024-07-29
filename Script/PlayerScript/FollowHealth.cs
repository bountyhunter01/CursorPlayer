using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHealth : MonoBehaviour
{
    RectTransform rectTransform;

    // 슬라이더를 플레이어 아래에 위치시키기 위한 오프셋
    public Vector3 offset = new Vector3(0, -70, 0); // Y 값을 조정하여 원하는 위치로 설정

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        // 오프셋을 적용하여 슬라이더를 플레이어 아래에 위치시킴
        rectTransform.position
        = Camera.main.WorldToScreenPoint(GameManager.Instance.player.transform.position) + offset;
    }
}
