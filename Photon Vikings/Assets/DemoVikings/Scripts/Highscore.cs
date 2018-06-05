using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/* For score of player - attached to player */
public class Highscore : Photon.MonoBehaviour
{
    public int m_score;

	// Use this for initialization
	void Start ()
    {
        photonView.owner.SetScore(m_score);
	}
	
	// Update is called once per frame
	void Update ()
    {
        // sort the list of player according to score 
        Array.Sort(PhotonNetwork.playerList, SortByScore);
        Array.Reverse(PhotonNetwork.playerList);

        Debug.Log("List: " + PhotonNetwork.playerList);
        foreach (var _player in PhotonNetwork.playerList)
        {

            Debug.Log("SCore: " + _player.GetScore().ToString());
            
        }
    }

    /* For printing on GUI of highscore */
    private void OnGUI()
    {
        // ------ RANK 
        GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 1000, 1000));

        // board indexes
        GUILayout.BeginHorizontal();
        GUILayout.Label("RANK");
        GUILayout.EndHorizontal();

        for (int i = 1; i <= 5; ++i)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(i + ".");
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

        // ------ PLAYER LIST 
        GUILayout.BeginArea(new Rect(Screen.width - 150, 0, 1000, 1000));

        // board indexes
        GUILayout.BeginHorizontal();
        GUILayout.Label("PLAYER");
        GUILayout.EndHorizontal();

        foreach (var _player in PhotonNetwork.playerList)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_player.NickName);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();

        // ------ PLAYER SCORE 
        GUILayout.BeginArea(new Rect(Screen.width - 50, 0, 1000, 1000));

        // board indexes
        GUILayout.BeginHorizontal();
        GUILayout.Label("SCORE");
        GUILayout.EndHorizontal();

        foreach (var _player in PhotonNetwork.playerList)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_player.GetScore().ToString());
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    /* Compare score to sort */
    static int SortByScore(PhotonPlayer p1, PhotonPlayer p2)
    {
        return p1.GetScore().CompareTo(p2.GetScore());
    }
}
