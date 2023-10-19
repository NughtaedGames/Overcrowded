using UnityEngine;

namespace WS20.P3.Overcrowded
{
    public class Billboard : MonoBehaviour
    {
        #region Public Fields
        
        public float offset;

        #endregion

        #region MonoBehaviour CallBacks

        void LateUpdate()
                {
                    Vector3 targetPosition = new Vector3(transform.position.x, Camera.main.transform.position.y,
                        Camera.main.transform.position.z - offset);
                    transform.LookAt(2 * transform.position - targetPosition);
                }

        #endregion
    }
}