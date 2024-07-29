using System.Collections;
using UnityEngine;

public enum TYPE
{
    FIREBALL, ICEBALL, WaterBlast, Laser, Explosion
}

public class WeaponManager : MonoBehaviour
{
    public TYPE id;
    [Tooltip("Ǯ�Ŵ����� ���°id")]
    public int prefabId;
    public float damage;
    [Tooltip("�����")]
    public int count;
    [Tooltip("ȸ���ӵ�")]
    public float speed;
    [Tooltip("���� ��Ÿ��")]
    public float cooldown = 1f; // �⺻ ���� ��Ÿ��
    private float timer;
    PlayerMove player;

    Animator ani;
    Collider2D coll;
    Rigidbody2D rigid;

    [Tooltip("ȸ�����ٶ� ������� ��")]
    private float pullStrength = 2f;

    [Tooltip("ȸ�����ٶ� �ӵ�")]
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
            return;//���⵵ ������Ű��

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
    /// ���� �������ϴ� �޼���
    /// </summary>
    /// <param name="damage">�߰� ������</param>
    /// <param name="count">�߰� �����</param>
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
    /// �ʱ�ȭ �޼��� ����
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
    /// ���̾ ��ų
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
    /// ���Ÿ� ���� �߻��ϴ� ���� ����� �޼���
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
            {//�����鼭 Ÿ���� ã�� �÷��̾ ������� ��������
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
    /// ������ �߻� �޼���
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

        // �������� �������� ������ ���� ������Ű��
        Vector3 newScale = weapon.localScale;
        newScale.y += 0.2f * (count - 1); // count�� ������ �ǹ��ϵ��� ����
        weapon.localScale = newScale;
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.Laser);
    }

    /// <summary>
    /// �������� �Ȱ����� ���ǵ� ������ ���� ���� �޼���
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
            // �� �߻�ü�� �ٸ� �������� �߻�ǵ��� ����
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
    /// ȸ���� ������ ���� ������� �������� �ִ� �޼���
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
    /// ȸ���� ������ ��ƾ�� �����ϴ� �޼���
    /// </summary>
    private IEnumerator WaterBlastRoutine(Transform weapon, Vector3 direction)
    {
        float elapsed = 0f;
        Rigidbody2D weaponRigidbody = weapon.GetComponent<Rigidbody2D>();
        while (elapsed < 3f)
        {
            elapsed += Time.deltaTime;
            weaponRigidbody.velocity = direction * waterBlastSpeed;

            // ���� ���� ���Ʈ ������Ʈ�� ������ Ȯ���Ͽ� ������� ������ ����
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
