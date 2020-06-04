using GameAI;
using UnityEngine;

namespace Melody
{
    public interface IMelodyInfo
    {
        Transform GetTransform();

        Vector3 GetTransformForward();

        MelodyConfig GetConfig();

        AIAgent GetLockonTarget();

        bool HasLockonTarget();

        float GetCurrentHealth();

        float GetMaxHealth();

        MelodySound GetMelodySound();
    }
}
