// using UnityEngine;
// using System.Collections.Generic;

// public class DrawingPersistence : MonoBehaviour
// {
//     private static Dictionary<string, List<Vector3[]>> sceneDrawings = new Dictionary<string, List<Vector3[]>>();
//     private static Dictionary<string, List<Color>> sceneColors = new Dictionary<string, List<Color>>();

//     public static void SaveDrawing(string sceneName, List<LineRenderer> lines)
//     {
//         List<Vector3[]> points = new List<Vector3[]>();
//         List<Color> colors = new List<Color>();
//         foreach (var line in lines)
//         {
//             Vector3[] linePoints = new Vector3[line.positionCount];
//             line.GetPositions(linePoints);
//             points.Add(linePoints);
//             colors.Add(line.material.color);
//         }
//         sceneDrawings[sceneName] = points;
//         sceneColors[sceneName] = colors;
//     }

//     public static void LoadDrawing(string sceneName, CanvasDrawing canvas)
//     {
//         if (sceneDrawings.ContainsKey(sceneName))
//         {
//             var pointsList = sceneDrawings[sceneName];
//             var colorsList = sceneColors[sceneName];
//             for (int i = 0; i < pointsList.Count; i++)
//             {
//                 canvas.SetColor(colorsList[i]);
//                 canvas.StartDrawing(pointsList[i][0]);
//                 for (int j = 1; j < pointsList[i].Length; j++)
//                 {
//                     canvas.AddPoint(pointsList[i][j]);
//                 }
//                 canvas.StopDrawing();
//             }
//         }
//     }
// }