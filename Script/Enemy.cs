using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("몬스터 움직임구현")]
    public float speed;

    [Tooltip("몬스터가 따라갈 타겟")]
    public Rigidbody2D target;

    [Tooltip("몬스터가 살아있는지 확인")]
    bool isLive;

    [Tooltip("몬스터의 물리")]
    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;
    [Tooltip("보스몬스터의 공격콜라이더전용")]
    public BoxCollider2D boxCollider2D;

    [Header("몬스터 세부설정")]
    public float health;
    public float maxHealth;
    public float damage;

    [Header("보스 몬스터 설정")]
    private bool isHit = false;
    public bool isBoss = false; // 보스 몬스터인지 확인하는 변수
    private float lastDamageTime; // 마지막으로 데미지를 받은 시간

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
            return; // 몬스터도 정지시키기

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit") || isHit || anim.GetCurrentAnimatorStateInfo(0).IsName("IsAttack"))
            return;

        MoveTowardsTarget();
    }
    /// <summary>
    ///보스 관련 메서드
    /// </summary>
    private void StopAndAttack()
    {
        // 멈추고 공격 상태로 애니메이션 설정
        anim.SetTrigger("IsAttack");
        rigid.velocity = Vector2.zero; // 멈추기 위해 속도 초기화
        anim.ResetTrigger("Hit"); // 공격 중에는 Hit 트리거가 무시되도록 설정
    }

    // Hit 트리거를 다시 활성화할 이벤트 추가
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
            return; // 몬스터도 정지시키기
        if (!isLive)
            return;

        if (isBoss)
        {
            bool facingRight = target.position.x >= rigid.position.x;
            spriter.flipX = facingRight; // 보스는 반대로 플립

            // 콜라이더 크기와 위치 조정
            AdjustBoxCollider(facingRight);
        }
        else
        {
            bool isMonsterBox = target.position.x < rigid.position.x; // 일반 몬스터는 정방향 플립
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
                // 보스의 경우 콜라이더 위치 조정
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
                // 일반 몬스터의 경우 콜라이더 위치 조정
                if (facingRight)
                {
                    boxCollider2D.offset = new Vector2(-0.5979129f, boxCollider2D.offset.y); // 예시값, 필요에 따라 조정
                }
                else
                {
                    boxCollider2D.offset = new Vector2(0.5979129f, boxCollider2D.offset.y); // 예시값, 필요에 따라 조정
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

        // 추가: 모든 상태 초기화
        isHit = false;
        rigid.velocity = Vector2.zero;
    }

    /// <summary>
    /// 데이터에 따라 다른 적이 나오게 하기 위한 메서드
    /// </summary>
    /// <param name="data"></param>
    public void Init(SpawnData data)
    {
        // 게임 매니저에서 플레이어의 현재 레벨을 가져옵니다.
        int playerLevel = GameManager.Instance.level;

        // 플레이어의 레벨에 따른 적 체력 스케일링 요소를 적용합니다.
        float healthScalingFactor = 1 + (playerLevel * 0.5f); // 필요에 따라 승수를 조정하세요.
        maxHealth = data.health * healthScalingFactor;
        health = maxHealth;

        speed = data.speed;
        damage = data.damage;

        // 스케일된 체력을 가진 새로운 적 객체를 생성합니다.
        GameObject newMonster = GameManager.Instance.pool.Get(data.spriteType);
        newMonster.transform.position = transform.position;
        newMonster.SetActive(true);

        // Enemy 컴포넌트를 가져와서 초기화합니다.
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
    /// 데미지 받았을 때 받는 넉백 및 일정 시간 동안 멈춤
    /// </summary>
    /// <returns></returns>
    IEnumerator HitReaction()
    {
        isHit = true;
        yield return wait; // 다음 하나의 물리 프레임 쉬기
        Vector3 playerPos = GameManager.Instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f); // Hit 애니메이션 지속 시간
        isHit = false;
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 몬스터무기콜라이더를 참조하는 변수
    /// </summary>
    public void BossActivateCollider()
    {
        if (coll != null)
        {
            boxCollider2D.enabled = true;
        }
    }

    /// <summary>
    /// Explosion 전용 이벤트를 통해 콜라이더를 비활성화
    /// </summary>
    public void BossOutCollider()
    {
        if (coll != null)
        {
            boxCollider2D.enabled = false; // 비활성화 시 콜라이더도 비활성화
        }
    }
}
