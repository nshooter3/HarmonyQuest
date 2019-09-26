namespace HarmonyQuest.DynamicCamera.Editor
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    // using BezierCurves;

    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor
    {
        private CameraController camera;
        private SerializedProperty curve;

        protected virtual void OnEnable()
        {
            this.camera = (CameraController) this.target;

            curve = serializedObject.FindProperty("curve");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.curve);

            // BezierCurve3D curve = (BezierCurve3D) EditorGUI.ObjectField(new Rect(3, 3, 6, 20), "Find Bezier Curve", this.curve, typeof(BezierCurve3D), true);
        }
    }
}