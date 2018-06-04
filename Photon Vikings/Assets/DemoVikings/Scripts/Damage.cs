using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Damage for melee */
public class Damage : Photon.MonoBehaviour
{
    private bool isDamaged;
    private float GodMode;

	// Use this for initialization
	void Start ()
    {
        isDamaged = false;
        GodMode = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isDamaged)
            GodMode += Time.deltaTime;

        if(GodMode > 0.6f)
        {
            GodMode = 0.0f;
            isDamaged = false;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Hitbox" &&!isDamaged)
        {
            GameObject player = null;
            foreach(var _player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (_player.GetComponentInChildren<PlayerName>().m_text.text == other.GetComponent<AssignPlayer>().nameofPlayer)
                {
                    player = _player;
                    break;
                }
            }

            //if (photonView.isMine)
            //    return;
            //Debug.Log("CHECKING " + other.transform.parent.gameObject);

            for (int i = 0; i < player.GetComponent<Party>().GetMembers().Length; i++)
            {
                if (photonView.owner.NickName == player.GetComponent<Party>().GetMembers()[i])
                    return;
            }

            GetComponentInChildren<Health>().m_health--;

            Debug.Log(player.GetComponentInChildren<PlayerName>().m_text.text);

            if (player)
                player.GetComponent<Highscore>().m_score++;
            isDamaged = true;
        }

    }
}
