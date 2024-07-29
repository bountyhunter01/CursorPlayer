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
        // GetComponentsInChildren������ �� ��° ������ �������⿡ 1���� �ڱ� �ڽ�
        Image[] images = GetComponentsInChildren<Image>();
        if (images.Length > 1)
        {
            icon = images[1];
            icon.sprite = data.itemIcon; // �ʱ�ȭ
        }

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 0)
        {
            iconLevel = texts[0]; // �ؽ�Ʈ�� �ϳ����̶� 0��°�̰� �ʱ�ȭ�ϴ� ��
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
    /// UI�� ������Ʈ�ϴ� �޼���
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
    /// Ŭ�� �� ��������Ű�� �޼���
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

        UpdateUI(); // ������ �� UI ������Ʈ
    }
}
