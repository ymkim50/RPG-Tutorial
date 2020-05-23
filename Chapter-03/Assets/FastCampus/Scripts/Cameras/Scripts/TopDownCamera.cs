using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace FastCampus.Cameras
{
    public class TopDownCamera : BaseCamera
    {
        #region Variables

        public float height = 5f;
        public float distance = 10f;
        public float angle = 45f;
        public float lookAtHeight = 2f;
        public float smoothSpeed = 0.5f;


        private Vector3 refVelocity;
        #endregion

        #region Main Methods
        #endregion

        #region Helper Methods
        [SerializeField]
        public override void HandleCamera()
        {
            if (!target)
            {
                return;
            }

            // Build world position vector
            Vector3 worldPosition = (Vector3.forward * -distance) + (Vector3.up * height);
            //Debug.DrawLine(m_Target.position, worldPosition, Color.red);

            // Build our Rotated vector
            Vector3 rotatedVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPosition;
            //Debug.DrawLine(m_Target.position, rotatedVector, Color.green);

            // Move our position
            Vector3 flatTargetPosition = target.position;
            flatTargetPosition.y += lookAtHeight;

            Vector3 finalPosition = flatTargetPosition + rotatedVector;
            //Debug.DrawLine(m_Target.position, finalPosition, Color.blue);

            //            transform.position = finalPosition;
            transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref refVelocity, smoothSpeed);
            //Vector3 smoothedPosition = Vector3.Lerp(transform.position, finalPosition, m_SmoothSpeed);
            //transform.position = smoothedPosition;

            transform.LookAt(target.position);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            if (target)
            {
                Vector3 lookAtPosition = target.position;
                lookAtPosition.y += lookAtHeight;
                Gizmos.DrawLine(transform.position, lookAtPosition);
                Gizmos.DrawSphere(lookAtPosition, 0.25f);
            }

            Gizmos.DrawSphere(transform.position, 0.25f);
        }
        #endregion
    }
}