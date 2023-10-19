using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(BoxCollider))]
    [System.Serializable]
    public class PaintWall : Task
    {
        [Header("Custom Settings")]

        #region Public Fields

        public GameObject painting;

        public Image barImage;

        public float timer;
        public float taskTime;

        #endregion

        #region Public Methods

        public override void Interact()
        {
            timer = 0;
            barImage.fillAmount = 0;

            base.Interact();
        }

        public override IEnumerator Timer()
        {
            while (isActive)
            {
                timer += 0.01f;
                yield return new WaitForSecondsRealtime(0.01f);
                barImage.fillAmount = timer / taskTime;

                if (barImage.fillAmount >= 1)
                {
                    TaskFinished();
                }
            }
        }
        
        public override void StopTask()
        {
            barImage.fillAmount = 0;
            base.StopTask();
        }
        
        #endregion

        [PunRPC]
        protected override void UpdateTask()
        {
            painting.SetActive(true);
        }
    }
}