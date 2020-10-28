using GameAI;
using UnityEngine;

namespace Melody
{
    public interface IMelodyInfo
    {
        Transform GetTransform();

        Vector3 GetCenter();

        Vector3 GetTransformForward();

        MelodyConfig GetConfig();

        AIAgent GetLockonTarget();

        bool HasLockonTarget();

		float GetCurrentHealth();

        float GetMaxHealth();

        MelodySound GetMelodySound();

		Vector3 GetVelocity();    }
}
