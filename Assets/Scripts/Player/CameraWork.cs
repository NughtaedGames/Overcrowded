using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class CameraWork : MonoBehaviour
    {        
        #region Public Fields
 
        public Transform cameraTransform;
 
        public bool isFollowing;
 
        #endregion
         
        #region Private Fields

        [SerializeField] private float distance = 7.0f;
        [SerializeField] private float height = 3.0f;
        [SerializeField] private Vector3 centerOffset = Vector3.zero;
        [SerializeField] private float smoothSpeed = 0.125f;

        private Vector3 cameraOffset = Vector3.zero;

        #endregion

        #region Public Methods
        
        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            Cut();
        }

        public void Follow()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;
            
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);
        }

        #endregion

        #region Private Methods

        private void Cut()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            Vector3 pos = transform.position;

            cameraTransform.position = pos + this.transform.TransformVector(cameraOffset);
            
            cameraTransform.LookAt(pos + centerOffset);
        }
        #endregion
    }
}