namespace Saving
{
    using UnityEngine;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.IO;

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

        /// <summary>
        /// Initializes save file with default SaveData values. This function can be called to clear an existing save.
        /// </summary>
        /// <param name="fileNum"> Which file slot to initialize. </param>
        public static void Init(int fileNum)
        {
            bf = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + "/saveData" + fileNum + ".dat");
            SaveData initSaveData = new SaveData();
            initSaveData.currentSaveFileActive = fileNum;
            bf.Serialize(file, initSaveData);
            file.Close();
        }

        /// <summary>
        /// Saves variables in the SaveData class to a file.
        /// </summary>
        /// <param name="fileNum"> Which file slot to save to. </param>
        public static void Save(int fileNum)
        {
            bf = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + "/saveData" + fileNum + ".dat");
            bf.Serialize(file, saveData);
            file.Close();
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
        public static void Load(int fileNum)
        {
            //Create empty save file at this slot if one does not exist
            if (!DoesSaveFileExist(fileNum))
            {
                Init(fileNum);
            }
            bf = new BinaryFormatter();
            file = File.Open(Application.persistentDataPath + "/saveData" + fileNum + ".dat", FileMode.Open);
            saveData = (SaveData)bf.Deserialize(file);
            file.Close();
        }
    }
}
