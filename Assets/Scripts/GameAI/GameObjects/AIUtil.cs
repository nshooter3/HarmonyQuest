namespace GameAI.AIGameObjects
{
    using UnityEngine;
    using HarmonyQuest;

    /// <summary>
    /// Utility class for miscellaneous AI Agent functions that don't belong in the other AI gameobject subclasses.
    /// </summary>
    public class AIUtil
    {
        private AIGameObjectData data;

        private Plane[] cameraPlanes;
        private Camera cam;

        public void Init(AIGameObjectData data)
        {
            this.data = data;
            cam = ServiceLocator.instance.GetCamera();
        }

        //Check if our agent is on screen by seeing if any of their renderers are within the camera's bounds.
        public bool IsAgentWithinCameraBounds()
        {
            cameraPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
            foreach (Renderer renderer in data.renderers)
            {
                if (GeometryUtility.TestPlanesAABB(cameraPlanes, renderer.bounds))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
