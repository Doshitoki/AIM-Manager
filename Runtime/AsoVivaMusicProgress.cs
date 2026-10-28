using System.Collections.Generic;

public static class AsoVivaMusicProgress
{
    private static HashSet<string> unlockedLayers = new HashSet<string>()
    {
        "Base"
    };

    public static void Unlock(string layerName)
    {
        unlockedLayers.Add(layerName);
    }

    public static void Lock(string layerName)
    {
        if (layerName == "Base") return;
        unlockedLayers.Remove(layerName);
    }

    public static bool IsUnlocked(string layerName)
    {
        return unlockedLayers.Contains(layerName);
    }

    public static void Reset()
    {
        unlockedLayers.Clear();
        unlockedLayers.Add("Base");
    }
}