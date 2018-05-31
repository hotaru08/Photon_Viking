using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This is for Health of Player */
public class Health : MonoBehaviour
{
    private int m_health, m_maxhealth;

    [SerializeField]
    private TextMesh m_text;


	// Use this for initialization
	void Awake ()
    {
        //m_text.color = Color.red;
        m_health = m_maxhealth = 10;
        PrintHealth();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Print health
    public string PrintHealth()
    {
        return m_text.text = m_health + " / " + m_maxhealth;
    }
}
