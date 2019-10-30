namespace Saving
{
    /// <summary>
    /// Put the variables you need to save here. Separate different categories by region for organization.
    /// Values assigned here will be the defaults used when creating a new save file.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        //Player data
        #region
        public string playerName = "";
        public int playerHealth = 0;
        public int playerHarmonyMeterCharge = 0;
        public int playerHealCharges = 0;
        public int money = 0;
        #endregion

        //System data
        #region
        // Which save file is active. If this param is set to -1, we have not loaded a save file yet.
        public int currentSaveFileActive = -1;
        public string currentScene = "";
        #endregion
    }
}
