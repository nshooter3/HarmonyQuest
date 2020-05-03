﻿using GameAI;
using UnityEngine;

namespace Melody
{
    public interface IMelodyInfo
    {
        Transform GetTransform();

        MelodyConfig GetConfig();

        AIAgent GetLockonTarget();

        bool HasLockonTarget();

        float GetCurrentHealth();

        float GetMaxHealth();
    }
}
