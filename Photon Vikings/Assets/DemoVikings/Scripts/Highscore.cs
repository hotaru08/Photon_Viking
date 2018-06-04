using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/* For score of player - attached to player */
public class Highscore : MonoBehaviour
{
    public int m_score;

    // TODO :
    // When kill player, increase score
    // when die, reset to 0

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        // sort the list of player according to score 
        Array.Sort(PhotonNetwork.playerList);
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
        GUILayout.BeginArea(new Rect(Screen.width - 90, 0, 1000, 1000));

        // board indexes
        GUILayout.BeginHorizontal();
        GUILayout.Label("SCORE");
        GUILayout.EndHorizontal();

        foreach (var _player in GameObject.FindGameObjectsWithTag("Player"))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_player.GetComponent<Highscore>().m_score.ToString());
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
    }

    /* Compare score to sort */
    static int SortByScore(GameObject p1, GameObject p2)
    {
        return p1.GetComponent<Highscore>().m_score.CompareTo(p2.GetComponent<Highscore>().m_score);
    }
}
