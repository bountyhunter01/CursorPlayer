using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rigid;
    private SpriteRenderer sprite;
    private Animator anim;
    public Scanner scanner;
    public GameObject gameOverText;
    public Vector2 inputVec;
    public float speed = 5f;
    private Collider2D coll;
    private WaitForFixedUpdate wait = new WaitForFixedUpdate();
    private PoolManager poolManager;
    private bool isInvincible = false;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        coll = GetComponent<Collider2D>();
        poolManager = FindObjectOfType<PoolManager>();
    }

    private void FixedUpdate()
    {
        if (!GameManager.Instance.isGamePaused || GameManager.Instance.isGameOver)
            return;

        Vector2 nextVec = inputVec * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void OnMove(InputValue value)
    {
        if (!GameManager.Instance.isGamePaused || GameManager.Instance.isGameOver)
            return;

        inputVec = value.Get<Vector2>();
    }

    private void LateUpdate()
    {
        anim.SetFloat("Speed", inputVec.magnitude);
        if (inputVec.x != 0)
        {
            sprite.flipX = inputVec.x < 0;
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Enemy") || isInvincible)
            return;

        float damage = other.collider.GetComponent<Enemy>().damage;
        GameManager.Instance.TakeDamage(damage);

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(Hited());
        }

        if (GameManager.Instance.health > 0)
        {
            anim.SetTrigger("Hit");
        }
    }

    IEnumerator Hited()
    {
        isInvincible = true;
        coll.enabled = false;
        anim.SetTrigger("Hit");
        yield return new WaitForSeconds(1.0f);
        isInvincible = false;
        coll.enabled = true;
        anim.ResetTrigger("Hit");
    }

    public void Dead()
    {
        GameManager.Instance.isGamePaused = true;
        inputVec = Vector2.zero;
        coll.enabled = false;
        rigid.simulated = false;
        anim.SetTrigger("Dead");
        gameOverText.SetActive(true);

        if (poolManager != null)
        {
            poolManager.SetGameOver(true);
        }
        AudioManager.Instance.StopAllSfx();
    }

    public void ResetInput()
    {
        inputVec = Vector2.zero;
    }
}
