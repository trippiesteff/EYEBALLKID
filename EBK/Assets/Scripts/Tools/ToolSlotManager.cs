using System.Collections.Generic;
using UnityEngine;

public class ToolSlotManager : MonoBehaviour, IDataPersistence
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

    private ToolItemData GetToolById(string toolId)
    {
        foreach (var tool in allTools)
        {
            if (tool != null && tool.toolId == toolId)
                return tool;
        }

        return null;
    }

    public void LoadData(GameData data)
    {
        if (data == null)
            return;

        ownedTools.Clear();
        equippedTool = null;

        foreach (string toolId in data.ownedToolIds)
        {
            ToolItemData tool = GetToolById(toolId);

            if (tool != null)
            {
                AddTool(tool);
            }
        }

        if (!string.IsNullOrEmpty(data.equippedToolId))
        {
            ToolItemData tool = GetToolById(data.equippedToolId);

            if (tool != null)
            {
                EquipTool(tool);
            }
        }

        if (equippedTool == null && ownedTools.Count > 0)
        {
            equippedTool = ownedTools[0];
        }
    }

    public void SaveData(GameData data)
    {
        data.ownedToolIds.Clear();

        foreach (ToolItemData tool in ownedTools)
        {
            if (tool != null)
            {
                data.ownedToolIds.Add(tool.toolId);
            }
        }

        data.equippedToolId = equippedTool != null ? equippedTool.toolId : "";
    }
}
