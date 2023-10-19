using UnityEngine;
using UnityEngine.UI;


namespace WS20.P3.Overcrowded
{
    public class Book : MonoBehaviour
    {
        #region Public Fields

        public Material selectedOutlineShader;
        public Material standardShader;
        public Image image;

        #endregion

        #region Private Fields

        private BookThief attachedTask;
        private bool isTargetBook;

        #endregion

        #region Public Methods

        public BookThief AttachedTask
        {
            set => attachedTask = value;
        }

        public void HoverButton()
        {
            transform.SetAsLastSibling(); //Set as last sibling so it will be rendered in front to make the outline always visible
            image.material = selectedOutlineShader;
        }

        public void StopHoverButton()
        {
            image.material = standardShader;
        }

        public void Clicked()
        {
            if (isTargetBook)
            {
                attachedTask.BookRemoved();
                gameObject.SetActive(false);
            }
        }

        public void MakeTarget()
        {
            isTargetBook=true;
        }

        #endregion
    }
}