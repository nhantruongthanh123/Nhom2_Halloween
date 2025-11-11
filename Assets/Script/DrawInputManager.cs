using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Thư viện Input System
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
            string recognizedShape = RecognizeShape(points);

            if (recognizedShape != "unknown")
            {
                TriggerPlayerAttack();
                BroadcastSymbol(recognizedShape);
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

    void BroadcastSymbol(string symbol)
    {
        Ghost[] allGhosts = FindObjectsByType<Ghost>(FindObjectsSortMode.None);
        foreach (Ghost ghost in allGhosts)
        {
            ghost.ProcessDrawnSymbol(symbol);
        }
    }

    private string RecognizeShape(List<Vector2> points)
    {
        Vector2 start = points[0];
        Vector2 end = points[points.Count - 1];
        float minX = points.Min(p => p.x);
        float maxX = points.Max(p => p.x);
        float minY = points.Min(p => p.y);
        float maxY = points.Max(p => p.y);
        float width = maxX - minX;
        float height = maxY - minY;

        float aspectRatio = width / height;
        if (aspectRatio < 0.3f) return "|";
        if (aspectRatio > 3.0f) return "-";

        Vector2 lowestPoint = points.OrderBy(p => p.y).First();
        Vector2 highestPoint = points.OrderByDescending(p => p.y).First();
        Vector2 leftmostPoint = points.OrderBy(p => p.x).First();
        Vector2 rightmostPoint = points.OrderByDescending(p => p.x).First();

        bool isApex(Vector2 apex)
        {
            return apex != start && apex != end;
        }

        if (isApex(highestPoint) && start.y < highestPoint.y && end.y < highestPoint.y)
        {
            return "^";
        }

        if (isApex(lowestPoint) && start.y > lowestPoint.y && end.y > lowestPoint.y)
        {
            return "V";
        }

        if (isApex(leftmostPoint) && start.x > leftmostPoint.x && end.x > leftmostPoint.x)
        {
            return "<";
        }

        if (isApex(rightmostPoint) && start.x < rightmostPoint.x && end.x < rightmostPoint.x)
        {
            return ">";
        }

        return "unknown";
    }
    

    void TriggerPlayerAttack()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Attack");
        }
    }
}