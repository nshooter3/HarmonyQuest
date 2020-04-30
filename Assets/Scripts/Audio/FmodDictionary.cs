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
            //{ "", "" },

            #region Reliqua
            { "twilit_hollow", "event:/music/twilit_hollow/twilit_hollow" },
            { "twilit_hollow_dissonance", "event:/music/twilit_hollow/twilit_hollow_dissonance" },
            { "twilit_hollow_ambience", "event:/music/twilit_hollow/twilit_hollow_ambience" },
            { "crystal_man_fight_pink", "event:/music/crystal_men/crystal_man_fight_pink" },
            #endregion
        };

        //Event name-to-fmod-path dictionary for SFX. Used to generate our fmod event pools.
        public Dictionary<string, string> fmodSFXEventDictionary = new Dictionary<string, string>()
        {
            //{ "", "" },

            #region Global
            { "PLACEHOLDER", "event:/PLACEHOLDER" },
            { "PLACEHOLDER_tonal", "event:/PLACEHOLDER_tonal" },
            #endregion

            #region Melody
            { "melody_attack_fail", "event:/sfx/melody/melody_attack_fail" },
            { "melody_attack_fail_tonal", "event:/sfx/melody/melody_attack_fail_tonal" },
            { "melody_attack_hit", "event:/sfx/melody/melody_attack_hit" },
            { "melody_attack_hit_tonal", "event:/sfx/melody/melody_attack_hit_tonal" },
            { "melody_attack_swing", "event:/sfx/melody/melody_attack_swing" },
            { "melody_damage", "event:/sfx/melody/melody_damage" },
            { "melody_dash", "event:/sfx/melody/melody_dash" },
            { "melody_death", "event:/sfx/melody/melody_death" },
            { "melody_footstep", "event:/sfx/melody/melody_footstep" },
            { "melody_footstep_tonal", "event:/sfx/melody/melody_footstep_tonal" },
            { "melody_harmony_mode_activate", "event:/sfx/melody/melody_harmony_mode_activate" },
            { "melody_harmony_mode_deactivate", "event:/sfx/melody/melody_harmony_mode_deactivate" },
            { "melody_heal", "event:/sfx/melody/melody_heal" },
            { "melody_heal_tonal", "event:/sfx/melody/melody_heal_tonal" },
            { "melody_lockoff", "event:/sfx/melody/melody_lockoff" },
            { "melody_lockon", "event:/sfx/melody/melody_lockon" },
            { "melody_parry_fail", "event:/sfx/melody/melody_parry_fail" },
            { "melody_parry_fail_tonal", "event:/sfx/melody/melody_parry_fail_tonal" },
            { "melody_parry_hit", "event:/sfx/melody/melody_parry_hit" },
            { "melody_parry_hit_tonal", "event:/sfx/melody/melody_parry_hit_tonal" },
            #endregion

            #region GeneralEnemy
            { "enemy_aggro", "event:/sfx/enemy/general_enemy/enemy_aggro" },
            { "enemy_death", "event:/sfx/enemy/general_enemy/enemy_death" },
            { "enemy_death_tonal", "event:/sfx/enemy/general_enemy/enemy_death_tonal" },
            #endregion

            #region FrogKnight
            { "frog_knight_attack_hit_tonal", "event:/sfx/enemy/frog_knight/frog_knight_attack_hit_tonal" },
            { "frog_knight_footstep", "event:/sfx/enemy/frog_knight/frog_knight_footstep" },
            { "frog_knight_hop", "event:/sfx/enemy/frog_knight/frog_knight_hop" },
            { "frog_knight_sword_fail", "event:/sfx/enemy/frog_knight/frog_knight_sword_fail" },
            { "frog_knight_sword_hit", "event:/sfx/enemy/frog_knight/frog_knight_sword_hit" },
            { "frog_knight_tongue_fail", "event:/sfx/enemy/frog_knight/frog_knight_tongue_fail" },
            { "frog_knight_tongue_hit", "event:/sfx/enemy/frog_knight/frog_knight_tongue_hit" },
            #endregion
        };

        //Param name-to-fmod-name dictionary
        public Dictionary<string, string> fmodParamDictionary = new Dictionary<string, string>()
        {
            //{ "", "" },

            #region Global
            {"global_pitch", "global_pitch" },
            {"global_octave", "global_octave" },
            {"global_footstep_surface", "global_footstep_surface" },
            #endregion

            #region Melody
            { "melody_attack_tonal_pitch_param", "global_pitch" },
            { "melody_attack_tonal_octave_param", "global_octave" },
            #endregion
        };
    }
}
