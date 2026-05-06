using UnityEngine;

public class AttackScript : MonoBehaviour
{
    /// <summary>
    /// This is for the Imposter only
    /// </summary>
     public int damageOutput = 5;
    public float knockbackForce = 20f;


    private void OnTriggerEnter2D(Collider2D other)
    {
       if(other.CompareTag("Player") || other.CompareTag("NPC") || other.CompareTag("Enemy"))
        {
            Vector2 knockbackDirection = 
            // other.GetComponent<Rigidbody2D>().linearVelocity.normalized; // if it is moving
            (other.transform.position - transform.position).normalized;
            
            IDamagable damageable = other.GetComponent<IDamagable>();
            if(damageable != null)
            {
                damageable.Damage(damageOutput);
               
            }
        
            Enemy newEnemy = other.GetComponent<Enemy>();
            if(newEnemy != null)
            {
                newEnemy.ApplyKnockback(knockbackDirection, knockbackForce );
            }
            NPC_Script nonPlayer = other.GetComponent<NPC_Script>();

            if(nonPlayer != null)
            {
                nonPlayer.ApplyKnockback(knockbackDirection, knockbackForce);
            }
            PlayerController player = other.GetComponent<PlayerController>();
            if(player != null)
            {
                player.ApplyKnockback(knockbackDirection, knockbackForce);
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
