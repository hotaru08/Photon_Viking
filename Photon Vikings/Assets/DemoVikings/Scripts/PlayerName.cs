using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/* This is for Player Name */
public class PlayerName : MonoBehaviour
{
    [SerializeField]
    private TextMesh m_text;

    //private string m_playerName;

	// Use this for initialization
	void Start ()
    {
        /* Add to method to carry out event */
        //PhotonNetwork.OnEventCall += this.OnEventHandler;
        //m_playerName = "";
        PrintName();
	}

    /* Print name of player */
    private string PrintName()
    {
        foreach (PhotonPlayer _player in PhotonNetwork.playerList)
        {
            return m_text.text = _player.NickName;
        }

        return null;

           //return m_text.text = PhotonNetwork.playerName;
    }

    ///* Receive data of events from server */
    //private void OnEventHandler(byte eventCode, object content, int senderId)
    //{
    //    switch (eventCode)
    //    {
    //        case 2:
    //            m_playerName = (string)content;
    //            Debug.Log("Name : " + m_playerName);
    //            //Debug.Log(string.Format("Message from Server : {0}", (string)content));
    //            break;
    //    }
    //}
    
}
