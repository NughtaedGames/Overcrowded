using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace WS20.P3.Overcrowded
{
    public class Paper : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private GraphicRaycaster m_Raycaster;
        [SerializeField] private EventSystem m_EventSystem;

        private PointerEventData m_PointerEventData;

        #endregion

        #region MonoBehaviour CallBacks

        void Update()
        {
            m_PointerEventData = new PointerEventData(m_EventSystem) {position = this.transform.localPosition}; //Set the Pointer Event Position to that of the game object

            List<RaycastResult> results = new List<RaycastResult>();

            m_Raycaster.Raycast(m_PointerEventData, results);
        }
        
        #endregion
    }
}