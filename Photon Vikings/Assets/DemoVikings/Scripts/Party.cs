using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : Photon.MonoBehaviour {

    public string[] members;
	// Use this for initialization
	void Start () {
        members = GameObject.FindGameObjectWithTag("Code").GetComponent<ChatVik>().Party;
    }
	
	// Update is called once per frame
	void Update () {

        if(photonView.owner.NickName == GameObject.FindGameObjectWithTag("Code").GetComponent<ChatVik>().PlayerParty[0])
            members = GameObject.FindGameObjectWithTag("Code").GetComponent<ChatVik>().Party;
	}
}
