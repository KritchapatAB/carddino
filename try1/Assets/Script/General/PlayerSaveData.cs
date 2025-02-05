using System.Collections.Generic;
using System;

[System.Serializable]
public class PlayerSaveData
{
    public List<int> playerDeckIds;
    public int money;
    public int currentStage;
    public bool isSaveValid;

    public List<string> lastStageNames = new List<string>(); 
}