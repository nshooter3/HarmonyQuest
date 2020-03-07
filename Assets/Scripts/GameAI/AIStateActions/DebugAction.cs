﻿namespace GameAI.AIStateActions
{
    using GameAI.Navigation;
    using GameAI.StateHandlers;
    using UnityEngine;

    public class DebugAction
    {
        public void NavPosTrackTarget(AIStateUpdateData updateData)
        {
            Vector3 newNavPos = updateData.aiGameObjectFacade.data.aggroTarget.position;
            newNavPos.y += NavigatorSettings.navPosHeightOffset;
            updateData.aiGameObjectFacade.data.navPos.transform.position = newNavPos;
        }

        public void NavPosSetPosition(AIStateUpdateData updateData, Vector3 position)
        {
            updateData.aiGameObjectFacade.data.navPos.transform.position = position;
        }
    }
}
