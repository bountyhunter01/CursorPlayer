using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("���� �����ӱ���")]
    public float speed;

    [Tooltip("���Ͱ� ���� Ÿ��")]
    public Rigidbody2D target;

    [Tooltip("���Ͱ� ����ִ��� Ȯ��")]
    bool isLive;

    [Tooltip("������ ����")]
    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;
    [Tooltip("���������� �����ݶ��̴�����")]
    public BoxCollider2D boxCollider2D;

    [Header("���� ���μ���")]
    public float health;
    public float maxHealth;
    public float damage;

    [Header("���� ���� ����")]
    private bool isHit = false;
    public bool isBoss = false; // ���� �������� Ȯ���ϴ� ����
    private float lastDamageTime; // ���������� �������� ���� �ð�

    private Transform player;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGamePaused)
            return; // ���͵� ������Ű��

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") || isHit || anim.GetCurrentAnimatorStateInfo(0).IsName("IsAttack"))
            return;

        MoveTowardsTarget();
    }
    /// <summary>
    ///���� ���� �޼���
    /// </summary>
    private void StopAndAttack()
    {
        // ���߰� ���� ���·� �ִϸ��̼� ����
        anim.SetTrigger("IsAttack");
        rigid.velocity = Vector2.zero; // ���߱� ���� �ӵ� �ʱ�ȭ
        anim.ResetTrigger("Hit"); // ���� �߿��� Hit Ʈ���Ű� ���õǵ��� ����
    }

    // Hit Ʈ���Ÿ� �ٽ� Ȱ��ȭ�� �̺�Ʈ �߰�
    public void AllowHitTrigger()
    {
        anim.SetTrigger("Hit");
    }

    private void MoveTowardsTarget()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    private void LateUpdate()
    {
        if (!GameManager.Instance.isGamePaused)
            return; // ���͵� ������Ű��
        if (!isLive)
            return;

        if (isBoss)
        {
            bool facingRight = target.position.x >= rigid.position.x;
            spriter.flipX = facingRight; // ������ �ݴ�� �ø�

            // �ݶ��̴� ũ��� ��ġ ����
            AdjustBoxCollider(facingRight);
        }
        else
        {
            bool isMonsterBox = target.position.x < rigid.position.x; // �Ϲ� ���ʹ� ������ �ø�
            spriter.flipX = isMonsterBox;
            if (boxCollider2D != null)
            {
                AdjustBoxCollider(isMonsterBox);
            }
        }
    }

    private void AdjustBoxCollider(bool facingRight)
    {
        if (boxCollider2D != null)
        {
            if (isBoss)
            {
                // ������ ��� �ݶ��̴� ��ġ ����
                if (facingRight)
                {
                    boxCollider2D.offset = new Vector2(0.9287037f, boxCollider2D.offset.y);
                }
                else
                {
                    boxCollider2D.offset = new Vector2(-0.9287037f, boxCollider2D.offset.y);
                }
            }
            else
            {
                // �Ϲ� ������ ��� �ݶ��̴� ��ġ ����
                if (facingRight)
                {
                    boxCollider2D.offset = new Vector2(-0.5979129f, boxCollider2D.offset.y); // ���ð�, �ʿ信 ���� ����
                }
                else
                {
                    boxCollider2D.offset = new Vector2(0.5979129f, boxCollider2D.offset.y); // ���ð�, �ʿ信 ���� ����
                }
            }
        }
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            target = GameManager.Instance.player.GetComponent<Rigidbody2D>();
            player = GameManager.Instance.player.transform;
        }
        isLive = true;
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);
        health = maxHealth;
        lastDamageTime = Time.time;

        // �߰�: ��� ���� �ʱ�ȭ
        isHit = false;
        rigid.velocity = Vector2.zero;
    }

    /// <summary>
    /// �����Ϳ� ���� �ٸ� ���� ������ �ϱ� ���� �޼���
    /// </summary>
    /// <param name="data"></param>
    public void Init(SpawnData data)
    {
        // ���� �Ŵ������� �÷��̾��� ���� ������ �����ɴϴ�.
        int playerLevel = GameManager.Instance.level;

        // �÷��̾��� ������ ���� �� ü�� �����ϸ� ��Ҹ� �����մϴ�.
        float healthScalingFactor = 1 + (playerLevel * 0.5f); // �ʿ信 ���� �¼��� �����ϼ���.
        maxHealth = data.health * healthScalingFactor;
        health = maxHealth;

        speed = data.speed;
        damage = data.damage;

        // �����ϵ� ü���� ���� ���ο� �� ��ü�� �����մϴ�.
        GameObject newMonster = GameManager.Instance.pool.Get(data.spriteType);
        newMonster.transform.position = transform.position;
        newMonster.SetActive(true);

        // Enemy ������Ʈ�� �����ͼ� �ʱ�ȭ�մϴ�.
        Enemy enemyComponent = newMonster.GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            enemyComponent.speed = data.speed;
            enemyComponent.maxHealth = maxHealth;
            enemyComponent.health = health;
            enemyComponent.damage = data.damage;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopAndAttack();
        }
        else if (other.CompareTag("Weapon") && isLive && !isHit)
        {
            Weapon weapon = other.GetComponent<Weapon>();
            if (weapon != null)
            {
                health -= weapon.damage;
                lastDamageTime = Time.time;

                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(HitReaction());
                }

                if (health > 0)
                {
                    anim.SetTrigger("Hit");
                }
                else
                {
                    Die();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            anim.ResetTrigger("IsAttack");
        }
    }

    private void Die()
    {
        if (!isBoss)
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetBool("Dead", true);
            GameManager.Instance.kill++;
            GameManager.Instance.GetExp();
        }
        else
        {
            isLive = false;
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;
            anim.SetTrigger("Dead");
            GameManager.Instance.kill++;
            GameManager.Instance.GetExp();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerMove player = collision.collider.GetComponent<PlayerMove>();
            if (player != null)
            {
                player.OnCollisionEnter2D(collision);
            }
        }
    }

    /// <summary>
    /// ������ �޾��� �� �޴� �˹� �� ���� �ð� ���� ����
    /// </summary>
    /// <returns></returns>
    IEnumerator HitReaction()
    {
        isHit = true;
        yield return wait; // ���� �ϳ��� ���� ������ ����
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f); // Hit �ִϸ��̼� ���� �ð�
        isHit = false;
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// ���͹����ݶ��̴��� �����ϴ� ����
    /// </summary>
    public void BossActivateCollider()
    {
        if (coll != null)
        {
            boxCollider2D.enabled = true;
        }
    }

    /// <summary>
    /// Explosion ���� �̺�Ʈ�� ���� �ݶ��̴��� ��Ȱ��ȭ
    /// </summary>
    public void BossOutCollider()
    {
        if (coll != null)
        {
            boxCollider2D.enabled = false; // ��Ȱ��ȭ �� �ݶ��̴��� ��Ȱ��ȭ
        }
    }
}
