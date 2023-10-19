using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

namespace WS20.P3.Overcrowded
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region Monobehaviour Callbacks

        private void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField!=null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }
        }

        #endregion

        #region Public Methods

           public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }

        #endregion

    }
    
}


