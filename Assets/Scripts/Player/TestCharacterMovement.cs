using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class TestCharacterMovement : MonoBehaviour
    {

        private Vector3 moveDirection = Vector3.zero;
        private CharacterController characterController;

        public float speed = 3.0f;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();

        }

        void Update()
        {
            CameraWork cameraWork = this.gameObject.GetComponent<CameraWork>();

            if (cameraWork.cameraTransform == null)
            {

                cameraWork.OnStartFollowing();
            }

            if (cameraWork.isFollowing)
            {
                cameraWork.Follow();
            }

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection.Normalize();
            moveDirection *= speed * 100;
            
            characterController.Move(moveDirection * Time.deltaTime);
        }
    }
}