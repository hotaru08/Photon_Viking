using UnityEngine;
using System.Collections;
using System.Text;

public class GameManagerVik : Photon.MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "Charprefab";
    public string petPrefabName = "Pet";
    private float m_timer;
    private bool m_btimerStart = false;
    private static GameObject player;
    private static GameObject pet;

    private static string m_PlayerPosition;

    void OnJoinedRoom()
    {
        PhotonNetwork.OnEventCall += this.OnEventHandler;
        DoRaiseEvent();
        GetStorePosition();
        //StartGame();
    }

    /* Store login info */
    public void DoRaiseEvent()
    {
        byte evCode = 2;
        string contentMessage = PhotonNetwork.playerName + "," + PlayerPrefs.GetString("playerPassword"); //content of message to be sent
        byte[] content = Encoding.UTF8.GetBytes(contentMessage); // convert to array
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null); // send to plugin
    }

    /* Store position of player */
    public void DoStorePosition()
    {
        byte evCode = 3; // evCode for saving position
        string contentMessage = PhotonNetwork.playerName + "," + PlayerPrefs.GetString("playerPos");
        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    public void GetStorePosition()
    {
        byte evCode = 4; // evCode for saving position
        string contentMessage = PhotonNetwork.playerName;
        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    /* Read name of player */


    IEnumerator OnLeftRoom()
    {
        //Easy way to reset the level: Otherwise we'd manually reset the camera

        //Wait untill Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;

        Application.LoadLevel(Application.loadedLevel);

    }

    void StartGame()
    {
        Camera.main.farClipPlane = 1000; //Main menu set this to 0.4 for a nicer BG    

        //prepare instantiation data for the viking: Randomly diable the axe and/or shield
        bool[] enabledRenderers = new bool[2];
        enabledRenderers[0] = Random.Range(0,2)==0;//Axe
        enabledRenderers[1] = Random.Range(0, 2) == 0; ;//Shield
        
        object[] objs = new object[1]; // Put our bool data in an object array, to send
        objs[0] = enabledRenderers;

        // Spawn our local player
        if (player == null)
        {
            if (m_PlayerPosition != null)
            {
                Debug.Log("Creating player at its saved position : " + m_PlayerPosition);
                string[] positions = m_PlayerPosition.Split(' ');
                Vector3 positionForPlayer = new Vector3(float.Parse(positions[0]), float.Parse(positions[1]), float.Parse(positions[2]));
                player = PhotonNetwork.Instantiate(this.playerPrefabName, positionForPlayer, Quaternion.identity, 0, objs);
                pet = PhotonNetwork.Instantiate(this.petPrefabName, positionForPlayer + new Vector3(0, 0, -1), Quaternion.identity, 0, objs);
                if(player.GetComponent<PhotonView>().isMine)
                    pet.GetComponent<PetMovement>().SetPlayer(player);
            }
            else
            {
                player = PhotonNetwork.Instantiate(this.playerPrefabName, transform.position, Quaternion.identity, 0, objs);
                pet = PhotonNetwork.Instantiate(this.petPrefabName, transform.position + new Vector3 (0,0, -1), Quaternion.identity,0, objs);
                if (player.GetComponent<PhotonView>().isMine)
                    pet.GetComponent<PetMovement>().SetPlayer(player);
            }
        }
        // start timer
        m_timer = 0.0f;
        m_btimerStart = true;
    }

    void OnGUI()
    {
        if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        // Button to leave room
        if (GUILayout.Button("Leave Room"))
        {
            string playerPos = player.transform.position.x + " " + player.transform.position.y + " " + player.transform.position.z;
            PlayerPrefs.SetString("playerPos", playerPos);
            DoStorePosition();

            PhotonNetwork.LeaveRoom();
        }

        // Broadcast login of player
        if (m_timer < 5.0f)
        {
            GUILayout.BeginHorizontal();
            GUI.color = Color.yellow;
            GUILayout.Label(PhotonNetwork.playerName + " has logged in!");
            GUI.color = Color.white;
            GUILayout.EndHorizontal();

        }
        else
            m_btimerStart = false;
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("OnDisconnectedFromPhoton");
    }

    public void Update()
    {
        if (m_btimerStart)
            m_timer += Time.deltaTime;
    }

    /* Receive data of events from server */
    private void OnEventHandler(byte eventCode, object content, int senderId)
    {
        switch(eventCode)
        {
            //case 2:
            //    Debug.Log(string.Format("Message from Server : {0}", (string)content));
            //    break;
            //case 3:
            //    m_PlayerPosition = (string)content;
            //    Debug.Log("position : " + m_PlayerPosition);
            //    break;
            case 4:
                m_PlayerPosition = (string)content;
                Debug.Log("position : " + m_PlayerPosition);
                StartGame();
                break;
        } 
    }
}
