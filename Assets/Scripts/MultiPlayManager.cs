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
        /// �7�6 �5�9�8�1�3�5 �3�3�0�5�2�3 �9�7�9�5 �6�7�9�0 �5�9�8�1�0�7 �6�9�1�0�6�1 �6�9�8�9 �6�5�2�1
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
        /// �5�3�0�5�3�3�2�5�3�5 �2�1 �1�3�6�1�2�3 �9�7�9�5 �7�5�3�9 �2�7�3�2
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

