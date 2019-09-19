namespace HarmonyQuest.Effects.OilPaint
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;

    public class OilPaintPostProcessing : PostProcessEffectSettings
    {
        [Range(1f, 9f)]
        public FloatParameter radius = new FloatParameter() { value = 5.0f };
        [Range(0f, 1.25f)]
        public FloatParameter distance = new FloatParameter() { value = 1.0f };
        [Range(0f, 0.5f)]
        public FloatParameter thickness = new FloatParameter() { value = 0.5f };
    }

    public class OilPaintPostProcessingRenderer<T> : PostProcessEffectRenderer<T> where T : OilPaintPostProcessing
    {
        private int RobertsCrossDepthNormalsPass = 0;
        private int _RadiusID, _DistanceID, _ThicknessID;

        private Shader shader;

        public override void Init()
        {
            base.Init();
            shader = Shader.Find("Hidden/OilPaintPostProcess");
            _RadiusID = Shader.PropertyToID("_Radius");
            _DistanceID = Shader.PropertyToID("_Distance");
            _ThicknessID = Shader.PropertyToID("_Thickness");
        }

        public override void Render(PostProcessRenderContext context)
        {
            if (settings == null)
                return;

            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(_RadiusID, settings.radius);
            sheet.properties.SetFloat(_DistanceID, settings.distance);
            sheet.properties.SetFloat(_ThicknessID, settings.thickness);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, RobertsCrossDepthNormalsPass);
        }

        public override DepthTextureMode GetCameraFlags()
        {
            if (settings == null)
            return DepthTextureMode.None;

            return DepthTextureMode.DepthNormals;
            // return base.GetCameraFlags();
        }
    }

    [System.Serializable]
    [PostProcess(typeof(OilPaintPostProcessingRenderer), PostProcessEvent.AfterStack, "Harmony Quest/Oil Paint")]
    public sealed class OilPaint : OilPaintPostProcessing { }

    public sealed class OilPaintPostProcessingRenderer : OilPaintPostProcessingRenderer<OilPaintPostProcessing> { }
}
