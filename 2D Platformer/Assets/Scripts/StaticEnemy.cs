using System.Collections;
using UnityEngine;

public class StaticEnemy : Entity
{
    private Animator anim;
    private bool IsDead = false;
    private bool IsAttacked = false;

    private void Start()
    {
        lives = 3;
    }

    private void Update()
    {
        if (!IsDead && !IsAttacked) State = StaticEnemyStates.idle;
    }

    public void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject == Hero.Instance.gameObject)
        {
            Hero.Instance.GetDamage();
            lives--;
        }

        if (lives > 1)
        {
            IsAttacked = true;
            State = StaticEnemyStates.hit;
            StartCoroutine(GetDamageAnimation());
        }
        if (lives < 1)
            Die();
    }

    public override void GetDamage()
    {
        if (lives > 1)
        {
            IsAttacked = true;
            State = StaticEnemyStates.hit;
            StartCoroutine(GetDamageAnimation());
        }

        base.GetDamage();
    }

    private IEnumerator GetDamageAnimation()
    {
        yield return new WaitForSeconds(1f);
        IsAttacked = false;
    }

    private StaticEnemyStates State
    {
        get { return (StaticEnemyStates)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    public override void Die()
    {
        IsDead = true;
        State = StaticEnemyStates.die;
        StartCoroutine(DieAnimation());
    }

    private IEnumerator DieAnimation()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        base.Die();
    }
}
