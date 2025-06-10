using System.Collections;
using UnityEngine;

public class DynamicEnemy : Entity
{
    private Vector3 dir;
    private SpriteRenderer sprite; 
    private Animator anim;
    private bool IsDead = false;
    private bool IsAttacked = false;

    private void Awake()
    {
        sprite = GetComponentInChildren<SpriteRenderer>(true); 
        anim = GetComponent<Animator>(); 
    }

    private void Start()
    {
        dir = transform.right;
        lives = 2;
    }

    private void Update()
    {
        if (!IsDead && !IsAttacked) State = DynamicEnemyStates.run;
        if (!IsDead && IsAttacked) sprite.flipX = dir.x < 0.0f;
        Move();
    }

    public override void GetDamage()
    {
        if (lives > 1)
        {
            IsAttacked = true;
            State = DynamicEnemyStates.hit;
            StartCoroutine(GetDamageAnimation());
        }

        base.GetDamage();
    }

    private IEnumerator GetDamageAnimation()
    {
        yield return new WaitForSeconds(1f);
        IsAttacked = false;
    }

    private DynamicEnemyStates State
    {
        get { return (DynamicEnemyStates)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Move()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            transform.position + transform.up * 0.1f + transform.right * dir.x * 0.7f,
            0.1f
        );

        if (colliders.Length > 0)
        {
            dir *= -1f;
            sprite.flipX = dir.x < 0.0f; 
        }
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
        }
    }

    private IEnumerator DieAnimation()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        base.Die();
    }

    public override void Die()
    {
        IsDead = true;
        State = DynamicEnemyStates.die;
        StartCoroutine(DieAnimation());
    }
}