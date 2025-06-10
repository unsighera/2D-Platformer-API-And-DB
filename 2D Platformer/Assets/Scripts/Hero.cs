using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class Hero : Entity
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] public int health;
    private bool isGrounded = false;

    [SerializeField] private Image[] hearts;
    [SerializeField] public int cliamedCoins = 0;
    [SerializeField] public int currentScore = 0;
    [SerializeField] private Sprite aliveHeart;
    [SerializeField] private Sprite deadHeart;

    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource coinSound;
    [SerializeField] private AudioSource hurtSound;

    public static Hero Instance { get; set; }
    public bool IsAttacking = false;
    public bool isRecharged = true;

    public Transform attackPos;
    public float attackRange;
    public LayerMask enemy;

    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public Text CoinsCount;
    public Text TotalScore;

    private void Start()
    {
        lives = 5;
    }
    private void Update()
    {
        if (!IsAttacking && isGrounded) State = HeroStates.idle;

        if (!IsAttacking && Input.GetButton("Horizontal"))
            Run();
        if (!IsAttacking && isGrounded && Input.GetButtonDown("Jump"))
            Jump();
        if (Input.GetButtonDown("Fire1"))
            Attack();

        if (health > 0)
            health = lives;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health) hearts[i].sprite = aliveHeart;
            else hearts[i].sprite = deadHeart;

            if (i < lives) hearts[i].enabled = true;
            else hearts[i].enabled = false;
        }

        if (groundCheckPoint.transform.position.y <= -15)
        {
            for (int i = 0; i < 5; i++)
                GetDamage();
        }

    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void Awake()
    {
        lives = 5;
        health = lives;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        isRecharged = true;
        Instance = this;
    }

    private HeroStates State
    {
        get { return (HeroStates)anim.GetInteger("state"); }
        set { anim.SetInteger("state", (int)value); }
    }

    private void Run()
    {
        if (isGrounded) State = HeroStates.run;

        Vector3 dir = transform.right * Input.GetAxis("Horizontal");
        transform.position = Vector3.MoveTowards(transform.position, transform.position + dir, speed * Time.deltaTime);
   
        sprite.flipX = dir.x < 0.0f;
    }

    private void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        jumpSound.Play();
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheckPoint.position,
            groundCheckRadius,
            groundLayer
        );

        if (!isGrounded)
        {
            State = HeroStates.jump;
        }
    }

    private void Attack()
    {
        if (isGrounded && isRecharged)
        {
            State = HeroStates.attack;
            IsAttacking = true;
            isRecharged = false;

            StartCoroutine(AttackAnimation());
            StartCoroutine(AttackCoolDown());
        }
    }

    private IEnumerator AttackAnimation()
    {
        attackSound.Play();
        yield return new WaitForSeconds(0.4f);
        IsAttacking = false;
    }

    private IEnumerator AttackCoolDown()
    {
        yield return new WaitForSeconds(0.5f);
        isRecharged = true;
    }

    public override void GetDamage()
    {
        hurtSound.Play();
        base.GetDamage();
        health -= 1;
        if (health == 0)
        {
            foreach (var h in hearts)
                h.sprite = deadHeart;
            Die();
            GameOverMenu.Instance.ActivateGameOver();
        }
    }

    public void GetCoin()
    {
        coinSound.Play();
        cliamedCoins++;
        currentScore += 200;
        if (CoinsCount != null)
            CoinsCount.text = cliamedCoins.ToString();
        if (TotalScore != null)
            TotalScore.text = currentScore.ToString();
    } 

    private void OnAttack()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemy);

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Entity>().GetDamage();
        }
    }
}

