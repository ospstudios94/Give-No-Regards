using UnityEngine;

public class RestoreUseitem : MonoBehaviour
{
    public int increaseUse = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().OnUseIncreased(increaseUse);
            Destroy(gameObject);
        }

    
        // if(collision.CompareTag("Player"))
        // {
        //     collision.GetComponent<PlayerController>().OnUseRestored();
        //     Destroy(gameObject);
        // }
    
    }


}
