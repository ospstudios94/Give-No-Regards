using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamagable
{
    Game_Inputs games;
    Rigidbody2D rig;
    Vector2 direction; // for player movement
     Vector2 lastFacingDirection = Vector2.right; // stores direction
    
    bool canRun = false;
    bool canMove = true;
    bool canThrow = false;
  
    bool canAttack = false;

    private WaitForSeconds wait;
    private WaitForSecondsRealtime realTime;
    private float lastThrowTime;
    private float lastAttackRate;

    Vector2 screenBounds;
    private float objectWidth;
    private float objectHeight;

    [Header("Player Movement")]
    [SerializeField]private float _speed;
    [SerializeField]private float _initialSpeed = 95f;
    [SerializeField]private float runMultiplier = 1.3f;
    [SerializeField]private float pauseDuration = .3f;
    [SerializeField]private float realTimePause = .25f;

    [Header("Throw Attributes")]
    [SerializeField]private float throwRate = 1.5f;
    [SerializeField]private GameObject throwItem;
    [SerializeField]private Transform throwPoint;
    [SerializeField]private float throwForce = 500f;

    [Header("Attack Attributes")]
    [SerializeField]private float attackRate = 1f;
    [SerializeField]GameObject[] hitColliders; // for activating/Deactivating

    public int hp = 100;
    public int maxHp = 100;
    public int Health { get => hp; set => hp = value; }
    private float knockbackTimer;
    public float knockbackTotalTime = 0.2f;

[Header("Pressure System")]
 public float pressure = 100f;
    public float maxPressure = 100f;
    public float decayRate = 1f; 
    public float regenRate = 2f; // How fast they recover while standing still
    
    public bool isPressureDrained; // global variable.. to stop the drain or not
    public float healthDrainRate = 2f; // Damage per second when at 0
    private float damageTickTimer = 0f; // when the timer starts for health drain
    private float heartbeatTimer;

    private bool isRestoring;

    private int totalCount = 0;
    public int maxUse =3;
    
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    
        games = new();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeInputs();
        if(rig == null)
        {
            Debug.LogWarning("Rigidbody is not found");
        } 
        wait = new WaitForSeconds(pauseDuration); // for throwing, attacking for the player
        realTime = new WaitForSecondsRealtime(realTimePause); // for movement,ui, animation and everything else
          screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        objectWidth = transform.GetComponent<SpriteRenderer>().bounds.extents.x; 
    objectHeight = transform.GetComponent<SpriteRenderer>().bounds.extents.y;

     
    }

    void InitializeInputs()
    {
        games.Enable();
        games.Player.Sprint.performed += ctx => canRun = true;
        games.Player.Sprint.canceled += ctx => canRun = false;

        games.Player.Throw.performed += ThrowObject;
        games.Player.Attack.performed += InitiateAttack;

        games.Player.Interact.performed += Interaction;
        games.Player.Rest.performed += ctx => isRestoring = true;
        games.Player.Rest.canceled += ctx => isRestoring  = false;

    }

    
    private void Interaction(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void InitiateAttack(InputAction.CallbackContext context)
    {
        if(Time.time > attackRate + lastAttackRate)
       {
            lastAttackRate = Time.time;
            StartCoroutine(Attacking());
       }
    }

    public IEnumerator Attacking()
    {
       canAttack = true;
       Debug.Log("Attack");   
       // add directionals and animation here
rig.linearVelocity = Vector2.zero;
       yield return wait;
    
       canAttack = false;
    }

    private void ThrowObject(InputAction.CallbackContext context)
    {
        if(Time.time > throwRate + lastThrowTime)
       {
            lastThrowTime = Time.time;
            StartCoroutine(Throwing());
       }
    }

    private IEnumerator Throwing()
    {
        canThrow = true; 
       rig.linearVelocity = Vector2.zero;
       Debug.Log("Throwing");   
       // rig.linearVelocity = Vector2.zero;
        GameObject bulletObj = Instantiate(throwItem, throwPoint.position, Quaternion.identity);
         PlayerProjectile pScript = bulletObj.GetComponent<PlayerProjectile>();
        if(pScript != null) {
        pScript.shooterPressure = this; // 'this' refers to this Player script
    }
        Rigidbody2D newRig = bulletObj.GetComponent<Rigidbody2D>();
        newRig.AddForce(lastFacingDirection * throwForce, ForceMode2D.Impulse);

       // add directionals and animation here
      
       yield return wait;
    
       canThrow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.gKey.wasPressedThisFrame)
        {
            Damage(2);
        }
          float pressureRatio = Mathf.Clamp(pressure / maxPressure, 0.2f, 1f);
        if(canRun)
        {
            _speed = _initialSpeed * runMultiplier * pressureRatio;
        }
        else
        {
            _speed = _initialSpeed * pressureRatio ;
        }

   if(games.Player.Rest.WasReleasedThisFrame())
        {
            
            totalCount++;
            
            if(totalCount == maxUse)
            {
                Debug.Log(games.Player.Rest.ToString() + isRestoring.ToString() + totalCount.ToString() + maxUse.ToString() );

                games.Player.Rest.Disable();
            }
        }
  
    }

    void FixedUpdate()
    {
        KnockBackTimer();
        if (canThrow || canAttack) return;
        direction = games.Player.Move.ReadValue<Vector2>();
        if (direction.sqrMagnitude > 0.01f)
        {
            // Lock to 4 cardinal directions
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                lastFacingDirection = new Vector2(Mathf.Sign(direction.x), 0);
            else
                lastFacingDirection = new Vector2(0, Mathf.Sign(direction.y));
        }


        rig.linearVelocity = direction * _speed * Time.fixedDeltaTime;

        if (!canMove)
        {
            rig.linearVelocity = Vector2.zero;
            _speed = 0;
        }

        PressureDrain();

    }


    private void PressureDrain() // will hold a button to press
    {

       if(isPressureDrained) return;
       
        pressure -= decayRate * Time.fixedDeltaTime;

    
         bool isStandingStill = rig.linearVelocity.magnitude < 0.1f;
         CalculateCriticalPressure();
         if (isStandingStill && isRestoring)
         { 
            if(pressure < maxPressure)
            {
                pressure += regenRate * Time.fixedDeltaTime;
            }
       
        }
       
        pressure = Mathf.Clamp(pressure, 0, maxPressure);
       
    }

    private void CalculateCriticalPressure()
    {
        float pressurePercent = (pressure / maxPressure) * 100f; // calculate percentage
        // AudioSource source = GetComponent<AudioSource>();
          // HANDLE HEALTH DRAIN
        if (pressure <= 0)
        {
            damageTickTimer += Time.fixedDeltaTime;
            if (damageTickTimer >= 2.0f) // Deal damage every 0.5 seconds
            {
                Damage((int)healthDrainRate);
                damageTickTimer = 0;
            }
        }
     
         if (pressurePercent <= 10f)
         {
         heartbeatTimer -= Time.deltaTime;
        
        if (heartbeatTimer <= 0)
        {
            PlayHeartbeat();

            // FIX: Set this to 1.0f (or slightly more) so the 1-second 
            // sound finishes before the next one starts. 
            // This prevents the "stacking" distortion.
            heartbeatTimer = 1.15f; 
        }
            
        //      if (!source.isPlaying)
        // {
        //     source.loop = true; // Make the 8s clip loop automatically
        //     source.Play();
        // }
          
            // Speed up the interval: 0.3s if at 0%, 0.8s if at 5%
           // heartbeatTimer = Mathf.Lerp(0.3f, 0.8f, pressure / (maxPressure * 0.1f));
            
            // Interval: Faster at 0% (0.3s), Slower at 10% (1.0s)
          //  heartbeatTimer = Mathf.Lerp(0.3f, 1.0f, pressure / (maxPressure * 0.1f));
          
            // // Optional: Add a UI Shake trigger here
            // StartCoroutine(ShakeUI(healthText.transform, 0.1f, 0.1f));
        }
        
        else
        {
             heartbeatTimer = 0;
        //     if (source.isPlaying)
        // {
        //     source.Stop();
        // }
        }
    }
    void PlayHeartbeat() // will use the audio manager
{
    AudioSource source = GetComponent<AudioSource>();
    if (source && source.clip != null)
    {
        // Use 0.7f volume to prevent "clipping" when mixed with other game sounds
        source.PlayOneShot(source.clip, 0.7f);
    }
}
    
    private void KnockBackTimer()
    {
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.fixedDeltaTime;

            // If the timer just finished, stop the movement completely
            if (knockbackTimer <= 0) rig.linearVelocity = Vector2.zero;
        }
    }

    void LateUpdate()
    {
         Vector3 camPos = Camera.main.transform.position;
        float minX = camPos.x - screenBounds.x + objectWidth;
        float maxX = camPos.x + screenBounds.x - objectWidth;
    
        float minY = camPos.y - screenBounds.y + objectHeight;
        float maxY = camPos.y + screenBounds.y - objectHeight;

    // 3. Clamp the player's position within these dynamic boundaries
        Vector3 viewPos = transform.position;
        viewPos.x = Mathf.Clamp(viewPos.x, minX, maxX);
        viewPos.y = Mathf.Clamp(viewPos.y, minY, maxY);


       
        transform.position = viewPos;
    }

    public void StopMovement()
    {
        canMove = false;
    }
    public void EnableMovement()
    {
        canMove = true;
    }
    void OnDestroy()
    {
       
        games.Dispose();
    }
    public void EnableCollider(GameObject hitObject)// for attack in the animator
    {
        ///
    }

    public void Damage(int damage)
    {
       hp -= damage;
       if(hp <= 0)
        {
             gameObject.SetActive(false);
            hp = 0;
            pressure = 0f;
            if(GameManager.Instance != null)
            {
                GameManager.Instance.isGameOver = true;
            }
            if(UI_Manager.Instance != null)
            {
                UI_Manager.Instance.OpenLoseScreen();
            }

        }
    }

    public void ApplyKnockback(Vector2 direction, float force)
{
    knockbackTimer = knockbackTotalTime; // Start the timer
    rig.linearVelocity = Vector2.zero;          // Reset current velocity first
    rig.AddForce(direction * force, ForceMode2D.Impulse);
}

// for items to call
 public void RestorePressure(float amount) 
    {
        pressure += amount;
        pressure = Mathf.Clamp(pressure, 0, maxPressure);
    }

    public void OnUseIncreased(int use)
    { 
        totalCount = 0;
        if(games.Player.Rest.enabled == false)
        {
       
        maxUse += use;
        
        games.Player.Rest.Enable();

        }
    }
    public void OnHeal( int amount)
    { 
       hp += amount;
       hp = Mathf.Clamp(hp , 0, maxHp);   
    }
   
    public float GetDamageMultiplier()
    {
        // Example: Base is 1.0. If pressure is < 20, return 2.0 (Double Damage).
        // Otherwise, return 1.0 (Normal Damage).
        return (pressure < 20f) ? 2.0f : 1.0f;
    }
}
