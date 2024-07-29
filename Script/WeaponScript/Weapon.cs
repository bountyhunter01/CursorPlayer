using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Weapon : MonoBehaviour
{
    public float damage;
    public int per;
    Rigidbody2D rigid;
    public bool isHit;
    Animator animator;
    Collider2D weaponCollider; // 콜라이더를 참조하는 변수
    Light2D weaponLight; // Light2D 컴포넌트 참조 변수

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weaponCollider = GetComponent<Collider2D>(); // 콜라이더 초기화
        weaponLight = GetComponent<Light2D>(); // Light2D 초기화
        gameObject.SetActive(true); // 오브젝트를 활성화 상태로 설정
    }

    /// <summary>
    /// 생성자와 같은 역할
    /// </summary>
    /// <param name="damage">플레이어가 적에게 주는 데미지</param>
    /// <param name="per">관통 변수</param>
    /// <param name="dir">무기의 초기 이동 방향</param>
    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;
        if (per >= 0)
        {
            rigid.velocity = dir * 10f;
        }
        // per = 양수는 근접무기 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((!other.CompareTag("Enemy") && other.gameObject.layer != LayerMask.NameToLayer("Enemy")) || per == -100)
            return;

        per--;
        if (per < 0)
        {
            rigid.velocity = Vector2.zero;
            isHit = true;
            if (isHit == true && animator != null)
            {
                // Hit 애니메이션 트리거를 설정하고 애니메이션 이벤트를 통해 비활성화
                if (animator.parameterCount > 0 && animator.parameters[0].name == "Hit")
                {
                    animator.SetTrigger("Hit");
                }
                
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;
        gameObject.SetActive(false);//플레이어 에리어 밖으로 벗어나면
    }

    /// <summary>
    /// 애니메이션 완료 시 호출되는 메서드
    /// </summary>
    public void OnHitAnimationComplete()
    {
        gameObject.SetActive(false);
       
    }

    /// <summary>
    /// 적을 끌어당기는 로직
    /// </summary>
    /// <param name="radius">끌어당길 범위</param>
    /// <param name="pullStrength">끌어당기는 힘</param>
    public void PullEnemies(float radius, float pullStrength)
    {
        // 현재 워터 블라스트 오브젝트의 개수를 확인하여 끌어당기는 강도를 조절
        int waterBlastCount = FindObjectsOfType<WeaponManager>().Length;
        float adjustedPullStrength = pullStrength / waterBlastCount;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D enemy in enemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                Vector3 direction = transform.position - enemy.transform.position;
                enemy.transform.position += direction.normalized * Time.deltaTime * adjustedPullStrength;
            }
        }
    }

    /// <summary>
    /// WaterBlast 생성 후 3초 후에 비활성화
    /// </summary>
    public IEnumerator WaterBlastRoutine()
    {
        float elapsed = 0f;
        while (elapsed < 3f)
        {
            elapsed += Time.deltaTime;
            PullEnemies(6f, 0.5f);
            yield return null;
        }

        OnHitAnimationComplete();
    }

   /// <summary>
    /// Explosion 전용 이벤트를 통해 콜라이더를 활성화
    /// </summary>
    public void ActivateCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }
        if (weaponLight != null)
        {
            weaponLight.enabled = true;
            weaponLight.pointLightOuterRadius = 6.32f; // 빛의 외부 반경을 설정
        }
    }

    /// <summary>
    /// Explosion 전용 이벤트를 통해 콜라이더를 비활성화
    /// </summary>
    public void ExplosionOutCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // 비활성화 시 콜라이더도 비활성화
        }
        if (weaponLight != null)
        {
            weaponLight.enabled = false;
        }
        OnHitAnimationComplete();
    }
}
