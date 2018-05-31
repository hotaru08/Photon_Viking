using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is for Player Name */
public class PlayerName : MonoBehaviour
{
    [SerializeField]
    private TextMesh m_text;

	// Use this for initialization
	void Start ()
    {
        //m_text.color = Color.red;
        PrintName();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Printing Player Name
    public string PrintName()
    {
        return m_text.text = PhotonNetwork.playerName;
    }

}
