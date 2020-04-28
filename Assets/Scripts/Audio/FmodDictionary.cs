namespace HarmonyQuest.Audio
{
    using System.Collections.Generic;

    /// <summary>
    /// Class that allows us to access fmod event and param names by our own defined keys. This way if fmod event names or paths change, we need only update the FmodNameDictionary script.
    /// </summary>
    public class FmodDictionary
    {
        //Event name-to-fmod-path dictionary for Music.
        public Dictionary<string, string> fmodMusicEventDictionary = new Dictionary<string, string>()
        {
            #region Reliqua
            { "twilit_hollow", "event:/music/twilit_hollow/twilit_hollow" },
            { "twilit_hollow_dissonance", "event:/music/twilit_hollow/twilit_hollow_dissonance" },
            { "cmf_pink_dissonance", "event:/music/crystal_men/referenced_events/cmf_pink_dissonance" },
            { "cmf_pink_dissonance_idle", "event:/music/crystal_men/referenced_events/cmf_pink_dissonance_idle" },
            { "cmf_pink_hit", "event:/music/crystal_men/referenced_events/cmf_pink_hit" },
            { "cmf_pink_intro", "event:/music/crystal_men/referenced_events/cmf_pink_intro" },
            { "cmf_pink_phase1", "event:/music/crystal_men/referenced_events/cmf_pink_phase1" },
            { "cmf_pink_phase1_idle", "event:/music/crystal_men/referenced_events/cmf_pink_phase1_idle" },
            #endregion
        };

        //Event name-to-fmod-path dictionary for SFX. Used to generate our fmod event pools.
        public Dictionary<string, string> fmodSFXEventDictionary = new Dictionary<string, string>()
        {
            #region Global
            { "PLACEHOLDER", "event:/PLACEHOLDER" },
            { "PLACEHOLDER_tonal", "event:/PLACEHOLDER_tonal" },
            #endregion

            #region Melody
            { "melody_attack_hit", "event:/sfx/melody/melody_attack_hit" },
            { "melody_attack_tonal", "event:/sfx/melody/melody_attack_tonal" },
            { "melody_parry_tonal", "event:/sfx/melody/melody_parry_tonal" },
            { "melody_heal", "event:/sfx/melody/melody_heal" },
            #endregion

            #region Enemy
            { "prototype_pill_attack_charge", "event:/sfx/enemy/prototype_pill/prototype_pill_attack_charge" },
            { "prototype_pill_attack_tonal", "event:/sfx/enemy/prototype_pill/prototype_pill_attack_tonal" },
            { "protoype_pill_attack_hit", "event:/sfx/enemy/prototype_pill/protoype_pill_attack_hit" },
            { "protoype_pill_attack_swing", "event:/sfx/enemy/prototype_pill/protoype_pill_attack_swing" },
            #endregion
        };

        //Param name-to-fmod-name dictionary
        public Dictionary<string, string> fmodParamDictionary = new Dictionary<string, string>()
        {
            #region Global
            {"global_pitch", "global_pitch" },
            {"global_octave", "global_octave" },
            #endregion

            #region Melody
            { "melody_attack_hit_param", "global_melody_attack_hit" },
            { "melody_attack_tonal_pitch_param", "global_pitch" },
            { "melody_attack_tonal_octave_param", "global_octave" },
            { "melody_heal_param", "global_melody_heal" },
            #endregion
        };

        //Enums for param values
        public enum melody_attack_hit_param_val { missed_attack = 1, good_hit = 2, great_hit = 3, parry = 4 };
        public enum melody_heal_param_val { small = 1, medium = 2, large = 3 };
    }
}
