namespace HarmonyQuest.Audio
{
    using GameManager;

    public class FmodMusicInstructions : ManageableObject
    {
        public string musicEventName;
        public float musicVolume;

        public string ambienceEventName;
        public float ambienceVolume;

        public override void OnAwake()
        {
            if (musicEventName != "")
            {
                if (musicEventName != FmodFacade.instance.GetMusicEventName())
                {
                    FmodFacade.instance.StopMusic();
                    FmodFacade.instance.StartMusic(musicEventName, musicVolume);
                }
            }
            else
            {
                FmodFacade.instance.StopMusic();
            }

            if (ambienceEventName != "")
            {
                if (ambienceEventName != FmodFacade.instance.GetAmbienceEventName())
                {
                    FmodFacade.instance.StopAmbience();
                    FmodFacade.instance.StartAmbience(ambienceEventName, ambienceVolume);
                }
            }
            else
            {
                FmodFacade.instance.StopAmbience();
            }
        }
    }
}
