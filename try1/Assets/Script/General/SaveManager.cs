using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class SaveManager
{
    private static readonly string saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");

    private class SaveFileWrapper
    {
        public string hash;
        public string jsonData;
    }

    public static void SaveGame(PlayerSaveData saveData)
    {
        try
        {
            string jsonData = JsonUtility.ToJson(saveData, true);
            SaveFileWrapper wrapper = new() { hash = GenerateHash(jsonData), jsonData = jsonData };
            File.WriteAllText(saveFilePath, JsonUtility.ToJson(wrapper, true));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save game: {ex.Message}");
        }
    }

    public static PlayerSaveData LoadGame(CardDatabase cardDatabase = null)
    {
        if (!File.Exists(saveFilePath)) return null;

        try
        {
            SaveFileWrapper wrapper = JsonUtility.FromJson<SaveFileWrapper>(File.ReadAllText(saveFilePath));
            return IsHashValid(wrapper.jsonData, wrapper.hash) ? JsonUtility.FromJson<PlayerSaveData>(wrapper.jsonData) : null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load game: {ex.Message}");
            return null;
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            try { File.Delete(saveFilePath); }
            catch (System.Exception ex) { Debug.LogError($"Failed to delete save file: {ex.Message}"); }
        }
    }

    private static string GenerateHash(string input)
    {
        using SHA256 sha256 = SHA256.Create();
        return System.BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(input))).Replace("-", "").ToLowerInvariant();
    }

    private static bool IsHashValid(string data, string hash) => GenerateHash(data) == hash;

    public static string GetSaveFilePath() => saveFilePath;
}
