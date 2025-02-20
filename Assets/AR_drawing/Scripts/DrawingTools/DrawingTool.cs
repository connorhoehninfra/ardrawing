using UnityEngine;

[CreateAssetMenu(fileName = "NewDrawingTool", menuName = "DrawingTool")]
public class DrawingTool : ScriptableObject
{
    public string Name;
    public GameObject Prefab;
    public bool overridePrefabSettings;
    public Color InkColor;
    public float Thickness;
    public float Hardness;
}
