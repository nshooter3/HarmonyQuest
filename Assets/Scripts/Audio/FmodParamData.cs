namespace HarmonyQuest.Audio
{
    public class FmodParamData
    {
        public string paramName;
        public float paramValue;

        public FmodParamData(string paramName, float paramValue)
        {
            this.paramName = paramName;
            this.paramValue = paramValue;
        }
    }
}
