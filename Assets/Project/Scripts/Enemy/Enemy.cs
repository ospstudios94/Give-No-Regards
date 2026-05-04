using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public int health = 100;
    public int maxHealth;
    public int Health { get => health; set => health = value; }
// get attacks and throw script here
private float knockbackTimer;
public float knockbackTotalTime = 0.2f;
Rigidbody2D rig;
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (knockbackTimer > 0)
        {
        knockbackTimer -= Time.fixedDeltaTime;
        // If the timer just finished, stop the movement completely
          if (knockbackTimer <= 0) rig.linearVelocity = Vector2.zero; 
        }
       
      
        
    }
    public void Damage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            // death Anim Here
            Destroy(gameObject, 1.5f);
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
{
    knockbackTimer = knockbackTotalTime; // Start the timer
    rig.linearVelocity = Vector2.zero;          // Reset current velocity first
    rig.AddForce(direction * force, ForceMode2D.Impulse);
}
}
