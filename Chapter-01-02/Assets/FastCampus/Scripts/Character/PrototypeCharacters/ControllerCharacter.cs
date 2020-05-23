using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastCampus.Characters
{
    [RequireComponent(typeof(CharacterController))]
    public class ControllerCharacter : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        private float speed = 5f;

        private CharacterController characterController;
        private Vector3 calcVelocity = Vector3.zero;


        [SerializeField]
        private float jumpHeight = 2f;

        [SerializeField]
        private float gravity = -9.81f;

        [SerializeField]
        private float groundCheckDistance = 0.2f;

        [SerializeField]
        private LayerMask groundLayerMask;

        [SerializeField]
        private float dashDistance = 5f;

        [SerializeField]
        private Vector3 drags;

        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            // Check grounded
            bool isGrounded = characterController.isGrounded;
            if (isGrounded && calcVelocity.y < 0)
            {
                calcVelocity.y = 0f;
            }

            // Process move inputs
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            characterController.Move(move * Time.deltaTime * speed);
            if (move != Vector3.zero)
            {
                transform.forward = move;
            }

            // Process jump input
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                calcVelocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Process dash input
            if (Input.GetButtonDown("Dash"))
            {
                Debug.Log("Dash");
                calcVelocity += Vector3.Scale(transform.forward, dashDistance * new Vector3((Mathf.Log(1f / (Time.deltaTime * drags.x + 1)) / -Time.deltaTime),
                    0,
                    (Mathf.Log(1f / (Time.deltaTime * drags.z + 1)) / -Time.deltaTime))
                    );

            }

            // Process gravity
            calcVelocity.y += gravity * Time.deltaTime;

            // Process dash ground drags
            calcVelocity.x /= 1 + drags.x * Time.deltaTime;
            calcVelocity.y /= 1 + drags.y * Time.deltaTime;
            calcVelocity.z /= 1 + drags.z * Time.deltaTime;

            characterController.Move(calcVelocity * Time.deltaTime);
        }

        #endregion Unity Methods
    }
}