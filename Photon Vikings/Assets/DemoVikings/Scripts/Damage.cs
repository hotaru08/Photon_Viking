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
            //if (photonView.isMine)
            //    return;

            for(int i = 0; i < other.transform.parent.GetComponentInParent<Party>().members.Length; i++)
            {
                if (photonView.owner.NickName == other.transform.parent.GetComponentInParent<Party>().members[i])
                    return;
            }

            GetComponentInChildren<Health>().m_health--;
            other.transform.parent.parent.GetComponent<Highscore>().m_score++;
            isDamaged = true;
        }

    }
}
