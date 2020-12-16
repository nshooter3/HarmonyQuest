using UnityEngine;

public static class PauseManager
{
    private static bool isPaused = false;
    private static bool isDialog = false;

    public delegate void OnPause();
    private static OnPause onPause;

    public delegate void OnUnpause();
    private static OnUnpause onUnpause;

    public delegate void OnDialog();
    private static OnDialog onDialog;

    public delegate void OnDialogEnd();
    private static OnDialogEnd onDialogEnd;

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

    public static void ToggleDialog(bool isDialog)
    {
        if (isDialog != PauseManager.isDialog)
        {
            PauseManager.isDialog = isDialog;
            if (isDialog && onDialog != null)
            {
                onDialog();
            }
            else if (onDialogEnd != null)
            {
                onDialogEnd();
            }
        }
    }

    public static bool GetPaused()
    {
        return isPaused;
    }

    public static bool GetDialog()
    {
        return isDialog;
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

    public static void AssignFunctionToOnDialogDelegate(OnDialog func)
    {
        onDialog += func;
    }

    public static void RemoveFunctionFromOnDialogDelegate(OnDialog func)
    {
        onDialog -= func;
    }

    public static void ClearOnDialogDelegate()
    {
        onDialog = null;
    }

    public static void AssignFunctionToOnDialogEndDelegate(OnDialogEnd func)
    {
        onDialogEnd += func;
    }

    public static void RemoveFunctionFromOnDialogEndDelegate(OnDialogEnd func)
    {
        onDialogEnd -= func;
    }

    public static void ClearOnDialogEndDelegate()
    {
        onDialogEnd = null;
    }
}
