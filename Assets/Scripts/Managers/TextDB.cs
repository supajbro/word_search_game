using System.Collections.Generic;
using UnityEngine;

public static class TextDB
{
    private static Dictionary<string, string> textLookup = new Dictionary<string, string>();
    private static bool loaded = false;

    public static void Load()
    {
        if (loaded)
            return;

        TextAsset file = Resources.Load<TextAsset>("words");

        string[] lines = file.text.Split('\n');

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split('=');

            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();

                textLookup[key] = value;
            }
        }

        loaded = true;
    }

    public static string Get(string key)
    {
        if (!loaded)
            Load();

        if (textLookup.TryGetValue(key, out string value))
            return value;

        return key;
    }
}