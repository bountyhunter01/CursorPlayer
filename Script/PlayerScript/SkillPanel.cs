using UnityEngine;

public class SkillPanel : MonoBehaviour
{
    RectTransform rect;
    Item[] items; // items 배열 추가

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
    /// 스킬창이 랜덤하게 뜨도록 설정하기
    /// </summary>
    void Next()
    {
        // 모든 스킬 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // 그중에서 랜덤 3개 활성화
        int[] random = new int[3];
        while (true)
        {
            random[0] = Random.Range(0, items.Length);
            random[1] = Random.Range(0, items.Length);
            random[2] = Random.Range(0, items.Length);

            if (random[0] != random[1] && random[1] != random[2] && random[0] != random[2])
            {
                // while문 빠져나가는 조건
                break;
            }
        }

        for (int index = 0; index < random.Length; index++)
        {
            Item ranItem = items[random[index]];
            // 만렙 아이템은 비활성화하고 빈자리를 다른 아이템으로 채움
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

                // 모든 스킬이 만렙인 경우를 대비한 예외 처리
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

        // 추가적인 빈 자리를 채우기 위한 로직
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
