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

        private float maxGrappleDistance;

        private UITracker grappleReticule;
        private Image grappleOnImage;

        private float highestScore;
        private float score;

        private float distance;
        private float distanceScore;
        private float minDistanceScore;
        private float distanceScoreWeight = 0.6f;

        private float angle;
        private float angleScore;
        private float angleScoreWeight = 0.4f;

        private float maxGrappleAngle;

        public MelodyGrappleHook(MelodyController controller)
        {
            this.controller = controller;
            
            grappleReticule = ServiceLocator.instance.GetUIManager().grappleReticule;
            grappleOnImage = ServiceLocator.instance.GetUIManager().grappleImage;
            maxGrappleDistance = controller.config.maxGrappleDistance;
            maxGrappleAngle = controller.config.maxGrappleAngle;
            grappleOnImage.enabled = false;

            FindGrapplePoints();
        }

        public void OnUpdate(float deltaTime)
        {
            SelectOptimalGrapplePoint();
            ToggleGrappleUI();
        }

        public bool HasGrappleDestination()
        {
            return grappleDestination != null;
        }

        public GrapplePoint GetGrappleDestination()
        {
            return grappleDestination;
        }

        private void FindGrapplePoints()
        {
            grapplePoints = Object.FindObjectsOfType<GrapplePoint>();
        }

        private void SelectOptimalGrapplePoint()
        {
            highestScore = 0f;
            score = 0f;

            angle = 0f;
            angleScore = 0f;

            distance = 0;
            distanceScore = 0f;
            minDistanceScore = float.MaxValue;

            grappleDestination = null;

            foreach (GrapplePoint grapplePoint in grapplePoints)
            {
                if (grapplePoint.active && grapplePoint.IsCooldownActive() == false && !IsGrapplePointObstructed(grapplePoint.actualDestination.position) && !IsPlayerOutOfGrappleAngleRange(grapplePoint))
                {
                    distance = Vector3.Distance(controller.transform.position, grapplePoint.actualDestination.position);
                    if (distance > maxGrappleDistance)
                    {
                        //Do nothing
                    }
                    else
                    {
                        distanceScore = (Mathf.Max(maxGrappleDistance - distance, 0f) / maxGrappleDistance) * distanceScoreWeight;

                        //Calculate the angle of the target by getting the angle between the target position relative to the player, and the direction the player is facing.
                        Vector3 sourceDirection = grapplePoint.actualDestination.position - controller.transform.position;
                        angle = Vector3.Angle(controller.transform.forward, sourceDirection);

                        if (angle > maxGrappleAngle)
                        {
                            //Do nothing
                        }
                        else
                        {
                            angleScore = ((maxGrappleAngle - angle) / maxGrappleAngle) * angleScoreWeight;

                            score = angleScore + distanceScore;

                            if (score > highestScore)
                            {
                                highestScore = score;
                                grappleDestination = grapplePoint;
                            }
                        }
                    }
                }
            }
        }

        private void ToggleGrappleUI()
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

        private bool IsGrapplePointObstructed(Vector3 destination)
        {
            return Physics.Linecast(controller.top.position,    destination, controller.config.grappleAttemptLayerMask) || 
                   Physics.Linecast(controller.center.position, destination, controller.config.grappleAttemptLayerMask);
        }

        //Check to see if the player's position relative to the grapple point is within 90 degrees of the grapple point's normal
        //This prevents the player from doing things like grappling up to a cliff, then turning around and grappling up to the edge they're already standing on.
        private bool IsPlayerOutOfGrappleAngleRange(GrapplePoint grapplePoint)
        {
            if (!grapplePoint.canGrappleFromAbove && controller.transform.position.y >= grapplePoint.transform.position.y)
            {
                return true;
            }
            if (grapplePoint.IsGrappleAngleConstricted())
            {
                return Vector3.Angle(grapplePoint.visibleDestination.forward, controller.center.position - grapplePoint.visibleDestination.position) > 90f;
            }
            return false;
        }
    }
}
