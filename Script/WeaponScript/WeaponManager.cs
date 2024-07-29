using System.Collections;
using UnityEngine;

public enum TYPE
{
    FIREBALL, ICEBALL, WaterBlast, Laser, Explosion
}

public class WeaponManager : MonoBehaviour
{
    public TYPE id;
    [Tooltip("풀매니저의 몇번째id")]
    public int prefabId;
    public float damage;
    [Tooltip("관통력")]
    public int count;
    [Tooltip("회전속도")]
    public float speed;
    [Tooltip("공격 쿨타임")]
    public float cooldown = 1f; // 기본 공격 쿨타임
    private float timer;
    PlayerMove player;

    Animator ani;
    Collider2D coll;
    Rigidbody2D rigid;

    [Tooltip("회오리바람 끌어당기는 힘")]
    private float pullStrength = 2f;

    [Tooltip("회오리바람 속도")]
    private float waterBlastSpeed = 5f;

    private void Awake()
    {
        player = GameManager.Instance?.player;
        ani = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!GameManager.Instance.isGamePaused)
            return;//무기도 정지시키기

        timer += Time.deltaTime;

        switch (id)
        {
            case TYPE.FIREBALL:
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                damage = 10f;
                break;

            case TYPE.ICEBALL:
                if (timer > cooldown)
                {
                    timer = 0f;
                    IceBall();
                }
                break;
            case TYPE.WaterBlast:
                if (timer > cooldown)
                {
                    timer = 0f;
                    WaterBlast();
                }
                break;
            case TYPE.Laser:
                if (timer > cooldown)
                {
                    timer = 0f;
                    Laser();
                }
                break;
            case TYPE.Explosion:
                if (timer > cooldown)
                {
                    timer = 0f;
                    Explosion();
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 무기 레벨업하는 메서드
    /// </summary>
    /// <param name="damage">추가 데미지</param>
    /// <param name="count">추가 관통력</param>
    public void LevelUp(float damage, int count)
    {
        this.damage += damage;
        this.count += count;
        if (id == TYPE.FIREBALL)
        {
            Batch();
        }
        if (id == TYPE.ICEBALL)
        {
            IceBall();
        }
        if (id == TYPE.WaterBlast)
        {
            WaterBlast();
        }
        if (id == TYPE.Laser)
        {
            Laser();
        }
        if (id == TYPE.Explosion)
        {
            Explosion();
        }
    }

    /// <summary>
    /// 초기화 메서드 역할
    /// </summary>
    public void Init(ItemData data)
    {
        name = "Weapon" + data.itemId;
        if (player == null)
        {
            player = GameManager.Instance.player;
            if (player == null)
            {
                return;
            }
        }
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = (TYPE)data.itemId;
        count = data.baseCount;
        damage = data.baseDamage;

        for (int index = 0; index < GameManager.Instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.Instance.pool.prefabs[index])
            {
                prefabId = index;

                break;
            }
        }

        switch (id)
        {
            case TYPE.FIREBALL:
                speed = 150;
                Batch();
                break;
            case TYPE.ICEBALL:
                cooldown = 0.6f;
                break;
            case TYPE.WaterBlast:
                cooldown = 3.5f;
                break;
            case TYPE.Laser:
                cooldown = 1.2f;
                break;
            case TYPE.Explosion:
                cooldown = 3.9f;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 파이어볼 스킬
    /// </summary>
    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform weapon;
            if (index < transform.childCount)
            {
                weapon = transform.GetChild(index);
            }
            else
            {
                weapon = GameManager.Instance.pool.Get(prefabId)?.transform;
                if (weapon == null)
                {

                    continue;
                }
                weapon.parent = transform;
            }

            weapon.localPosition = Vector3.zero;
            weapon.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            weapon.Rotate(rotVec);
            weapon.Translate(weapon.up * 1.5f, Space.World);
            weapon.GetComponent<Weapon>().Init(damage, -100, Vector3.zero);
        }
    }

    /// <summary>
    /// 원거리 무기 발사하는 방향 만드는 메서드
    /// </summary>
    void IceBall()
    {
        if (player.scanner.nearestTarget == null)
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir.Normalize();

        for (int index = 0; index < count; index++)
        {
            Vector3 spreadDir = Quaternion.Euler(0, 0, 360 * index / count) * dir;
            Transform weapon = GameManager.Instance.pool.Get(prefabId)?.transform;

            if (weapon == null)
            {//죽으면서 타겟을 찾는 플레이어가 사라지니 에러가뜸
                continue;
            }
            weapon.position = GameManager.Instance.player.transform.position;
            float angle = Mathf.Atan2(spreadDir.y, spreadDir.x) * Mathf.Rad2Deg;
            weapon.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            weapon.GetComponent<Weapon>().Init(damage, count, spreadDir);
        }
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.IceBall);
    }

