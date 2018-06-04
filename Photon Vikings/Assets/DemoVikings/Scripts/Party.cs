using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour {

    public string[] members;
	// Use this for initialization
	void Start () {
        members = GameObject.FindGameObjectWithTag("Code").GetComponent<ChatVik>().Party;
    }
	
	// Update is called once per frame
	void Update () {
        members = GameObject.FindGameObjectWithTag("Code").GetComponent<ChatVik>().Party;
	}
}
