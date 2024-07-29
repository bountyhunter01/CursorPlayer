using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHealth : MonoBehaviour
{
    RectTransform rectTransform;

    // �����̴��� �÷��̾� �Ʒ��� ��ġ��Ű�� ���� ������
    public Vector3 offset = new Vector3(0, -70, 0); // Y ���� �����Ͽ� ���ϴ� ��ġ�� ����

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        // �������� �����Ͽ� �����̴��� �÷��̾� �Ʒ��� ��ġ��Ŵ
        rectTransform.position
        = Camera.main.WorldToScreenPoint(GameManager.Instance.player.transform.position) + offset;
    }
}
