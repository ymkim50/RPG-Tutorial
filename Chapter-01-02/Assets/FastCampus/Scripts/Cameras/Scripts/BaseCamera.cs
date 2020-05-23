using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastCampus.Cameras
{
    public class BaseCamera : MonoBehaviour
    {
        #region Variables
        public Transform target;
        #endregion

        #region Main Methods
        // Start is called before the first frame update
        void Start()
        {
            HandleCamera();
        }

        private void LateUpdate()
        {
            HandleCamera();
        }
        #endregion

        #region Helper Methods
        public virtual void HandleCamera()
        {
        }
        #endregion
    }
}