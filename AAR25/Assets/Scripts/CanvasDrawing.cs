using UnityEngine;
using System.Collections.Generic;

public class CanvasDrawing : MonoBehaviour
{
    public Material drawingMaterial;
    private LineRenderer currentLine;
    private bool isDrawing;
    private static List<LineRenderer> allLines = new List<LineRenderer>();
    public static List<LineRenderer> GetLines() => allLines;

    void Start()
    {
        if (drawingMaterial == null)
        {
            drawingMaterial = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            drawingMaterial.color = Color.black;
        }
    }

    public void StartDrawing(Vector3 position)
    {
        isDrawing = true;
        currentLine = new GameObject("Line").AddComponent<LineRenderer>();
        currentLine.material = drawingMaterial;
        currentLine.startWidth = 0.01f;
        currentLine.endWidth = 0.01f;
        currentLine.positionCount = 0;
        allLines.Add(currentLine);
        AddPoint(position);
    }

    public void AddPoint(Vector3 position)
    {
        if (isDrawing)
        {
            currentLine.positionCount++;
            currentLine.SetPosition(currentLine.positionCount - 1, position);
        }
    }

    public void StopDrawing()
    {
        isDrawing = false;
    }

    public void SetColor(Color color)
    {
        drawingMaterial.color = color;
    }

    public static void ClearLines()
    {
        foreach (var line in allLines)
        {
            if (line != null) Destroy(line.gameObject);
        }
        allLines.Clear();
    }
}