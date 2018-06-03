using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/* This is for Health of Player */
public class Health : MonoBehaviour, IPunObservable
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
    

	// Use this for initialization
	void Start ()
    {
        //m_text.color = Color.red;
        m_maxhealth = 10;
        PrintHealth();
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("In health : " + m_health);
	}

    // Print health
    public string PrintHealth()
    {
        return m_text.text = m_health + " / " + m_maxhealth;
    }

    // stream - send over network
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.isWriting)
    //    {
    //        // We own this player: send the others our data
    //        //stream.SendNext(IsFiring);
    //        stream.SendNext(m_health);
    //    }
    //    else
    //    {
    //        // Network player, receive data
    //        //this.IsFiring = (bool)stream.ReceiveNext();
    //        this.m_health = (int)stream.ReceiveNext();
    //    }
    //}
}
