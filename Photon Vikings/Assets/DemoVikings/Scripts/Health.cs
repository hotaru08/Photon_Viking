using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/* This is for Health of Player */
public class Health : MonoBehaviour/*, IPunObservable*/
{
    [SerializeField]
    private TextMesh m_text;
    public int m_health, m_maxhealth;

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

    private GameObject m_player;
    public static bool isDied;
    private double m_timer;
    

	// Use this for initialization
	void Start ()
    {
        m_maxhealth = 10;
        m_timer = 0.0;
        isDied = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
        PrintHealth();
        //Debug.Log("In health : " + m_health);

        if (m_health <= 0)
        {
            m_health = 0;
            isDied = true;
        }

        // When died, set buffer then respawn
        if (isDied)
        {
            m_timer += Time.deltaTime;
            if (m_timer > 5.0)
            {
                m_health = m_maxhealth;
                isDied = false;
                m_timer = 0.0;
            }
        }
	}

    // Print health
    public string PrintHealth()
    {
        return m_text.text = m_health + " / " + m_maxhealth;
    }
}
