namespace Melody
{
    using HarmonyQuest;
    using Objects;
    using UI;
    using UnityEngine;
    using UnityEngine.UI;

    public class MelodyGrappleHook
    {
        private MelodyController controller;
        private GrapplePoint[] grapplePoints;
        private GrapplePoint grappleDestination;

        private float grappleDistance;
        private float maxGrappleDistance;

        private UITracker grappleReticule;
        private Image grappleOnImage;

        private float curGrappleScore;
        private float minGrappleScore;

        public MelodyGrappleHook(MelodyController controller)
        {
            this.controller = controller;
            
            grappleReticule = ServiceLocator.instance.GetUIManager().grappleReticule;
            grappleOnImage = ServiceLocator.instance.GetUIManager().grappleImage;
            maxGrappleDistance = controller.config.maxGrappleDistance;
            grappleOnImage.enabled = false;

            FindGrapplePoints();
        }

        public void OnUpdate(float deltaTime)
        {
            SelectOptimalGrapplePoint();
            ToggleGrappleUI();
        }

        public void FindGrapplePoints()
        {
            grapplePoints = Object.FindObjectsOfType<GrapplePoint>();
        }

        public void SelectOptimalGrapplePoint()
        {
            curGrappleScore = 0f;
            minGrappleScore = float.MaxValue;
            grappleDestination = null;

            foreach (GrapplePoint grapplePoint in grapplePoints)
            {
                curGrappleScore = GetGrapplePointScore(grapplePoint);
                if (curGrappleScore > 0f && curGrappleScore < minGrappleScore)
                {
                    minGrappleScore = curGrappleScore;
                    grappleDestination = grapplePoint;
                }
            }
        }

        public void ToggleGrappleUI()
        {
            if (grappleDestination != null)
            {
                grappleReticule.SetTarget(grappleDestination.transform);
                grappleOnImage.enabled = true;
            }
            else
            {
                grappleOnImage.enabled = false;
            }
        }

        public float GetGrapplePointScore (GrapplePoint grapplePoint)
        {
            if (!IsGrapplePointObstructed(grapplePoint.actualDestination.position))
            {
                grappleDistance = Vector3.Distance(controller.transform.position, grapplePoint.actualDestination.position);
                if (grappleDistance <= maxGrappleDistance)
                {
                    return grappleDistance;
                }
            }
            return -1f;
        }

        public bool IsGrapplePointObstructed(Vector3 destination)
        {
            return Physics.Linecast(controller.top.position,    destination, controller.config.grappleAttemptLayerMask) || 
                   Physics.Linecast(controller.center.position, destination, controller.config.grappleAttemptLayerMask);
        }
    }
}
