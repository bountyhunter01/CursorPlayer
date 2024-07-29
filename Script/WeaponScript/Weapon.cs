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
    Collider2D weaponCollider; // �ݶ��̴��� �����ϴ� ����
    Light2D weaponLight; // Light2D ������Ʈ ���� ����

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        weaponCollider = GetComponent<Collider2D>(); // �ݶ��̴� �ʱ�ȭ
        weaponLight = GetComponent<Light2D>(); // Light2D �ʱ�ȭ
        gameObject.SetActive(true); // ������Ʈ�� Ȱ��ȭ ���·� ����
    }

    /// <summary>
    /// �����ڿ� ���� ����
    /// </summary>
    /// <param name="damage">�÷��̾ ������ �ִ� ������</param>
    /// <param name="per">���� ����</param>
    /// <param name="dir">������ �ʱ� �̵� ����</param>
    public void Init(float damage, int per, Vector3 dir)
    {
        this.damage = damage;
        this.per = per;
        if (per >= 0)
        {
            rigid.velocity = dir * 10f;
        }
        // per = ����� �������� 
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
                // Hit �ִϸ��̼� Ʈ���Ÿ� �����ϰ� �ִϸ��̼� �̺�Ʈ�� ���� ��Ȱ��ȭ
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
        gameObject.SetActive(false);//�÷��̾� ������ ������ �����
    }

    /// <summary>
    /// �ִϸ��̼� �Ϸ� �� ȣ��Ǵ� �޼���
    /// </summary>
    public void OnHitAnimationComplete()
    {
        gameObject.SetActive(false);
       
    }

    /// <summary>
    /// ���� ������� ����
    /// </summary>
    /// <param name="radius">������ ����</param>
    /// <param name="pullStrength">������� ��</param>
    public void PullEnemies(float radius, float pullStrength)
    {
        // ���� ���� ���Ʈ ������Ʈ�� ������ Ȯ���Ͽ� ������� ������ ����
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
    /// WaterBlast ���� �� 3�� �Ŀ� ��Ȱ��ȭ
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
    /// Explosion ���� �̺�Ʈ�� ���� �ݶ��̴��� Ȱ��ȭ
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
            weaponLight.pointLightOuterRadius = 6.32f; // ���� �ܺ� �ݰ��� ����
        }
    }

    /// <summary>
    /// Explosion ���� �̺�Ʈ�� ���� �ݶ��̴��� ��Ȱ��ȭ
    /// </summary>
    public void ExplosionOutCollider()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false; // ��Ȱ��ȭ �� �ݶ��̴��� ��Ȱ��ȭ
        }
        if (weaponLight != null)
        {
            weaponLight.enabled = false;
        }
        OnHitAnimationComplete();
    }
}
