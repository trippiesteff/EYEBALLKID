using System.Collections.Generic;
using UnityEngine;

public class ToolSlotManager : MonoBehaviour
{
    [SerializeField] private List<ToolItemData> allTools = new();

    private readonly List<ToolItemData> ownedTools = new();
    private ToolItemData equippedTool;

    private void Awake()
    {
        InitializeTools();
    }

    private void InitializeTools()
    {
        ownedTools.Clear();
        equippedTool = null;

        foreach (var tool in allTools)
        {
            if (tool == null)
                continue;

            if (tool.ownedByDefault)
            {
                AddTool(tool);

                if (tool.equippedByDefault && equippedTool == null)
                    equippedTool = tool;
            }
        }

        if (equippedTool == null && ownedTools.Count > 0)
            equippedTool = ownedTools[0];
    }

    public void AddTool(ToolItemData tool)
    {
        if (tool == null)
            return;

        if (ownedTools.Contains(tool))
            return;

        ownedTools.Add(tool);
    }

    public bool HasTool(ToolItemData tool)
    {
        if (tool == null)
            return false;

        return ownedTools.Contains(tool);
    }

    public bool EquipTool(ToolItemData tool)
    {
        if (tool == null)
            return false;

        if (HasTool(tool) == false)
            return false;

        equippedTool = tool;
        return true;
    }

    public ToolItemData GetEquippedTool()
    {
        return equippedTool;
    }

    public List<ToolItemData> GetOwnedTools()
    {
        return ownedTools;
    }
}
