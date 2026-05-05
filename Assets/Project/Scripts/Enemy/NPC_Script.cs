using System;
using UnityEditor.SceneManagement;
using UnityEngine;

public class NPC_Script : MonoBehaviour, IDamagable
{
    /// <summary>
    /// Area concerning Dialogue
    /// </summary>
    
    public bool isLie = false; // false if the truth, true if it is a lie // test
    private bool hasSwitched = false;
    Rigidbody2D rig;
    Animator anim;

    // Dialogue Events
    public static Action OnLie;
    public static Action OnInterrogation;

// patrolling movement with wait time.
    [Header("Health")]
    public int health = 10;
    public int maxHealth = 10; 
    public int Health { get => health; set => health = value; }
    [Space]
    [Header("Knock Back")]
    private float knockbackTimer;
    public float knockbackTotalTime = .25f;

   
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        OnLie += SetToEnemy;
        OnInterrogation += InitiateAttack;
    }

    // Update is called once per frame
    void Update()
    {
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;

            // If the timer just finished, stop the movement completely
            if (knockbackTimer <= 0) rig.linearVelocity = Vector2.zero;
        }
        if(isLie && !hasSwitched)
        {
            SetToEnemy();
        }
    }
    /// Player interaction here
    public void OnInteract()
    {
        
    }
    public void SetToEnemy() // if Lied and set this anywhere
    {
        if(isLie == true)
       // make the NPC have its own movement (have to for the imposter)
       {
        tag = "Enemy";
        gameObject.layer = 9;
       hasSwitched = true;
            isLie = true; // set to true.. never to be truthful again
       
       } 
    }
    void InitiateAttack() // for telling the truth
    {
        if(!isLie)
        {
            // if the imposter in range
            // move and attack him.
        }
       // get a script here
    }
public void ApplyKnockback(Vector2 direction, float force)
{
    knockbackTimer = knockbackTotalTime; // Start the timer
    rig.linearVelocity = Vector2.zero;          // Reset current velocity first
    rig.AddForce(direction * force, ForceMode2D.Impulse);
}
    public void Damage(int damage)
    {
       health -= damage;
        anim.SetTrigger("isHit");
        if(health <= 0)
        {
            // make all animations a second long
            /// animation here
            Destroy(gameObject, 1.55f);
        }
    }

    void OnDestroy()
    {
        OnLie -= SetToEnemy;
        OnInterrogation -= InitiateAttack;
    }

}
