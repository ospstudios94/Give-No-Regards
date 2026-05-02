using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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

    [Header("Attack Attributes")]
    [SerializeField]private float attackRate = 1f;
    [SerializeField]GameObject[] hitColliders; // for activating/Deactivating




    
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
       
        Rigidbody2D newRig = bulletObj.GetComponent<Rigidbody2D>();
        newRig.AddForce(lastFacingDirection * 1000, ForceMode2D.Impulse);

       // add directionals and animation here
      
       yield return wait;
    
       canThrow = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(canRun)
        {
            _speed = _initialSpeed * runMultiplier;
        }
        else
        {
            _speed = _initialSpeed;
        }
    }

    void FixedUpdate()
    {
        if(canThrow || canAttack) return;
        direction = games.Player.Move.ReadValue<Vector2>();
          if (direction.sqrMagnitude > 0.01f)
        {
            // Lock to 4 cardinal directions
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                lastFacingDirection = new Vector2(Mathf.Sign(direction.x), 0);
            else
                lastFacingDirection = new Vector2(0, Mathf.Sign(direction.y));
        }

        
    rig.linearVelocity = direction * _speed * Time.deltaTime;
        
        if(!canMove)
        {
            rig.linearVelocity = Vector2.zero;
            _speed = 0;
        }

       
        // add boundaries here

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
}
