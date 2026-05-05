using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int damageOutput = 5;
    public float knockbackForce = 50f;
      [HideInInspector] public PlayerController shooterPressure;

    void Awake()
    {
        shooterPressure = FindAnyObjectByType<PlayerController>();
    }
    void Start()
    {

        Destroy(gameObject, 1.5f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
       if(other.CompareTag("Enemy") || other.CompareTag("Imposter"))
        {
            Vector2 knockbackDirection = 
            other.GetComponent<Rigidbody2D>().linearVelocity.normalized; // if it is moving
            //(other.transform.position - transform.position).normalized;
             int finalDamage = damageOutput;
            if (shooterPressure != null)
            {
                // Multiply damage based on the system we built (higher damage at low pressure)
                finalDamage = Mathf.RoundToInt(damageOutput * shooterPressure.GetDamageMultiplier());
            }
            IDamagable damageable = other.GetComponent<IDamagable>();
            if(damageable != null)
            {
                damageable.Damage(finalDamage);
                Destroy(gameObject);
            }
            Enemy newEnemy = other.GetComponent<Enemy>();
            if(newEnemy != null)
            {
                newEnemy.ApplyKnockback(knockbackDirection, knockbackForce );
            }
             ImposterScript impost = other.GetComponent<ImposterScript>();

            if(impost != null)
            {
                impost.ApplyKnockback(knockbackDirection, knockbackForce);
            }

        //     Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        // if (rb != null)
        // {
        //     // Calculate direction from projectile to target
        //     Vector2 direction = (other.transform.position - transform.position).normalized;
        //     rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        // }

        }
    
        // // 1. Try to find the IDamageable interface on the hit object
        // IDamageable damageable = other.GetComponent<IDamageable>();

        // // 2. If it exists, apply damage
        // if (damageable != null)
        // {
        //     damageable.TakeDamage(damage);
            
        //     // Optional: Add impact effects or destroy the projectile
        //     Destroy(gameObject); 
        // }
    }
    
}
