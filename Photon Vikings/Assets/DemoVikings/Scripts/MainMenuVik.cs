using UnityEngine;
using System.Collections;

public class MainMenuVik : MonoBehaviour
{

    void Awake()
    {
        //PhotonNetwork.logLevel = NetworkLogLevel.Full;

        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        if (!PhotonNetwork.connected)
            PhotonNetwork.ConnectUsingSettings("v1.0"); // version of the game/demo. used to separate older clients from newer ones (e.g. if incompatible)

        //Load name from PlayerPrefs
        PhotonNetwork.playerName = PlayerPrefs.GetString("playerName", "Guest" + Random.Range(1, 9999));

        //Set camera clipping for nicer "main menu" background
        Camera.main.farClipPlane = Camera.main.nearClipPlane + 0.1f;

    }

    private string roomName = "myRoom";
    private Vector2 scrollPos = Vector2.zero;
    private string password = "";

    private bool isLogin = true;
    //private GameManagerVik manager = new GameManagerVik();

    void OnGUI()
    {
        if (!PhotonNetwork.connected)
        {
            ShowConnectingGUI();
            return;   //Wait for a connection
        }
        if (PhotonNetwork.room != null)
            return; //Only when we're not in a Room


        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Main Menu");

        //Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player name:", GUILayout.Width(150));
        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
        if (GUI.changed)//Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
        GUILayout.EndHorizontal();

        //Player password
        GUILayout.BeginHorizontal();
        GUILayout.Label("Password:", GUILayout.Width(150)); // render text
        password = GUILayout.TextField(password); // render the previous pw and input field
        if (GUI.changed) // password is changed, save it
            PlayerPrefs.SetString("playerPassword", password);
        GUILayout.EndHorizontal();

        //Print login button
        //GUILayout.Space(5);
        //GUILayout.BeginHorizontal();
        //GUILayout.Space(100);
        //if (GUILayout.Button("Login", GUILayout.Width(120)))
        //{
        //    // call function to send msg to server plugin
        //    manager.Login(); 


        //    // receive the message from server
        //    //PhotonNetwork.OnEventCall += this.OnEventHandler;

        //}
        //GUILayout.EndHorizontal();
        
        GUILayout.Space(50);

        if (isLogin)
        {
            //Join room by title
            GUILayout.BeginHorizontal();
            GUILayout.Label("JOIN ROOM:", GUILayout.Width(150));
            roomName = GUILayout.TextField(roomName);
            if (GUILayout.Button("GO"))
            {
                PhotonNetwork.JoinRoom(roomName);
            }
            GUILayout.EndHorizontal();

            //Create a room (fails if exist!)
            GUILayout.BeginHorizontal();
            GUILayout.Label("CREATE ROOM:", GUILayout.Width(150));
            roomName = GUILayout.TextField(roomName);
            if (GUILayout.Button("GO"))
            {
                // using null as TypedLobby parameter will also use the default lobby
                PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 10 }, TypedLobby.Default);
            }
            GUILayout.EndHorizontal();

            //Join random room
            GUILayout.BeginHorizontal();
            GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150));
            if (PhotonNetwork.GetRoomList().Length == 0)
            {
                GUILayout.Label("..no games available...");
            }
            else
            {
                if (GUILayout.Button("GO"))
                {
                    PhotonNetwork.JoinRandomRoom();
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(30);
            GUILayout.Label("ROOM LISTING:");
            if (PhotonNetwork.GetRoomList().Length == 0)
            {
                GUILayout.Label("..no games available..");
            }
            else
            {
                //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
                scrollPos = GUILayout.BeginScrollView(scrollPos);
                foreach (RoomInfo game in PhotonNetwork.GetRoomList())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
                    if (GUILayout.Button("JOIN"))
                    {
                        PhotonNetwork.JoinRoom(game.name);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
        }

        GUILayout.EndArea();
    }

    /* Loading GUI before main menu */
    void ShowConnectingGUI()
    {
        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Connecting to Photon server.");
        GUILayout.Label("Hint: This demo uses a settings file and logs the server address to the console.");

        GUILayout.EndArea();
    }

    /* Call this to change to main menu */
    public void OnConnectedToMaster()
    {
        // this method gets called by PUN, if "Auto Join Lobby" is off.
        // this demo needs to join the lobby, to show available rooms!

        PhotonNetwork.JoinLobby();  // this joins the "default" lobby
    }
}

     