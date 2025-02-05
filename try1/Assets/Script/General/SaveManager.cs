// using System.Collections.Generic;
// using System.IO;
// using System.Security.Cryptography;
// using System.Text;
// using UnityEngine;

// public static class SaveManager
// {
//     private static readonly string saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");

//     private class SaveFileWrapper
//     {
//         public string hash;      // Hash for validation
//         public string jsonData;  // Save data as JSON
//     }

//     public static void SaveGame(PlayerSaveData saveData)
//     {
//         try
//         {
//             string jsonData = JsonUtility.ToJson(saveData, true);
//             string hash = GenerateHash(jsonData);

//             SaveFileWrapper wrapper = new SaveFileWrapper
//             {
//                 hash = hash,
//                 jsonData = jsonData
//             };

//             string wrappedJson = JsonUtility.ToJson(wrapper, true);
//             File.WriteAllText(saveFilePath, wrappedJson);

//             Debug.Log($"Game saved successfully at: {saveFilePath}");
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"Failed to save game: {ex.Message}");
//         }
//     }

//     public static PlayerSaveData LoadGame(CardDatabase cardDatabase = null)
//     {
//         if (!File.Exists(saveFilePath))
//         {
//             Debug.LogWarning("Save file not found.");
//             return null;
//         }

//         try
//         {
//             string wrappedJson = File.ReadAllText(saveFilePath);
//             SaveFileWrapper wrapper = JsonUtility.FromJson<SaveFileWrapper>(wrappedJson);

//             if (!IsHashValid(wrapper.jsonData, wrapper.hash))
//             {
//                 Debug.LogError("Save file is corrupted or has been tampered with.");
//                 return null;
//             }

//             PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(wrapper.jsonData);
//             Debug.Log("Game loaded successfully.");
//             return saveData;
//         }
//         catch (System.Exception ex)
//         {
//             Debug.LogError($"Failed to load game: {ex.Message}");
//             return null;
//         }
//     }

//     public static void DeleteSave()
//     {
//         if (File.Exists(saveFilePath))
//         {
//             try
//             {
//                 File.Delete(saveFilePath);
//                 Debug.Log("Save file deleted.");
//             }
//             catch (System.Exception ex)
//             {
//                 Debug.LogError($"Failed to delete save file: {ex.Message}");
//             }
//         }
//         else
//         {
//             Debug.LogWarning("No save file to delete.");
//         }
//     }

//     private static string GenerateHash(string input)
//     {
//         using (SHA256 sha256 = SHA256.Create())
//         {
//             byte[] bytes = Encoding.UTF8.GetBytes(input);
//             byte[] hash = sha256.ComputeHash(bytes);
//             return System.BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
//         }
//     }

//     private static bool IsHashValid(string data, string hash)
//     {
//         string computedHash = GenerateHash(data);
//         return computedHash == hash;
//     }

//     public static string GetSaveFilePath() => saveFilePath;
// }

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
