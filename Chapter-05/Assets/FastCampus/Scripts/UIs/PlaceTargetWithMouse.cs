using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastCampus.SceneUtils
{
    public class PlaceTargetWithMouse : MonoBehaviour
    {
        #region Variables
        public float surfaceOffset = 1.5f;
        public Transform target = null;

        #endregion Variables
        // Update is called once per frame
        void Update()
        {
            if (target)
            {
                transform.position = target.position + Vector3.up * surfaceOffset;
            }
            //if (!Input.GetMouseButtonDown(0))
            //{
            //    return;
            //}

            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit hit;
            //if (!Physics.Raycast(ray, out hit))
            //{
            //    return;
            //}

            //transform.position = hit.point + hit.normal * surfaceOffset;
        }

        public void SetPosition(RaycastHit hit)
        {
            target = null;
            transform.position = hit.point + hit.normal * surfaceOffset;
        }
    }

}