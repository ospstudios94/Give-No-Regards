using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// This will handle the win conditions, game changes, and storage of data
    /// Become a Singleton.. The load manager will be on this game object as well.
    /// </summary>
    /// 
    public static GameManager Instance; // Reference when being invoked.
    private static GameManager _instance;

    public bool isGameOver = false;

    PlayerController player;

   // make some event here.. but it does not have to be perfect.



    void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        _instance = this;
        Instance = this;

        SetTimeScale(1);
        player = FindAnyObjectByType<PlayerController>();
    }
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGameOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.hp <=0)
        {
            isGameOver = true;
        }
       if(isGameOver)
        {
            SetTimeScale(0);

           
        }
    }

 public void SetTimeScale(float scale)
    {
        float newScale = Mathf.Clamp01(scale);
        Time.timeScale = newScale;
    }


void CheckCondition()
    {
        // if the player won or not.
    }
}
