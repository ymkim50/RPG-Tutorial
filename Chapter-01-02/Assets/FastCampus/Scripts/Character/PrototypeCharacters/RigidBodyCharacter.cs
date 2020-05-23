using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastCampus.Characters
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider))]
    public class RigidBodyCharacter : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private float speed = 5f;

        private Rigidbody rigidbody;
        private Vector3 inputDirection = Vector3.zero;

        [SerializeField]
        private float jumpHeight = 2f;

        [SerializeField]
        private float groundCheckDistance = 0.2f;

        [SerializeField]
        private LayerMask groundLayerMask;

        private bool isGrounded = true;

        [SerializeField]
        private float dashDistance = 5f;

        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            // Check grounded
            CheckGroundStatus();

            // Process move inputs
            inputDirection = Vector3.zero;
            inputDirection.x = Input.GetAxis("Horizontal");
            inputDirection.z = Input.GetAxis("Vertical");
            if (inputDirection != Vector3.zero)
            {
                transform.forward = inputDirection;
            }

            // Process jump input
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rigidbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            }

            if (Input.GetButtonDown("Dash"))
            {
                Vector3 dashVelocity = Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * rigidbody.drag + 1)) / -Time.deltaTime), 0, (Mathf.Log(1f / (Time.deltaTime * rigidbody.drag + 1)) / -Time.deltaTime)));
                rigidbody.AddForce(dashVelocity, ForceMode.VelocityChange);
            }
        }

        private void FixedUpdate()
        {
            rigidbody.MovePosition(rigidbody.position + inputDirection * speed * Time.fixedDeltaTime);
        }

        #endregion Unity Methods

        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * groundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character
            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, groundCheckDistance, groundLayerMask))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }


    }
}