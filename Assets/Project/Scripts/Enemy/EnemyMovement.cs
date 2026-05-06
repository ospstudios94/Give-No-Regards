using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    // this is for Skeleton Enemy, trigger to walk is the same.. 
    // for others, even the chasing and moving around worked.. 
    [SerializeField] private Transform m_Target;
    [SerializeField] float chaseRange = 3;
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float _speed;

    [SerializeField] float _patrolRange = 5f;

    private float attackTime;
    [SerializeField] float attackRate = 1f;

    private float distance = 0.0f;
    private Vector2 direction;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Animator _anim;
    float hInput;
    float vInput;


    [SerializeField]
    private float defaultSpeed = 2f;

    Enemy enems;
    bool isMoving = true;

    void Awake()
    {
          _anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        m_Target = FindAnyObjectByType<PlayerController>().transform;
        enems = GetComponent<Enemy>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       

      
        defaultSpeed = _speed;
    }

    // Update is called once per frame


    private void FixedUpdate()
    {
        if (enems != null && enems.knockbackTimer > 0)
    {
        return; // EXIT and do nothing. Let the physics force move the rig.
    }
        if(isMoving)
        {
            Movement();
        }
        else
        {
            body.linearVelocity = Vector2.zero;
            _speed = 0;
        }
      
        
        
    }

    private void Movement()
    {
       
       
         direction = (m_Target.position - transform.position).normalized;

        body.linearVelocity = _speed * Time.fixedDeltaTime *direction;

        distance = Vector3.Distance(transform.position, m_Target.transform.position); // general movement

        if (distance <= chaseRange)
        { 
            // moving to movement
            Vector3 temp = Vector3.MoveTowards(transform.position, m_Target.position, _speed * Time.fixedDeltaTime);
            _anim.SetBool("Walk", true);
            ChangeAnimation(temp - transform.position);
            body.MovePosition(temp);
        
            CanMove();

            if (distance <= attackRange)
            {
                // attack state
                StopMoving();
                _anim.SetBool("Walk", false);
                if (Time.time - attackTime > attackRate)
                {
                    attackTime = Time.time;
                    _anim.SetTrigger("Attack");
                    StopMoving();
                }
            }

        }
        else
        {
            // idle
            CanMove();
            SetAnimFloat(transform.position);
            _anim.SetBool("Walk", false);
        }
    }

    void SetAnimFloat(Vector2 setVector) // for animation
    {
        _anim.SetFloat("moveX", setVector.x);
        _anim.SetFloat("moveY", setVector.y);

        if (setVector.x != 0 || setVector.y != 0)
        {
            _anim.SetFloat("lastX", setVector.x);
            _anim.SetFloat("lastY", setVector.y);

        }


    }
    void ChangeAnimation(Vector2 dir) // for directional movement
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            if (dir.x > 0)
            {
                SetAnimFloat(Vector2.right);
            }
            else if (dir.x < 0)
            {
                SetAnimFloat(Vector2.left);
            }
        }

        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y > 0)
            {
                SetAnimFloat(Vector2.up);
            }
            else if (dir.y < 0)
            {
                SetAnimFloat(Vector2.down);
            }
        }

    }


    public void StopMoving()
    {
        isMoving = false;
        _speed = 0;
    }

    public void CanMove()
    {
        isMoving = true;
        _speed = defaultSpeed;


    }
    
}
