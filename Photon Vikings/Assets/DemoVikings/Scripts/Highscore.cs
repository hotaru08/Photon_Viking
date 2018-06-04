using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/* For score of player - attached to player */
public class Highscore : MonoBehaviour
{
    public int m_score;
    public List<GameObject> m_playerList;

    // Use this for initialization
    void Start()
    {
        //foreach (var _player in GameObject.FindGameObjectsWithTag("Player"))
        //{
        //    m_playerList.Add(_player); // add all found player into list
        //}
    }
	
	// Update is called once per frame
	void Update ()
    {
        

        // sort the list of player according to score 
        //m_playerList.Sort(CompareByScore);
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
    public static int CompareByScore(GameObject score1, GameObject score2)
    {
        return score1.GetComponent<Highscore>().m_score.CompareTo(score2.GetComponent<Highscore>().m_score);
    }
}
