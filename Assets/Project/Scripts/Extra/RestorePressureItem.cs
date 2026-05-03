using UnityEngine;

public class RestorePressureItem : MonoBehaviour
{
    public float restoreAmount = 10;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().RestorePressure(restoreAmount);
            Destroy(gameObject);
        }
    }
}
