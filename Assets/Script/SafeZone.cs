using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public PlayerHealth playerHealth;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ghost"))
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1); 
            }

            Destroy(other.gameObject); 
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
