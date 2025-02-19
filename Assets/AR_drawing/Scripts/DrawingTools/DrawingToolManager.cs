using System.Collections.Generic;
using UnityEngine;

public class DrawingToolManager : MonoBehaviour
{
    public static DrawingToolManager Instance;
    public List<DrawingTool> DrawingTools;
    public int selectedTool = 0;


    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }


    public DrawingTool GetNextTool(bool getNextTool)
    {
        //Increment the index and return the element
        selectedTool = getNextTool ? selectedTool++ : selectedTool--;
        if (selectedTool >= DrawingTools.Count) selectedTool = 0;
        else if (selectedTool < 0) selectedTool = DrawingTools.Count - 1;

        return DrawingTools[selectedTool];
    }
}
