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

    [Header("메인 정보")]
    public ItemType itemtype;
    public int itemId;
    public string itemName;
    [Tooltip("아이템 설명"), TextArea]
    public string itemDesc;
    [Tooltip("아이템 아이콘 스프라이트")]
    public Sprite itemIcon;
    [Header("레벨 정보")]
    public float baseDamage; // 초기 데미지
    public int baseCount; // 초기 횟수
    public float[] damages;
    public int[] counts;
    [Header("무기 정보")]
    public GameObject projectile;
}
