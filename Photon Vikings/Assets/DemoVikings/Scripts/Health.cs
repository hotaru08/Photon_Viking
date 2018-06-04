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
    public static bool isDied, isShielded, isSpawned;
    private double m_timer, m_shieldtime;

    private const int MINUS_SCORE_DIED = 20;

    public string shield = "Shield";
    public GameObject m_shield;

    // Use this for initialization
    void Start ()
    {
        m_maxhealth = 10;
        m_timer = 0.0;
        m_shieldtime = 0.0;
        isDied = false;
        isShielded = false;
        isSpawned = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        PrintHealth();
        // Debug.Log("In health : " + m_health);

        if (m_health <= 0 && !isShielded)
        {
            m_health = 0;
            isDied = true;
        }

        // When died, respawn after buffer
        if (isDied)
        {
            transform.parent.GetComponent<Highscore>().m_score -= MINUS_SCORE_DIED;
            m_timer += Time.deltaTime;
            //Debug.Log("died Time : " + m_timer);
            if (m_timer > 3.0)
            {
                isShielded = true;
                isSpawned = true;
                m_health = m_maxhealth;
                isDied = false;
                m_timer = 0.0;
            }
        }

        // Activate shield 
        if (isShielded)
        {
            //create shield
            m_shield = PhotonNetwork.Instantiate(shield, transform.parent.position, Quaternion.identity, 0);
            m_shield.transform.SetParent(transform.parent);

            isShielded = false;
        }

        // Activate countdown for shield
        if (isSpawned)
        {
            m_shieldtime += Time.deltaTime;
            //Debug.Log("Shield Time : " + m_shieldtime);
            if (m_shieldtime > 5.0 && m_shield)
            {
                // destroy shield 
                PhotonNetwork.Destroy(m_shield);

                isSpawned = false;
                m_shieldtime = 0.0;
            }
        }
    }

    // Print health
    public string PrintHealth()
    {
        return m_text.text = m_health + " / " + m_maxhealth;
    }
}
