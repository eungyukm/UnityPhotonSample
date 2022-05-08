using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;
using PN = Photon.Pun.PhotonNetwork;
using Random = UnityEngine.Random;
using Photon.Realtime;

namespace MultiPlayCore
{
    /// <summary>
    /// 
    /// </summary>
    public class MultiPlayManager : MonoBehaviourPunCallbacks
    {
        public static MultiPlayManager instnace { get; private set; }

        [Header("Debug")]
        [SerializeField] bool autoJoin = false;
        [SerializeField] byte autoMaxPlayer = 1;


        [Header("Panel")]
        [SerializeField] private GameObject connectPanel;
        [SerializeField] private GameObject lobbyPanel;
        [SerializeField] private GameObject gamePanel;

        [Header("TMP")]
        [SerializeField] private TMP_Text quickMatchText;

        [Header("SpawnPos")]
        [SerializeField] private Transform[] spawnPostions;

        public enum State { None, QuickMatching, QuickMatchDone, GameStart, GameDone }
        public State state;

        public int GetIndex
        {
            get
            {
                for(int i=0; i<PN.PlayerList.Length; i++)
                {
                    if(PN.PlayerList[i] == PN.LocalPlayer)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);
            ShowPanel("ConnectPanel");
            quickMatchText.gameObject.SetActive(false);
        }

        /// <summary>
        /// ?? ?????? ?????? ???? ???? ?????? ?????? ???? ????
        /// </summary>
        /// <param name="name"></param>
        private void ShowPanel(string name)
        {
            connectPanel.SetActive(false);
            lobbyPanel.SetActive(false);
            gamePanel.SetActive(false);

            if(name == connectPanel.name)
            {
                connectPanel.SetActive(true);
            }
            else if(name == lobbyPanel.name)
            {
                lobbyPanel.SetActive(true);
            }
            else if(name == gamePanel.name)
            {
                gamePanel.SetActive(true);
            }
        }


        public void ConnectClick(TMP_InputField nickInput)
        {
            PN.ConnectUsingSettings();
            string nickName = nickInput.text == null ? $"Player{Random.Range(0, 100)}" : nickInput.text;
            PN.NickName = nickName;
        }

        public override void OnConnectedToMaster()
        {
            PN.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            ShowPanel("LobbyPanel");
        }

        public void QuickMachClick()
        {
            if(state == State.None)
            {
                state = State.QuickMatching;

                quickMatchText.gameObject.SetActive(true);

                PN.JoinRandomOrCreateRoom(null, autoMaxPlayer, Photon.Realtime.MatchmakingMode.FillRoom, null, null, 
                    $"room{Random.Range(0,1000)}", new Photon.Realtime.RoomOptions { MaxPlayers = autoMaxPlayer });
            }
            else if(state == State.QuickMatching)
            {
                state = State.None;

                quickMatchText.gameObject.SetActive(false);

                PN.LeaveRoom();
            }
        }

        public override void OnJoinedRoom()
        {
            PlayerChanged();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            PlayerChanged();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerChanged();
        }

        /// <summary>
        /// ?????????? ?? ?????? ???? ???? ????
        /// </summary>
        private void PlayerChanged()
        {
            if (PN.CurrentRoom.PlayerCount == autoMaxPlayer)
            {

            }
            else if (PN.CurrentRoom.PlayerCount != PN.CurrentRoom.MaxPlayers)
            {
                return;
            }

            GameStart();
        }

        private void GameStart()
        {
            Debug.Log("GameStart");
            //PN.LoadLevel("Main");

            if(GetIndex != -1)
            {
                // PN.Instantiat("Player", spawnPostions[GetIndex].position, spawnPostions[GetIndex].rotation);
            }
            else
            {
                Debug.Log("Pun Index is not available");
            }
        }

        private void Update()
        {
            if(state == State.QuickMatching && PN.InRoom)
            {
                quickMatchText.text = $"{PN.CurrentRoom.PlayerCount} / {PN.CurrentRoom.MaxPlayers}";
            }
        }
    }
}

