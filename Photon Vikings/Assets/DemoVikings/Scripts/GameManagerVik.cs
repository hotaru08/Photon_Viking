using UnityEngine;
using System.Collections;
using System.Text;

public class GameManagerVik : Photon.MonoBehaviour {

    // this is a object name (must be in any Resources folder) of the prefab to spawn as player avatar.
    // read the documentation for info how to spawn dynamically loaded game objects at runtime (not using Resources folders)
    public string playerPrefabName = "Charprefab";

    void OnJoinedRoom()
    {
        StartGame();
        DoRaiseEvent();
    }

    public void DoRaiseEvent()
    {
        byte evCode = 2;
        string contentMessage = PhotonNetwork.playerName + "," + PlayerPrefs.GetString("playerPassword"); //content of message to be sent
        byte[] content = Encoding.UTF8.GetBytes(contentMessage); // convert to array
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null); // send to plugin
    }

    public void DoStorePosition()
    {
        byte evCode = 3; // evCode for saving position
        string contentMessage = PhotonNetwork.playerName + "," + PlayerPrefs.GetString("playerPos");
        byte[] content = Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

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
        PhotonNetwork.Instantiate(this.playerPrefabName, transform.position, Quaternion.identity, 0, objs);
    }

    void OnGUI()
    {
        if (PhotonNetwork.room == null) return; //Only display this GUI when inside a room

        if (GUILayout.Button("Leave Room"))
        {
            string playerPos = transform.position.x + " " + transform.position.y + " " + transform.position.z;
            PlayerPrefs.SetString("playerPos", playerPos);

            DoStorePosition();
            PhotonNetwork.LeaveRoom();
        }
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("OnDisconnectedFromPhoton");
    }    
}
