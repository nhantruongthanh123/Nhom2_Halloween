using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class SymbolSpriteMapping
{
    public string symbolName;
    public Sprite symbolSprite;
    public Color symbolColor = Color.white;
}

public class Ghost : MonoBehaviour
{
    private Queue<int> symbolQueue = new Queue<int>(); 
    public int maxComboLength = 3;
    public float moveSpeed = 2f;
    private Animator animator;
    private float timeAlive = 0f;

    [Header("Sprite Symbol Settings")]
    public GameObject symbolPrefab; 
    public Transform symbolContainer;
    public SymbolSpriteMapping[] symbolDatabase;

    // THÊM BIẾN NÀY: Khoảng cách giữa các ký hiệu
    public float symbolSpacing = 0.8f; 

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
        if (moveSpeed > 0)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            timeAlive += Time.deltaTime;
        }
    }

    private void GenerateRandomSymbols()
    {
        int comboLength = Random.Range(1, maxComboLength + 1);
        for (int i = 0; i < comboLength; i++)
        {
            int randomIndex = Random.Range(0, symbolDatabase.Length);
            symbolQueue.Enqueue(randomIndex);
        }
    }

    // === HÀM NÀY ĐÃ ĐƯỢC NÂNG CẤP ĐỂ XẾP HÀNG ===
    private void UpdateSymbolDisplay()
    {
        // Xóa icon cũ
        foreach (Transform child in symbolContainer)
        {
            Destroy(child.gameObject);
        }

        int iconCount = symbolQueue.Count;
        if (iconCount == 0) return; // Không có gì để vẽ

        // Tính toán vị trí bắt đầu để căn giữa
        float totalWidth = (iconCount - 1) * symbolSpacing;
        float startOffset = -totalWidth / 2f;
        float currentOffset = startOffset;

        // Tạo icon mới
        foreach (int symbolIndex in symbolQueue)
        {
            var mapping = symbolDatabase[symbolIndex];
            
            GameObject iconGO = Instantiate(symbolPrefab, symbolContainer);
            SpriteRenderer sr = iconGO.GetComponent<SpriteRenderer>();

            sr.sprite = mapping.symbolSprite;
            sr.color = mapping.symbolColor;
            
            // Đặt vị trí cục bộ (local) của icon
            iconGO.transform.localPosition = new Vector3(currentOffset, 0, 0);
            
            // Tăng offset cho icon tiếp theo
            currentOffset += symbolSpacing;
        }
    }
    
    public void ProcessDrawnSymbol(int drawnIndex)
    {
        // ... (Hàm này giữ nguyên) ...
        if (symbolQueue.Count == 0) return; 
        int expectedIndex = symbolQueue.Peek();
        if (drawnIndex == expectedIndex)
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
        // ... (Hàm này giữ nguyên) ...
        GameManager.instance.ReportKill(timeAlive); 
        moveSpeed = 0;
        if (GetComponent<Collider2D>() != null)
        {
            GetComponent<Collider2D>().enabled = false;
        }
        if (symbolContainer != null) symbolContainer.gameObject.SetActive(false);
        animator.SetTrigger("isDie");
    }

    public void DestroyGhost()
    {
        Destroy(gameObject);
    }
}