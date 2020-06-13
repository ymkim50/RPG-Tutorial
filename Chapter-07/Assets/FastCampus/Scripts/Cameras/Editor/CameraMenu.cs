using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace FastCampus.Cameras
{
    public class CameraMenu : MonoBehaviour
    {
        [MenuItem("FastCampus/Cameras/Top Down Camera")]
        public static void CreateTopDownCamera()
        {
            GameObject[] selectedGOs = Selection.gameObjects;
            //foreach (var selected in selectedGOs)
            //{
            //    Debug.Log(selected.name);
            //}


            if (selectedGOs.Length < 1)
            {
                EditorUtility.DisplayDialog("Camera Tool",
                    "You need to select a GameObject in the Scene that has a Camera component assigned to it!",
                    "OK");
                return;
            }

            if (selectedGOs.Length > 0)
            {
                GameObject cameraGO = selectedGOs.First(selected => selected.GetComponent<Camera>());
                if (cameraGO == null)
                {
                    EditorUtility.DisplayDialog("Camera Tool",
                        "You need to select a GameObject in the Scene that has a Camera component assigned to it!",
                        "OK");
                    return;
                }

                if (selectedGOs.Length == 1)
                {
                    AttachTopDownScript(cameraGO);
                }
                else if (selectedGOs.Length == 2)
                {
                    AttachTopDownScript(cameraGO, selectedGOs[0] == cameraGO ? selectedGOs[1].transform : selectedGOs[0].transform);
                }
                else
                {
                    EditorUtility.DisplayDialog("Camera Tools",
                        "You can only select two GameObjects in the scene for this to work and the first selection needs to be camera!",
                        "OK");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Camera Tool",
                    "You need to select a GameObject in the Scene that has a Camera component assigned to it!",
                    "OK");
            }


        }

        static void AttachTopDownScript(GameObject camera, Transform target = null)
        {
            // Assign top down script to the camera
            TopDownCamera cameraScript = null;
            if (camera)
            {
                cameraScript = camera.AddComponent<TopDownCamera>();

                // check to see if we have a Target and we have a scrip reference
                if (cameraScript && target)
                {
                    cameraScript.target = target;
                }

                Selection.activeGameObject = camera;
            }
        }
    }

}