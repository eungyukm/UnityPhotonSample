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

        public enum State { None, QuickMatching, QuickMatchDone, GameStart, GameDone }
        public State state;

        private void Awake()
        {
            ShowPanel("ConnectPanel");
            quickMatchText.gameObject.SetActive(false);
        }

        /// <summary>
        /// 각 패널의 이름에 따라 해당 패널만 활성화 하는 함수
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
        /// 플레이어의 수 변화에 따라 게임 시작
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

