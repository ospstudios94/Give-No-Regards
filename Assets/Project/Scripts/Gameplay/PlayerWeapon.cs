using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public int damageOutput = 10;
    public float knockbackForce = 50f;
    [HideInInspector] public PlayerController shooterPressure;
    void Awake()
    {
        shooterPressure = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
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
                //Destroy(gameObject);
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
        }

        // for destructibles
    }
}
