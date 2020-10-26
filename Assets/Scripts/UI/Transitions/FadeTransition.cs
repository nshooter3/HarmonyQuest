namespace UI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class FadeTransition : UITransition
    {
        public Image fadeImage;
        private Color fadedColor, color;

        private float fadeInTimer, maxFadeInTimer = 1.0f;
        private float fadeOutTimer, maxFadeOutTimer = 0.5f;

        public override void OnAwake()
        {
            fadedColor = fadeImage.color;
            fadedColor.a = 0f;

            color = fadeImage.color;
            color.a = 1f;
        }

        public override void TransitionIntro()
        {
            fadeInTimer = maxFadeInTimer;
        }

        public override void TransitionOutro()
        {
            fadeOutTimer = maxFadeOutTimer;
        }

        public override void OnUpdate()
        {
            if (fadeInTimer > 0f)
            {
                fadeInTimer = Mathf.Max(fadeInTimer - Time.deltaTime, 0f);
                fadeImage.color = Color.Lerp(fadedColor, color, 1 - fadeInTimer/maxFadeInTimer);
                if (fadeInTimer <= 0f)
                {
                    UITransitionManager.instance.TransitionToNewScene();
                }
            }
            else if (fadeOutTimer > 0f)
            {
                fadeOutTimer = Mathf.Max(fadeOutTimer - Time.deltaTime, 0f);
                fadeImage.color = Color.Lerp(color, fadedColor, 1 - fadeOutTimer / maxFadeOutTimer);
                if (fadeOutTimer <= 0f)
                {
                    UITransitionManager.instance.FinishTransition();
                }
            }
        }

        public override void ResetTransition()
        {
            base.ResetTransition();
            fadeInTimer = 0f;
            fadeOutTimer = 0f;
        }
    }
}
