using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

public class HUD : MonoBehaviour
{
    public enum InfoType
    {
        Exp, Level, Kill, Time, Health
    }
    public InfoType type;

    TextMeshProUGUI myText;
    Slider mySlider;

    private void Awake()
    {
        myText = GetComponent<TextMeshProUGUI>();
        mySlider = GetComponent<Slider>();
    }

    private void LateUpdate()
    {
        switch (type)
        {
            case InfoType.Exp:
                float curExp = GameManager.Instance.exp;
                float maxExp = GameManager.Instance.nextExp[Mathf.Min(GameManager.Instance.level, GameManager.Instance.nextExp.Length - 1) ];
                mySlider.value = curExp / maxExp;
                break;
            case InfoType.Level:
                myText.text = string.Format("Lv.{0:F0}", GameManager.Instance.level);
                break;
            case InfoType.Kill:
                myText.text = string.Format("X{0:F0}", GameManager.Instance.kill);
                break;
            case InfoType.Time:
                // 게임 오버 시 타이머를 멈춤
                if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                    return;

                //기록된 시간
                float remainTimer = GameManager.Instance.gameTime;
                int min = Mathf.FloorToInt(remainTimer / 60);
                int sec = Mathf.FloorToInt(remainTimer % 60);
                myText.text = string.Format("{0:D2}:{1:D2}", min, sec);//D2자리수지정
                break;
            case InfoType.Health:
                float curHealth = GameManager.Instance.health;
                float maxHealth = GameManager.Instance.maxHealth;
                mySlider.value = curHealth / maxHealth;
                break;
            default: 
                break;
        }
    }
}
