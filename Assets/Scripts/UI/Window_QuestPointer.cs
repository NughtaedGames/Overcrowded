using UnityEngine;
using UnityEngine.SceneManagement;

namespace WS20.P3.Overcrowded
{
        
    public class Window_QuestPointer : MonoBehaviour
    {
        [SerializeField]
        private Camera uiCamera;

        private Camera mainCamera;

        public Vector3 targetPosition;
        private RectTransform pointerRectTransform;
        public Vector3 startPosition;

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        private void Awake()
        {
            pointerRectTransform = transform.Find("Pointer").GetComponent<RectTransform>();
            mainCamera = Camera.main;
        }


        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (PlayerManager.LocalPlayerInstance == null)
            {
                return;
            }
            Vector3 toPosition = targetPosition;
            Vector3 fromPosition = PlayerManager.LocalPlayerInstance.transform.position; 
            fromPosition.y = 0f;
            Vector3 dir = toPosition - fromPosition;
            float angle = GetAngleFromVectorFloat(dir);
            pointerRectTransform.localEulerAngles = new Vector3(0,0,angle);

            float borderSize = 50f;
            
            Vector3 targetPositionScreenPoint = mainCamera.WorldToScreenPoint(targetPosition);
            bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

            if (isOffScreen)
            {
                pointerRectTransform.localPosition = new Vector3(0,0,0);
            }
            else
            {
                Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
                pointerRectTransform.position = pointerWorldPosition;
                Vector3 localPosition = pointerRectTransform.localPosition;
                pointerRectTransform.localPosition = new Vector3(localPosition.x, localPosition.y, 0f);
            }
        }




        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float n = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            if (n < 0) n += 360;

            return n;
        }
    }

}
