using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PhotonNetManager : MonoBehaviourPunCallbacks
{
    [Header("Login Panel")]
    public InputField NickNameInput;

    public GameObject LoginPanel;

    [Header("LobbyPanel")] 
    public GameObject LobbyPanel;

    public InputField RoomNameInput;
    public Text WelcomText;
    public Text LobbyInfoText;
    
    [Header("ETC")]
    public Text StatusText;

    public GameObject[] Panels = new GameObject[3];
    public PhotonView PV;
    public PanelState PanelStateValue = PanelState.None;

    private void Awake()
    {
        SetPanel();
    }


    public void Connect()
    {
        // 포톤 초기 연결
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PanelStateValue = PanelState.Logined;
        SetPanel();
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
    }

    public void CreateRoom()
    {
        string roomName = RoomNameInput.text == "" ? "Room" + Random.Range(0, 100) : RoomNameInput.text;
        PhotonNetwork.CreateRoom(roomName, new RoomOptions {MaxPlayers = 4});
    }


    private void Update()
    {
        StatusText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    private void SetPanel()
    {
        for (int i = 0; i < 3; i++)
        {
            if (PanelStateValue == (PanelState) i)
            {
                Panels[i].SetActive(true);
            }
            else
            {
                Panels[i].SetActive(false);
            }
        }
    }
}

public enum PanelState
{
    // 상태 없음
    None,
    // 로그인 상태
    Logined,
    // 로비 상태
    Lobby,
    
}
