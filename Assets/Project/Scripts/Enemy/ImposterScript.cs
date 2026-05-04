using System;
using System.Collections;
using UnityEngine;

public class ImposterScript : MonoBehaviour, IDamagable
{
    public enum AttackStates {Normal,Sneak, Boss}
    public AttackStates currentState;
    public int healthPoints = 100;
    public int maxHealthPoints = 100;

    public int damageOutput = 10; 
    public int Health { get => healthPoints; set => healthPoints = value; }

    Rigidbody2D rig;
    Animator anim;
    [Header("Knockback Settings")]
    private float knockbackTimer;
    public float knockbackTotalTime = 0.2f;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;

            // If the timer just finished, stop the movement completely
            if (knockbackTimer <= 0) 
            {
                rig.linearVelocity = Vector2.zero;
                knockbackTimer = 0;
            }
        }

        // switch(currentState)
        // {
        //     case AttackStates.Normal:
        //     // Search for NPCs 
        //     // Take items in range
        //     // if Player is in range, run away from them
        //     // Kill enemies (his own allies)
        //     break;
        //     case AttackStates.Sneak:
        //     // target NPCs and kill them
        //     // go back to normal
        //     break;
        //     case AttackStates.Boss:
            
        //     // chase and attack only.. the current script
        //     // add phases here
        //     break;
        // }
    }
#region  Boss Fights
// public void InitiateBossFight() // called from another script
//     {
//         StartCoroutine(StartBossPhase())
//         // enter phase 1;
        
//     } 
    
//     IEnumerator StartBossPhase()
//     {
//         // let dialogue get through
//         yield return new WaitForSeconds(2 );
//         Phase1();
//     }// ca
// void Phase1() // all actions, movements here
//     {
       
    
//     }

// void Phase2() // all actions, movements here
//     {
        
//     }
#endregion


public void ApplyKnockback(Vector2 direction, float force)
{
    knockbackTimer = knockbackTotalTime; // Start the timer
    rig.linearVelocity = Vector2.zero;          // Reset current velocity first
    rig.AddForce(direction * force, ForceMode2D.Impulse);
}
public void Damage(int damage)
    {
       healthPoints -= damage;
        if(healthPoints <= 0)
        {
            Death();
        }
    }

private void Death()
    {
       
        // death animation 1 min
        // fade in death 1 min
        Destroy(gameObject, 2.5f);
    }
}
