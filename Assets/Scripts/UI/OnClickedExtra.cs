using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace WS20.P3.Overcrowded
{

    public class OnClickedExtra : MonoBehaviourPunCallbacks
    {
        //This could be put into different single Scripts, extending Button functionality. Because there is not many methods we kept it as one for convenience

        public void CallLeaveRoom()
        {
            //GameManager.Instance.LeaveRoom();
            if (PhotonNetwork.CurrentRoom != null)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                SceneManager.LoadScene(0);
            }
            
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.LoadLevel(0);
        }

        public void UpdateAmountOfSeekers()
        {
            GameManager.Instance.UpdateAmountOfSeekers((int)this.gameObject.GetComponent<Slider>().value);
        }

        public void UpdateAmountOfNPCs(float value)
        {
            GameManager.Instance.UpdateAmountOfNPCs((int) value * 10);
        }

        public void TogglePrivateLobby()
        {
            PhotonNetwork.CurrentRoom.IsVisible = !this.gameObject.GetComponent<Toggle>().isOn;
            
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }

}


