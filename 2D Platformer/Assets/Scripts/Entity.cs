using UnityEngine;

public class Entity : MonoBehaviour
{
    protected int lives;
   
    public virtual void GetDamage()
    {
        lives--;
        if (lives < 1)
            Die();
    }

    public virtual void Die()
    {
        Destroy(this.gameObject);
    }
}

public enum StaticEnemyStates
{
    idle,
    die,
    hit
}

public enum DynamicEnemyStates
{
    run,
    die,
    hit
}
public enum HeroStates
{
    idle,
    run,
    jump,
    attack
}