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
    }
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
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