    /// <summary>
    /// 레이저 발사 메서드
    /// </summary>
    void Laser()
    {
        if (player.scanner.nearestTarget == null)
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir.Normalize();

        Transform weapon = GameManager.Instance.pool.Get(prefabId)?.transform;

        if (weapon == null)
        {
            return;
        }
        weapon.position = GameManager.Instance.player.transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        weapon.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        weapon.GetComponent<Weapon>().Init(damage, count, dir);

        // 레이저의 스케일을 레벨에 따라 증가시키기
        Vector3 newScale = weapon.localScale;
        newScale.y += 0.2f * (count - 1); // count가 레벨을 의미하도록 설정
        weapon.localScale = newScale;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Laser);
    }

    /// <summary>
    /// 레이저랑 똑같지만 스피드 조절을 위한 폭발 메서드
    /// </summary>
    void Explosion()
    {
        if (player.scanner.nearestTarget == null)
        {
            return;
        }

        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = targetPos - transform.position;
        dir.Normalize();

        RaycastHit2D[] targets = player.scanner.targets;
        int targetCount = Mathf.Min(count, targets.Length);

        for (int index = 0; index < targetCount; index++)
        {
            // 각 발사체가 다른 방향으로 발사되도록 설정
            Vector3 spreadDir;
            if (index < targets.Length)
            {
                spreadDir = (targets[index].transform.position - transform.position).normalized;
            }
            else
            {
                spreadDir = Quaternion.Euler(0, 0, 360 * index / count) * dir;
            }
            Transform weapon = GameManager.Instance.pool.Get(prefabId)?.transform;

            if (weapon == null)
            {
                continue;
            }
            weapon.position = GameManager.Instance.player.transform.position;

            weapon.rotation = Quaternion.identity;
            weapon.GetComponent<Weapon>().Init(damage, count, spreadDir * 0.3f);
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.FirExplosion);
        }
    }

    /// <summary>
    /// 회오리 공격이 적을 끌어당기며 데미지를 주는 메서드
    /// </summary>
    void WaterBlast()
    {
        RaycastHit2D[] targets = player.scanner.targets;
        int targetCount = Mathf.Min(count, targets.Length);

        for (int index = 0; index < targetCount; index++)
        {
            Transform weapon = GameManager.Instance.pool.Get(prefabId)?.transform;
            if (weapon == null)
            {
                continue;
            }

            weapon.position = transform.position;
            Vector3 direction;
            if (index < targets.Length)
            {
                direction = (targets[index].transform.position - transform.position).normalized;
            }
            else
            {
                direction = Random.insideUnitCircle.normalized;
            }

            weapon.GetComponent<Weapon>().Init(damage, count, direction);
            StartCoroutine(WaterBlastRoutine(weapon, direction));
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.Wind);
        }
    }

    /// <summary>
    /// 회오리 공격의 루틴을 관리하는 메서드
    /// </summary>
    private IEnumerator WaterBlastRoutine(Transform weapon, Vector3 direction)
    {
        float elapsed = 0f;
        Rigidbody2D weaponRigidbody = weapon.GetComponent<Rigidbody2D>();
        while (elapsed < 3f)
        {
            elapsed += Time.deltaTime;
            weaponRigidbody.velocity = direction * waterBlastSpeed;

            // 현재 워터 블라스트 오브젝트의 개수를 확인하여 끌어당기는 강도를 조절
            int waterBlastCount = FindObjectsOfType<WeaponManager>().Length;
            float adjustedPullStrength = pullStrength / waterBlastCount;

            Collider2D[] enemies = Physics2D.OverlapCircleAll(weapon.position, 5f);
            foreach (Collider2D enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    Vector3 pullDirection = weapon.position - enemy.transform.position;
                    enemy.transform.position += pullDirection.normalized * Time.deltaTime * adjustedPullStrength;
                }
            }

            yield return null;
        }

        if (ani != null)
        {
            ani.SetTrigger("Hit");
            yield return new WaitForSeconds(1f);
            ani.ResetTrigger("Hit");
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }
    }
}
