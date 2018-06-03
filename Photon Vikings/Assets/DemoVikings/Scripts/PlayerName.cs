using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/* This is for Player Name */
public class PlayerName : MonoBehaviour
{
    [SerializeField]
    private TextMesh m_text;
    [SerializeField]
    private GameObject m_player;

    //private string m_playerName;

	// Use this for initialization
	void Start ()
    {
        /* Add to method to carry out event */
        //PhotonNetwork.OnEventCall += this.OnEventHandler;
        //m_playerName = "";
        
	}

    private void Update()
    {
        PrintName();
    }

    /* Print name of players */
    private void PrintName()
    {
        // for each player in the room 
        foreach (PhotonPlayer _player in PhotonNetwork.playerList)
        {
            if (_player.Equals(PhotonNetwork.player))
            {
                m_text.text = PhotonNetwork.playerName; // return player's name
            }
            else
            {
                m_text.text = _player.NickName; //return others names
            }
        }
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
