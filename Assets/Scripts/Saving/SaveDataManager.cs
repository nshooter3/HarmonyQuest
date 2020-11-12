namespace Saving
{
    using UnityEngine;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;
    using System;

    /// <summary>
    /// Handles saving and loading for all of our files, and references the class holding the active save file's data.
    /// </summary>
    public static class SaveDataManager
    {
        /// <summary>
        /// Public reference to our active save data class. Access it through the SaveDataManager instance.
        /// </summary>
        public static SaveData saveData;

        private static BinaryFormatter bf;
        private static FileStream file;

        public static bool saveLoaded = false;

        /// <summary>
        /// Initializes save file with default SaveData values. This function can be called to clear an existing save.
        /// </summary>
        /// <param name="fileNum"> Which file slot to initialize. </param>
        /// <param name="errorMessage"> If this function fails, this message will contain the error message </param>
        public static bool Init(int fileNum, out string errorMessage)
        {
            bool functionCompleted = true;
            errorMessage = "";
            try
            {
                //using keyword ensures that file gets disposed of even if exceptions are thrown, ensuring that the file isn't left open.
                using (file = File.Create(Application.persistentDataPath + "/saveData" + fileNum + ".dat"))
                {
                    bf = new BinaryFormatter();
                    SaveData initSaveData = new SaveData();
                    initSaveData.currentSaveFileActive = fileNum;
                    bf.Serialize(file, initSaveData);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                errorMessage = "Error initializing file " + fileNum + "\n" + e.Message + "\n" + e.StackTrace;
                functionCompleted = false;
            }
            return functionCompleted;
        }

        /// <summary>
        /// Saves variables in the SaveData class to a file.
        /// </summary>
        /// <param name="fileNum"> Which file slot to save to. </param>
        /// <param name="errorMessage"> If this function fails, this message will contain the error message </param>
        public static bool Save(int fileNum, out string errorMessage)
        {
            bool functionCompleted = true;
            errorMessage = "";

            if (saveData == null)
            {
                errorMessage = "Error saving file " + fileNum + ". saveData is null, meaning no save data has been created yet.";
                functionCompleted = false;
                return functionCompleted;
            }

            try
            {
                //using keyword ensures that file gets disposed of even if exceptions are thrown, ensuring that the file isn't left open.
                using (file = File.Create(Application.persistentDataPath + "/saveData" + fileNum + ".dat"))
                {
                    bf = new BinaryFormatter();
                    bf.Serialize(file, saveData);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                errorMessage = "Error saving file " + fileNum + "\n" + e.Message + "\n" + e.StackTrace;
                functionCompleted = false;
            }
            return functionCompleted;
        }

        /// <summary>
        /// Returns whether or not a save file exist at the specified file number.
        /// </summary>
        /// <param name="fileNum"> The file number for the save file we check for </param>
        /// <returns></returns>
        public static bool DoesSaveFileExist(int fileNum)
        {
            return File.Exists(Application.persistentDataPath + "/saveData" + fileNum + ".dat");
        }

        /// <summary>
        /// Loads variables to the SaveData class from a file.
        /// </summary>
        /// <param name="fileNum"> Which file slot to load </param>
        /// <param name="errorMessage"> If this function fails, this message will contain the error message </param>
        public static bool Load(int fileNum, out string errorMessage)
        {
            bool functionCompleted = true;
            errorMessage = "";
            //Create empty save file at this slot if one does not exist
            if (!DoesSaveFileExist(fileNum))
            {
                if (Init(fileNum, out errorMessage) == false)
                {
                    errorMessage = "Could not initialize new save in when loading file " + fileNum + " due to following error: " + errorMessage;
                    functionCompleted = false;
                    return functionCompleted;
                }
            }
            try
            {
                //using keyword ensures that file gets disposed of even if exceptions are thrown, ensuring that the file isn't left open.
                using (file = File.Open(Application.persistentDataPath + "/saveData" + fileNum + ".dat", FileMode.Open))
                {
                    bf = new BinaryFormatter();
                    saveData = (SaveData)bf.Deserialize(file);
                    file.Close();
                    saveLoaded = true;
                }
            }
            catch (Exception e)
            {
                errorMessage = "Error loading file " + fileNum + "\n" + e.Message + "\n" + e.StackTrace;
                functionCompleted = false;
            }
            return functionCompleted;
        }
    }
}
