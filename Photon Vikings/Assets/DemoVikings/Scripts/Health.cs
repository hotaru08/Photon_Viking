using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/* This is for Health of Player */
public class Health : MonoBehaviour
{
    [SerializeField]
    private TextMesh m_text;
    private int m_health, m_maxhealth;

    public int PlayerHealth
    {
        get
        {
            return m_health;
        }
        set
        {
            m_health = value;
        }
    }
    public int PlayerMaxHealth
    {
        get
        {
            return m_maxhealth;
        }
        set
        {
            m_maxhealth = value;
        }
    }
    

	// Use this for initialization
	void Awake ()
    {
        //m_text.color = Color.red;
        m_health = m_maxhealth = 10;
        PrintHealth();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // Print health
    public string PrintHealth()
    {
        return m_text.text = m_health + " / " + m_maxhealth;
    }

    
}
