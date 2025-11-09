using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class Ghost : MonoBehaviour
{
    private Queue<string> symbolQueue = new Queue<string>();
    private string[] availableSymbols = { "|", "<", ">", "^", "V" };
    public int maxComboLength = 3;
    public TextMeshPro symbolText;
    public float moveSpeed = 2f;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        GenerateRandomSymbols();
        UpdateSymbolDisplay();
    }
    void Update()
    {
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    public void DestroyGhost()
    {
        Destroy(gameObject);
    }
    
    private void GenerateRandomSymbols()
    {
        int comboLength = Random.Range(1, maxComboLength + 1);

        for (int i = 0; i < comboLength; i++)
        {
            string randomSymbol = availableSymbols[Random.Range(0, availableSymbols.Length)];
            
            symbolQueue.Enqueue(randomSymbol);
        }
    }

    private void UpdateSymbolDisplay()
    {
        if (symbolText != null)
        {
            symbolText.text = string.Join(" ", symbolQueue);
        }
    }


    public void ProcessDrawnSymbol(string drawnSymbol)
    {
        if (symbolQueue.Count == 0) return;

        if (drawnSymbol == symbolQueue.Peek())
        {
            symbolQueue.Dequeue();

            UpdateSymbolDisplay();

            if (symbolQueue.Count == 0)
            {
                Die();
            }
        }
        else
        {
            Debug.Log("Vẽ sai ký hiệu!");
        }
    }
    
    private void Die()
    {
        moveSpeed = 0; 
        
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false; 
        }
        
        animator.SetTrigger("isDie"); 
    }
}
