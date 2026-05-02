using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour
{
    /// <summary>
    /// This will hold all things UI. 
    /// Opening/Closing all menus
    /// </summary>
    public static UI_Manager Instance; // Reference when being invoked.
    private static UI_Manager _instance;

    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject inventoryMenu;
    public GameObject winScreen;
    public GameObject loseScreen;

    public bool isPaused = false;

 
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


       
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        inventoryMenu.SetActive(false);
       
    }

    // Update is called once per frame
    void Update()
    {
        if(isPaused)
        {
           GameManager.Instance.SetTimeScale(0);
           OpenPauseMenu();
            // stop player Movement here
        }
        else if(!isPaused)
        {
          GameManager.Instance.SetTimeScale(1);
          CloseMenu();
            // enable player movement here
        }
    }

    public void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);
        optionsMenu.SetActive(false);
        isPaused = true;
    }

    public void CloseMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        isPaused = false;
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        isPaused = true;
    }

#region PAUSE MENU BUTTONS
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ResumeButton()
    {
        CloseMenu();
    }
    public void OpenOptions()
    {
        optionsMenu.SetActive(true);
        pauseMenu.SetActive(false);
       isPaused = true;
    }
    
#endregion

#region OPTIONS MENU BUTTONS
    public void OptionsBackButton()
    {
        CloseOptionsMenu();
    }
    public void SaveSettings()
    {
        PlayerPrefs.Save();
        // save settings here with player Prefs
        // will be part of the Save Menu if I want to add accessibliity
    }
    public void ResetAllToDefault()
    {
       // set all to Zero.
       // save it here.
    }

#endregion

#region WIN/LOSE
    public void OpenWinScreen()
    {
        isPaused = true;
        winScreen.SetActive(true);
        loseScreen.SetActive(false);
        // make win screen active
    }
    public void OpenLoseScreen()
    {
        isPaused = true; 
        loseScreen.SetActive(true);
        winScreen.SetActive(false);
       
        // make lose screen active
    }
#endregion
   
}
