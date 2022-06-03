using UnityEditor;
using UnityEngine;

namespace FastCampus.Cameras
{
    [CustomEditor(typeof(TopDownCamera))]
    public class TopDownCamera_Editor : Editor
    {
        #region Variables
        private TopDownCamera targetCamera;
        #endregion

        #region Main Methods
        //private void OnEnable()
        //{
        //}

        public override void OnInspectorGUI()
        {
            targetCamera = (TopDownCamera)target;

            base.OnInspectorGUI();
        }

        private void OnSceneGUI()
        {
            // Make sure we have a target first!
            if (!targetCamera || !targetCamera.target)
            {
                return;
            }

            // Storing target reference
            Transform camTarget = targetCamera.target;
            Vector3 targetPosition = camTarget.position;
            targetPosition.y += targetCamera.lookAtHeight;

            // Draw distance circle
            Handles.color = new Color(1f, 0f, 0f, 0.15f);
            Handles.DrawSolidDisc(targetPosition, Vector3.up, targetCamera.distance);

            Handles.color = new Color(0f, 1f, 0f, 0.75f);
            Handles.DrawWireDisc(targetPosition, Vector3.up, targetCamera.distance);

            // Create slider handles to adjust camera properties
            Handles.color = new Color(1f, 0f, 0f, 0.5f);
            targetCamera.distance = Handles.ScaleSlider(targetCamera.distance, targetPosition, -camTarget.forward, Quaternion.identity, targetCamera.distance, 0.1f);
            targetCamera.distance = Mathf.Clamp(targetCamera.distance, 2f, float.MaxValue);

            Handles.color = new Color(0f, 0f, 1f, 0.5f);
            targetCamera.height = Handles.ScaleSlider(targetCamera.height, targetPosition, Vector3.up, Quaternion.identity, targetCamera.height, 0.1f);
            targetCamera.height = Mathf.Clamp(targetCamera.height, 2f, float.MaxValue);

            // Create Labels
            GUIStyle labelStyle = new GUIStyle();
            labelStyle.fontSize = 15;
            labelStyle.normal.textColor = Color.white;
            labelStyle.alignment = TextAnchor.UpperCenter;

            Handles.Label(targetPosition + (-camTarget.forward * targetCamera.distance), "Distance", labelStyle);

            labelStyle.alignment = TextAnchor.MiddleRight;
            Handles.Label(targetPosition + (Vector3.up * targetCamera.height), "Height", labelStyle);

            targetCamera.HandleCamera();
        }

        #endregion
    }
}