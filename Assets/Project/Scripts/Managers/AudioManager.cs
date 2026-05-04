using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Will handle all audio playing and implementation
    /// </summary>
    /// 
    
     public static AudioManager Instance; // Reference when being invoked.
    private static AudioManager _instance;

    [SerializeField] private AudioSource[] sfx;
    [SerializeField] private AudioSource[] bgm;
    int musicIndex;
    [SerializeField] private bool musicPlaying;

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
  
    // Update is called once per frame
    void Update()
    {
        if (!musicPlaying)
        {
            StopAllBGM();
            StopSFXAll();
        }
        else
        {
            if (!bgm[musicIndex].isPlaying)
            {
                PlayBGM(musicIndex);
            }
        }



    }

    public void PlayBGM(int music)
    {
        musicIndex = music;
        StopAllBGM();
        bgm[music].Play();

    }

    private void StopSFXAll()
    {
        for (int i = 0; i < sfx.Length; i++)
        {
            if (sfx[i].isPlaying)
            {
                sfx[i].Stop();
            }

        }

    }

    private void StopAllBGM()
    {
        for (int j = 0; j < bgm.Length; j++)
        {
            bgm[j].Stop();
        }
    }

    public void PlaySFX(int index)
    {
        musicIndex = index;
        if (index < sfx.Length)
            sfx[index].PlayOneShot(sfx[index].clip);
    }
    public void StopPlayingSFX(int index)
    {
        musicIndex = index;
        sfx[musicIndex].Stop();
    }


}
