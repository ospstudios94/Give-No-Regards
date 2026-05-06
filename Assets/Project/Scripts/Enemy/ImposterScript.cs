using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

public class ImposterScript : MonoBehaviour, IDamagable
{
    public enum AttackStates {Normal,Sneak, Boss}
    public enum MoveState { Idle, Chase, Attack, Dodge }

    [Header("Health")]
    public int healthPoints = 100;
    public int maxHealthPoints = 100;
    public int Health { get => healthPoints; set => healthPoints = value; }
   
   
    [Header("Knockback Settings")]
    private float knockbackTimer;
    public float knockbackTotalTime = 0.2f;


    [Header("Dodging")]
    bool isDodging;
    public float dodgeCoolDown = 2.0f; // Seconds between dodges
    private float nextDodgeTime;       // Internal timer
    public float dodgeForce = 50f;
    public float dodgeChance = .3f;

   
    [Header("Attacking")]
     public AttackStates currentState;
    public Transform target; // for the player only
    public float attackRange = .5f;
    public float attackCoolDown = 1.4f;
    public GameObject[] hitColliders; // for activating/Deactivating 
    public bool isActing;
   
    [Header("Movement")]
    public MoveState current = MoveState.Idle;
    public float speed;
    private float dSpeed = 1;

    [Header("Throwing")]
    public float throwRate = 1.0f;
    private float lastThrowTime = 0;

    //public GameObject throwObject;
   
    public float chaseRange = 3f;

   [Header("Detection")] // look up the closest target..
    public float detectRadius; // for enemies.. l
    private Transform detectTarget; 
    public LayerMask targetLayers;
    

    private Vector2 direction;
    SpriteRenderer sp;
    PlayerController play;
    private bool canMove = true;
    Rigidbody2D rig;
    Animator anim;
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
       play = FindAnyObjectByType<PlayerController>();

