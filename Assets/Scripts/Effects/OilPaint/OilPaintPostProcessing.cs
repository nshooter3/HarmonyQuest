namespace HarmonyQuest.Effects.OilPaint
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;

    public class OilPaintPostProcessing : PostProcessEffectSettings
    {
            public enum Target { ViewSpace, WorldSpace }

    [System.Serializable]
    public sealed class TargetParameter : ParameterOverride<Target> {}

    public TargetParameter target = new TargetParameter();
        [Range(1f, 9f)]
        public FloatParameter radius = new FloatParameter() { value = 5.0f };
        [Range(0f, 15f)]
        public FloatParameter distance = new FloatParameter() { value = 1.0f };
        [Range(0f, 1f)]
        public FloatParameter thickness = new FloatParameter() { value = 0.5f };
        public Vector3Parameter point = new Vector3Parameter();
    }

    public class OilPaintPostProcessingRenderer<T> : PostProcessEffectRenderer<T> where T : OilPaintPostProcessing
    {
        private int RobertsCrossDepthNormalsPass = 1;
        private int _RadiusID, _DistanceID, _ThicknessID, _InverseViewID, _PointID;

        private Shader shader;

        public override void Init()
        {
            base.Init();
            shader = Shader.Find("Hidden/OilPaintPostProcess");
            _RadiusID = Shader.PropertyToID("_Radius");
            _DistanceID = Shader.PropertyToID("_Distance");
            _ThicknessID = Shader.PropertyToID("_Thickness");
            _InverseViewID = Shader.PropertyToID("_InverseView");
            _PointID = Shader.PropertyToID("_Point");
        }

        public override void Render(PostProcessRenderContext context)
        {
            var cmd = context.command;
            cmd.BeginSample("Oil Paint");
            var sheet = context.propertySheets.Get(shader);

            sheet.properties.SetFloat(_RadiusID, settings.radius);
            sheet.properties.SetFloat(_DistanceID, settings.distance);
            sheet.properties.SetFloat(_ThicknessID, settings.thickness);
            sheet.properties.SetMatrix(_InverseViewID, context.camera.cameraToWorldMatrix);
            sheet.properties.SetVector(_PointID, settings.point);

            var pass = (int)settings.target.value;
            cmd.BlitFullscreenTriangle(context.source, context.destination, sheet, pass);

            cmd.EndSample("Oil Paint");
        }

        public override DepthTextureMode GetCameraFlags()
        {
            // if (settings == null)
            // return DepthTextureMode.None;

            return DepthTextureMode.Depth;
            // return base.GetCameraFlags();
        }
    }

    [System.Serializable]
    [PostProcess(typeof(OilPaintPostProcessingRenderer), PostProcessEvent.AfterStack, "Harmony Quest/Oil Paint")]
    public sealed class OilPaint : OilPaintPostProcessing { }

    public sealed class OilPaintPostProcessingRenderer : OilPaintPostProcessingRenderer<OilPaintPostProcessing> { }
}
