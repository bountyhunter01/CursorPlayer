using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;
    public int level;
    public WeaponManager weapon;

    Image icon;
    TextMeshProUGUI iconLevel;
    TextMeshProUGUI iconName;
    TextMeshProUGUI iconDesc;

    private void Awake()
    {
        // GetComponentsInChildren에서는 두 번째 값으로 가져오기에 1번이 자기 자신
        Image[] images = GetComponentsInChildren<Image>();
        if (images.Length > 1)
        {
            icon = images[1];
            icon.sprite = data.itemIcon; // 초기화
        }

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            iconLevel = texts[0]; // 텍스트는 하나뿐이라 0번째이고 초기화하는 중
            iconName = texts[1];
            iconDesc = texts[2];
            iconName.text = data.itemName;
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    /// <summary>
    /// UI를 업데이트하는 메서드
    /// </summary>
    private void UpdateUI()
    {
        iconLevel.text = "Lv." + level;
        if (level < data.damages.Length && level < data.counts.Length)
        {
            iconDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
        }
    }

    /// <summary>
    /// 클릭 시 레벨업시키는 메서드
    /// </summary>
    public void OnClick()
    {
        switch (data.itemtype)
        {
            case ItemData.ItemType.FireBall:
            case ItemData.ItemType.IceBall:
            case ItemData.ItemType.WaterBlast:
            case ItemData.ItemType.Laser:
            case ItemData.ItemType.Explosion:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<WeaponManager>();
                    weapon.Init(data);
                }
                else
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;
                    if (level < data.damages.Length)
                    {
                        nextDamage += data.damages[level];
                        nextCount += data.counts[level];
                    }

                    weapon.LevelUp(nextDamage, nextCount);
                }
                break;
        }
        level++;
        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }

        UpdateUI(); // 레벨업 후 UI 업데이트
    }
}
