using UnityEngine;

public static class PauseManager
{
    private static bool isPaused = false;
    private static bool isCutscene = false;

    public delegate void OnPause();
    private static OnPause onPause;

    public delegate void OnUnpause();
    private static OnUnpause onUnpause;

    public delegate void OnCutscene();
    private static OnCutscene onCutscene;

    public delegate void OnCutsceneEnd();
    private static OnCutsceneEnd onCutsceneEnd;

    public static void TogglePaused(bool isPaused)
    {
        if (isPaused != PauseManager.isPaused)
        {
            PauseManager.isPaused = isPaused;
            if (isPaused)
            {
                if (onPause != null)
                {
                    onPause();
                }
            }
            else
            {
                if (onUnpause != null)
                {
                    onUnpause();
                }
            }
        }
    }

    public static void ToggleCutscene(bool isCutscene)
    {
        if (isCutscene != PauseManager.isCutscene)
        {
            PauseManager.isCutscene = isCutscene;
            if (isCutscene && onCutscene != null)
            {
                onCutscene();
            }
            else if (onCutsceneEnd != null)
            {
                onCutsceneEnd();
            }
        }
    }

    public static bool GetPaused()
    {
        return isPaused;
    }

    public static bool GetCutscene()
    {
        return isCutscene;
    }

    public static void AssignFunctionToOnPauseDelegate(OnPause func)
    {
        onPause += func;
    }

    public static void RemoveFunctionFromOnPauseDelegate(OnPause func)
    {
        onPause -= func;
    }

    public static void ClearOnPauseDelegate()
    {
        onPause = null;
    }

    public static void AssignFunctionToOnUnpauseDelegate(OnUnpause func)
    {
        onUnpause += func;
    }

    public static void RemoveFunctionFromOnUnpauseDelegate(OnUnpause func)
    {
        onUnpause -= func;
    }

    public static void ClearOnUnpauseDelegate()
    {
        onUnpause = null;
    }

    public static void AssignFunctionToOnCutsceneDelegate(OnCutscene func)
    {
        onCutscene += func;
    }

    public static void RemoveFunctionFromOnCutsceneDelegate(OnCutscene func)
    {
        onCutscene -= func;
    }

    public static void ClearOnCutsceneDelegate()
    {
        onCutscene = null;
    }

    public static void AssignFunctionToOnCutsceneEndDelegate(OnCutsceneEnd func)
    {
        onCutsceneEnd += func;
    }

    public static void RemoveFunctionFromOnCutsceneEndDelegate(OnCutsceneEnd func)
    {
        onCutsceneEnd -= func;
    }

    public static void ClearOnCutsceneEndDelegate()
    {
        onCutsceneEnd = null;
    }
}
