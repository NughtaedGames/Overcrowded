using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(BoxCollider))]
    [System.Serializable]
    public class BookThief : Task
    {
        [Header("Custom Settings")]

        #region Public Fields

        public int targetBooks;

        #endregion

        #region Private Fields

        [SerializeField] private GameObject originalTaskSprite;
        
        [SerializeField] private List<Book> books;
        
        [SerializeField] private Sprite[] targetBookSprites;
        [SerializeField] private Sprite[] decoyBookSprites;

        private int booksRemoved;
            
        #endregion

        #region MonoBehaviour CallBacks

        public void Awake()
        {
            foreach (var book in books)
            {
                book.image.sprite = decoyBookSprites[Random.Range(0, decoyBookSprites.Length)];
                book.AttachedTask = this;
            }
            for (int i = 0; i < targetBooks; i++)
            {
                var r = books[Random.Range(0, books.Count)];
                r.MakeTarget();
                r.image.sprite = targetBookSprites[Random.Range(0, targetBookSprites.Length)];
                books.Remove(r);
            }
        }

        #endregion
        
        #region Public Methods

        public void BookRemoved()
        { 
            AudioManager.instance.PlayRandomFromList("RemoveBook");
            
            if(++booksRemoved == targetBooks) TaskFinished();
        }

        #endregion

        #region Photon RPCs
        
        [PunRPC]
        protected override void UpdateTask()
        {
            originalTaskSprite.SetActive(false);
            StartCoroutine(nameof(ShowWarningCo));
        }
        
        private IEnumerator ShowWarningCo()
        {

            Image img = mapPointer.GetComponent<TaskPointerAlertReference>().alertReference;
            
            AudioManager.instance.PlayLocalSound("AngryAfterTask", transform.position);
            
            img.gameObject.SetActive(true);
            mapPointer.SetActive(true);
            mapPointer.GetComponent<Image>().enabled = false;
            yield return new WaitForSeconds(3.25f);
            mapPointer.SetActive(false);
            
        }

        #endregion
    }
}