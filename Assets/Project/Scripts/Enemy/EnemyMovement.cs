using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
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



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hInput = body.linearVelocity.x;
        vInput = body.linearVelocity.y;

        _anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        m_Target = FindAnyObjectByType<PlayerController>().transform;
        defaultSpeed = _speed;
    }

    // Update is called once per frame


    private void FixedUpdate()
    {

        Movement();
    }

    private void Movement()
    {


        body.linearVelocity = _speed * Time.fixedDeltaTime * new Vector3(hInput, vInput, 0f).normalized;
        //body.MovePosition(body.position + direction * _speed * Time.deltaTime);

        distance = Vector3.Distance(transform.position, m_Target.transform.position);

        if (distance <= chaseRange)
        {
            Vector3 temp = Vector3.MoveTowards(transform.position, m_Target.position, _speed * Time.fixedDeltaTime);
            _anim.SetBool("Walk", true);
            ChangeAnimation(temp - transform.position);
            body.MovePosition(temp);

            CanMove();

            if (distance <= attackRange)
            {
                StopMoving();
                _anim.SetBool("Walk", false);
                if (Time.time - attackTime > attackRate)
                {
                    attackTime = Time.time;
                    _anim.SetTrigger("isHit");
                }
            }

        }
        else
        {

            CanMove();
            SetAnimFloat(transform.position);
            _anim.SetBool("Walk", false);
        }
    }

    void SetAnimFloat(Vector2 setVector)
    {
        _anim.SetFloat("moveX", setVector.x);
        _anim.SetFloat("moveY", setVector.y);

        if (setVector.x != 0 || setVector.y != 0)
        {
            _anim.SetFloat("lastX", setVector.x);
            _anim.SetFloat("lastY", setVector.y);

        }


    }
    void ChangeAnimation(Vector2 dir)
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
        _speed = 0;
    }

    public void CanMove()
    {

        _speed = defaultSpeed;


    }

}