       target = GameObject.FindGameObjectWithTag("Player").transform;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WakeImposter();
       
    }

    void WakeImposter()
    {
        gameObject.SetActive(true);
        healthPoints = maxHealthPoints;
        isActing = false;
        isDodging = false;
        canMove = true;
    }
    // Update is called once per frame
    void Update()
    { 
   

    // 2. ONLY check knockback if we aren't dodging
    
        CheckForTargets();
       
    }

    void FixedUpdate()
    {
        if (isDodging) return; 
        if (knockbackTimer > 0)
        {
        knockbackTimer -= Time.deltaTime;
        if (knockbackTimer <= 0) rig.linearVelocity = Vector2.zero;
      
        }

        if (isActing) return;
        float distance = Vector2.Distance(transform.position, target.position);
        direction = (target.position - transform.position).normalized;
        
    
    if(!canMove)
    {
        rig.linearVelocity = Vector2.zero;
        speed =0;
           
    }
      
    // Check if player is attacking and within a certain range
    if (play.canAttack && Vector2.Distance(transform.position, target.position) < 1.5f) 
    {
        TryDodge();
    }
    //   switch(current)
    //     {
    //         case MoveState.Idle:

    //         break;
    //         case MoveState.Chase:

    //         break;
    //         case MoveState.Attack:
            
            
    //         break;
    //         case MoveState.Dodge:
    //         break;
    //     }

        if (distance <= attackRange) 
        {
            Debug.Log("Start Attack");
            StartCoroutine(AttackRoutine());
            
        } 
         if(distance > attackRange && distance <= chaseRange)
        {
            current = MoveState.Chase;
            UpdateAnimator(direction);
            Move(direction);
        }
        if(distance > chaseRange)

        {
            current = MoveState.Idle;
            anim.SetBool("Walk", false);
           rig.linearVelocity = Vector2.zero;
        }
        
    }

    void Move(Vector2 dir) 
     {
        rig.linearVelocity = dir * speed;
     }

    void TryDodge()
    {
            if(Time.time >= nextDodgeTime )
            if (UnityEngine.Random.value <dodgeChance) 
            {
                nextDodgeTime = Time.time + dodgeCoolDown;
                StartCoroutine(DodgeRoutine());
            }
        else
        {
            isActing = true;
            nextDodgeTime = Time.time + 0.5f; 
            Invoke("ResetActing", 0.5f); // Brief pause to act like they were "surprised"
    
        }
    }
    void ResetActing() => isActing = false;
    void UpdateAnimator(Vector2 dir) 
    {
        // Locks to cardinal directions by prioritizing the larger axis
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            dir = new Vector2(dir.x > 0 ? 1 : -1, 0);
        else
            dir = new Vector2(0, dir.y > 0 ? 1 : -1);

        anim.SetFloat("moveX", dir.x);
        anim.SetFloat("moveY", dir.y);
        anim.SetBool("Walk", true);

        if(dir.x != 0 || dir.y != 0)
        {
              anim.SetFloat("lastX", dir.x);
            anim.SetFloat("lastY", dir.y);
        }
    }

    IEnumerator AttackRoutine() {

        Debug.Log("I am attacking");
        anim.SetBool("Walk", false);
        current = MoveState.Attack;
        isActing = true;
        canMove = false;
        rig.linearVelocity = Vector2.zero;
        anim.SetTrigger("Attack"); // Ensure your Blend Tree state has this trigger

        yield return new WaitForSeconds(attackCoolDown);
        Debug.Log("I'mma wait..");
        isActing = false;
        canMove = true;
        speed = dSpeed;
        current = MoveState.Idle;
        // Example: Small chance to dodge after attacking
        
    }

    IEnumerator DodgeRoutine() {
        isDodging = true;
       
        current = MoveState.Dodge;
        //cols.enabled = false;
            Vector2 relPos = (transform.position - target.position).normalized;

            Vector2 dodgeDir;
        // Dodge perpendicular to player to "sidestep"
       //dodgeDirection = new Vector2(target.position.y, target.position.x).normalized;
        if (Mathf.Abs(relPos.x) > Mathf.Abs(relPos.y))
        dodgeDir = new Vector2(relPos.x > 0 ? 1 : -1, 0);
    else
        dodgeDir = new Vector2(0, relPos.y > 0 ? 1 : -1);


        // rig.linearVelocity = attackDir * dodgeForce;
       // anim.SetTrigger("Dodge");
        StartCoroutine(FadePerson());
        // yield return new WaitForSeconds(0.3f); // Dodge duration
        float elapsed = 0;
     float duration = 0.25f; // Short and fast
 while (elapsed < duration)
    {
        // MovePosition ensures the physics engine sees the move every frame
        Vector2 newPos = rig.position + (dodgeDir * dodgeForce * Time.deltaTime);
        rig.MovePosition(newPos);
        
        elapsed += Time.deltaTime;
        yield return null; // Wait for next frame
    }

    rig.linearVelocity = Vector2.zero;
        isDodging = false;
      speed = dSpeed;
     
    
        //cols.enabled = true;
    }
    IEnumerator FadePerson()
    {
        // canMove = false;
         sp.color = new Color(0,0,0, .45f);
        yield return new WaitForSeconds(.25f);
        sp.color = new (0,0,0,1f);
        // canMove = true;
    }
    void CheckForTargets()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectRadius, targetLayers);
        foreach (var hit in hitColliders) 
        {

            if( hit.CompareTag("Enemy") || hit.CompareTag("NPC"))
            {
                detectTarget = hit.transform;
            }
        }
    }

    void CheckToDodge()
    {
       // RaycastHit2D hitPlayer = Physics2D.Raycast(transform.position,)
    }

     public void StopMoving()
    {

        canMove = false;
       speed = 0;
    }
   
    public void CanMove()
    {
        foreach (GameObject hit in hitColliders)
        {
            hit.SetActive(false);
        }
        canMove = true;
        speed = dSpeed;
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
      if (isDodging) return; 
    knockbackTimer = knockbackTotalTime; // Start the timer
    rig.linearVelocity = Vector2.zero;          // Reset current velocity first
    rig.AddForce(direction * force, ForceMode2D.Impulse);
    speed = dSpeed;
}
public void Damage(int damage)
    {
         if (isDodging) return;
       healthPoints -= damage;
        anim.SetTrigger("isHit");
        if(healthPoints <= 0)
        {
            healthPoints = 0;
            Death();
        }
    }

private void Death()
    {
        if(currentState == AttackStates.Boss)
        //this.enabled = false;
        {
        anim.SetTrigger("isDead");
        // death animation 1 min
       
        Destroy(gameObject, 2.5f);
        }
        else if(currentState == AttackStates.Sneak)
        {
            gameObject.SetActive(false);
            Invoke(nameof(WakeImposter), 3f); // will make a method to activate in the scene.
        }
        else
        {
            current = MoveState.Idle;
            rig.linearVelocity = Vector2.zero;
            healthPoints = maxHealthPoints;
            
        }
            // fade in death 1 min
    }

     public void EnableCollider()// for attack in the animator
    {
        // F =0, B = 1, L = 2, R = 3
        
        // for (int i = 0; i < hitColliders.Length; i++)
        
            if(anim.GetFloat("lastY") == 1)
            {
                hitColliders[1].gameObject.SetActive(true);
            }
            if(anim.GetFloat("lastY") == -1)
            {
             hitColliders[0].gameObject.SetActive(true);
            }
            if(anim.GetFloat("lastX") == 1)
            {
             hitColliders[3].gameObject.SetActive(true);
            }
            if(anim.GetFloat("lastX") == -1)
            {
             hitColliders[2].gameObject.SetActive(true);
            }
        
        ///
    }

    void OnDrawGizmos()
    {
        
    }
}
