using UnityEngine;

public class Coin : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true; 
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Hero.Instance.GetCoin();
            Destroy(gameObject);
        }
    }
}
