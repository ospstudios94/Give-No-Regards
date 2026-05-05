using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    public int health = 100;
    public int maxHealth;
    Animator anim;
    public int Health { get => health; set => health = value; }
// get attacks and throw script here
public float knockbackTimer = .5f;
public float knockbackTotalTime = 0.5f;
Rigidbody2D rig;
    void Awake()
    {
        anim = GetComponent<Animator>();    
        rig = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
       
       
      
        
    }
    void FixedUpdate()
    {
         if (knockbackTimer > 0)
        {
        knockbackTimer -= Time.fixedDeltaTime;
        // If the timer just finished, stop the movement completely
          if (knockbackTimer <= 0)
            {
                rig.linearVelocity = Vector2.zero; 
            } 
        
        }
    }
    public void Damage(int damage)
    {
        health -= damage;
        anim.SetTrigger("isHit");
        if(health <= 0)
        {
            anim.SetTrigger("isDead");
            // death Anim Here
            Destroy(gameObject, 1.5f);
        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
{
    
    Debug.Log("Forcing Knockback in direction: " + direction);
    knockbackTimer = knockbackTotalTime; // Start the time
   // rig.linearVelocity = Vector2.zero;          // Reset current velocity first
    rig.AddForce(direction * force, ForceMode2D.Impulse);
}
}
