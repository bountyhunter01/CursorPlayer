using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        FireBall, IceBall, WaterBlast, Laser, Explosion
    }

    [Header("���� ����")]
    public ItemType itemtype;
    public int itemId;
    public string itemName;
    [Tooltip("������ ����"), TextArea]
    public string itemDesc;
    [Tooltip("������ ������ ��������Ʈ")]
    public Sprite itemIcon;
    [Header("���� ����")]
    public float baseDamage; // �ʱ� ������
    public int baseCount; // �ʱ� Ƚ��
    public float[] damages;
    public int[] counts;
    [Header("���� ����")]
    public GameObject projectile;
}
