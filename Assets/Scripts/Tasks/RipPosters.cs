using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(BoxCollider))]
    [System.Serializable]
    public class RipPosters : Task
    {
        [Header("Custom Settings")]

        #region Public Fields

        public GameObject oldPainting;

        public GameObject painting;
        public Image barImage;

        #endregion

        #region Private Fields

        private int paperCount;

        #endregion

        #region Public Methods

        public override void Interact()
        {
            barImage.fillAmount = 0;

            base.Interact();
        }

        public void AddPaper()
        {
            paperCount++;
            AudioManager.instance.PlayRandomFromList("PosterSounds");

            if (paperCount >= 16)
            {
                TaskFinished();
            }
        }

        public override IEnumerator Timer() { yield break; }

        #endregion

        #region Photon RPCs

        [PunRPC]
        protected override void UpdateTask()
        {
            painting.SetActive(true);
            oldPainting.SetActive(false);
        }

        #endregion
    }
}