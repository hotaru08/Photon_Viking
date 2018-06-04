﻿using System.Collections;
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

            GetComponentInChildren<Health>().m_health -= 1;
            isDamaged = true;
        }

    }
}
