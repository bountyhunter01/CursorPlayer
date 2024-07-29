using UnityEngine;

public class RePosition : MonoBehaviour
{
    private Collider2D coll;
    private Camera mainCamera;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
        mainCamera = Camera.main;
    }
    /// <summary>
    /// 콜라이더가 'Area' 태그를 가진 오브젝트를 벗어날 때 호출됩니다.
    /// 플레이어의 위치를 기준으로 자신의 위치를 재조정합니다.
    /// </summary>
    /// <param name="collision">충돌한 콜라이더</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 myPos = transform.position;

        switch (transform.tag)
        {
            case "Ground":
                float diffX = playerPos.x - myPos.x;
                float diffY = playerPos.y - myPos.y;
                float dirX = diffX < 0 ? -1 : 1;
                float dirY = diffY < 0 ? -1 : 1;
                diffX = Mathf.Abs(diffX);
                diffY = Mathf.Abs(diffY);

                if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 44);
                }
                else if (diffX < diffY)
                {
                    transform.Translate(Vector3.up * dirY * 44);
                }
                break;
            case "Enemy":
                if (coll.enabled && CompareTag("Player"))
                {
                    Vector3 dist = playerPos - myPos;
                    Vector3 ran = new Vector3(Random.Range(4, -4), Random.Range(4, -4), 0);
                    transform.Translate(ran + dist * 2);
                }
                break;
        }
    }
    /// <summary>
    /// 주어진 위치가 카메라의 경계를 벗어나지 않도록 조정합니다.
    /// </summary>
    /// <param name="newPos">새로운 위치</param>
    /// <returns>조정된 위치</returns>
    private Vector3 CheckBoundsAndReposition(Vector3 newPos)
    {
        if (mainCamera == null)
        {
            return newPos;
        }

        Vector3 worldMin = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, mainCamera.transform.position.z));
        Vector3 worldMax = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, mainCamera.transform.position.z));

        newPos.x = Mathf.Clamp(newPos.x, worldMin.x, worldMax.x);
        newPos.y = Mathf.Clamp(newPos.y, worldMin.y, worldMax.y);

        return newPos;
    }
}
