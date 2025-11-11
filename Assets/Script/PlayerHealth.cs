using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3; 
    private int currentHealth;

    public float invincibilityDuration = 1f; 
    private bool isInvincible = false;

    private Animator animator;

    private GameManager gameManager;
    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); 
        gameManager = GameManager.instance;
    }

    void Update()
    {

    }

    public void TakeDamage(int damageAmount)
    {
        if (isInvincible) return;

        currentHealth -= damageAmount;
        Debug.Log("Player took " + damageAmount + " damage. Current Health: " + currentHealth);


        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    void Die()
    {
        Debug.Log("Player has died!");
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }

        if (gameManager != null)
        {
            gameManager.GameOver(); 
        }
        Destroy(gameObject, 2f);
    }
    
    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
