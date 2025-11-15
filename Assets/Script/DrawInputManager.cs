using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class DrawInputManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    private List<Vector2> points;
    private Camera mainCamera;
    private Animator playerAnimator;

    void Start()
    {
        mainCamera = Camera.main;
        points = new List<Vector2>();
        
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 0;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerAnimator = player.GetComponent<Animator>();
        }
    }

    void Update()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.leftButton.wasPressedThisFrame)
        {
            StartDrawing();
        }

        if (mouse.leftButton.isPressed)
        {
            Vector2 screenPosition = mouse.position.ReadValue();
            ContinueDrawing(screenPosition);
        }

        if (mouse.leftButton.wasReleasedThisFrame)
        {
            StopDrawing();
        }
    }

    void StartDrawing()
    {
        points.Clear();
        lineRenderer.positionCount = 0;
    }

    void ContinueDrawing(Vector2 screenPosition)
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(screenPosition);
        
        if (points.Count == 0 || Vector2.Distance(points[points.Count - 1], mousePos) > 0.1f)
        {
            points.Add(mousePos);
            UpdateLine();
        }
    }

    void StopDrawing()
    {
        if (points.Count > 5) 
        {
            int recognizedIndex = RecognizeShape(points);
            Debug.Log("Shape Recognized Index: " + recognizedIndex);

            if (recognizedIndex != -1)
            {
                TriggerPlayerAttack();
                BroadcastSymbol(recognizedIndex); 
            }
        }
        
        points.Clear();
        lineRenderer.positionCount = 0;
    }

    void UpdateLine()
    {
        lineRenderer.positionCount = points.Count;
        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    void BroadcastSymbol(int symbolIndex)
    {
        Ghost[] allGhosts = FindObjectsByType<Ghost>(FindObjectsSortMode.None);
        foreach (Ghost ghost in allGhosts)
        {
            ghost.ProcessDrawnSymbol(symbolIndex);
        }
    }

    private int RecognizeShape(List<Vector2> points)
    {

        Vector2 start = points[0];
        Vector2 end = points[points.Count - 1];
        float minX = points.Min(p => p.x);
        float maxX = points.Max(p => p.x);
        float minY = points.Min(p => p.y);
        float maxY = points.Max(p => p.y);
        float width = maxX - minX;
        float height = maxY - minY;
        
        float size = Mathf.Max(width, height); 
        float aspectRatio = width / (height + 0.001f); 


        if (aspectRatio < 0.3f && height > size * 0.7f) return 0;


        if (aspectRatio > 3.0f && width > size * 0.7f) return 1;


        float startEndDist = Vector2.Distance(start, end);
        if (aspectRatio > 0.7f && aspectRatio < 1.3f && startEndDist < size * 0.3f)
        {
            return 5;
        }
        

        List<Vector2> peaks = new List<Vector2>();
        List<Vector2> valleys = new List<Vector2>();
        for (int i = 1; i < points.Count - 1; i++)
        {
            if (points[i].y > points[i-1].y && points[i].y > points[i+1].y) peaks.Add(points[i]);
            if (points[i].y < points[i-1].y && points[i].y < points[i+1].y) valleys.Add(points[i]);
        }

        if (valleys.Count >= 2 && peaks.Count >= 1)
        {
            return 4;
        }


        Vector2 lowestPoint = points.OrderBy(p => p.y).First();
        Vector2 highestPoint = points.OrderByDescending(p => p.y).First();

        bool isApex(Vector2 apex) { return apex != start && apex != end; }


        if (isApex(highestPoint) && start.y < highestPoint.y && end.y < highestPoint.y)
        {
            return 3;
        }

        if (isApex(lowestPoint) && start.y > lowestPoint.y && end.y > lowestPoint.y)
        {
            return 2;
        }
        
        return -1; 
    }

    void TriggerPlayerAttack()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Attack");
        }
    }
}