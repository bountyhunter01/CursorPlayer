using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    RectTransform rect;
    Item[] items; // items �迭 �߰�

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        rect.localScale = new Vector3(9.3f, 4, 1);
        GameManager.Instance.Stop();
        Next();
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Heal);
        AudioManager.Instance.EffectBgm(true);
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.Instance.Resume();
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.ClickButton);
        AudioManager.Instance.EffectBgm(false);
    }

    /// <summary>
    /// ��ųâ�� �����ϰ� �ߵ��� �����ϱ�
    /// </summary>
    void Next()
    {
        // ��� ��ų ��Ȱ��ȭ
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // ���߿��� ���� 3�� Ȱ��ȭ
        int[] random = new int[3];
        while (true)
        {
            random[0] = Random.Range(0, items.Length);
            random[1] = Random.Range(0, items.Length);
            random[2] = Random.Range(0, items.Length);

            if (random[0] != random[1] && random[1] != random[2] && random[0] != random[2])
            {
                // while�� ���������� ����
                break;
            }
        }

        for (int index = 0; index < random.Length; index++)
        {
            Item ranItem = items[random[index]];
            // ���� �������� ��Ȱ��ȭ�ϰ� ���ڸ��� �ٸ� ���������� ä��
            if (ranItem.level == ranItem.data.damages.Length)
            {
                bool foundReplacement = false;
                foreach (Item item in items)
                {
                    if (!item.gameObject.activeSelf && item.level < item.data.damages.Length)
                    {
                        item.gameObject.SetActive(true);
                        foundReplacement = true;
                        break;
                    }
                }

                // ��� ��ų�� ������ ��츦 ����� ���� ó��
                if (!foundReplacement)
                {
                    items[Random.Range(0, items.Length)].gameObject.SetActive(true);
                }
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
        }

        // �߰����� �� �ڸ��� ä��� ���� ����
        int activeCount = 0;
        foreach (Item item in items)
        {
            if (item.gameObject.activeSelf)
            {
                activeCount++;
            }
        }

        while (activeCount < 3)
        {
            foreach (Item item in items)
            {
                if (!item.gameObject.activeSelf && item.level < item.data.damages.Length)
                {
                    item.gameObject.SetActive(true);
                    activeCount++;
                    if (activeCount >= 3)
                    {
                        break;
                    }
                }
            }
        }
    }
}
