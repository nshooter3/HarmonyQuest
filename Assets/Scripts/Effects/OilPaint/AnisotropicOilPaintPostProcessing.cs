namespace HarmonyQuest.Effects.OilPaint
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;

    public class AnisotropicOilPaintPostProcessing : PostProcessEffectSettings
    {
        [Range(1f, 9f)]
        public FloatParameter radius = new FloatParameter() { value = 5.0f };
        [Range(0f, 1.25f)]
        public FloatParameter distance = new FloatParameter() { value = 1.0f };
        [Range(0f, 0.5f)]
        public FloatParameter thickness = new FloatParameter() { value = 0.5f };
        [Range(0f, 5f)]
        public FloatParameter alpha = new FloatParameter() { value = 1.0f };
        [Range(0f, 5f)]
        public FloatParameter q = new FloatParameter() { value = 1.0f };
        public TextureParameter k0123 = new TextureParameter() {};
    }

    public class AnisotropicOilPaintPostProcessingRenderer<T> : PostProcessEffectRenderer<T> where T : AnisotropicOilPaintPostProcessing
    {
        private int RobertsCrossDepthNormalsPass = 0;
        private int _RadiusID, _DistanceID, _ThicknessID, _AlphaID, _QID, _K0123ID;

        private Shader shader;

        public override void Init()
        {
            base.Init();
            shader = Shader.Find("Hidden/AnisotropicOilPaintPostProcess");
            _RadiusID = Shader.PropertyToID("_Radius");
            _DistanceID = Shader.PropertyToID("_Distance");
            _ThicknessID = Shader.PropertyToID("_Thickness");
            _AlphaID = Shader.PropertyToID("_Alpha");
            _QID = Shader.PropertyToID("_Q");
            _K0123ID = Shader.PropertyToID("_K0123");
        }

        public override void Render(PostProcessRenderContext context)
        {
            if (settings == null)
                return;

            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(_RadiusID, settings.radius);
            sheet.properties.SetFloat(_DistanceID, settings.distance);
            sheet.properties.SetFloat(_ThicknessID, settings.thickness);
            sheet.properties.SetFloat(_AlphaID, settings.alpha);
            sheet.properties.SetFloat(_QID, settings.q);
            if (settings.k0123 != null)
                sheet.properties.SetTexture(_K0123ID, settings.k0123);

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
    [PostProcess(typeof(AnisotropicOilPaintPostProcessingRenderer), PostProcessEvent.AfterStack, "Harmony Quest/Anisotropic Oil Paint")]
    public sealed class AnisotropicOilPaint : AnisotropicOilPaintPostProcessing { }

    public sealed class AnisotropicOilPaintPostProcessingRenderer : AnisotropicOilPaintPostProcessingRenderer<AnisotropicOilPaintPostProcessing> { }
}
