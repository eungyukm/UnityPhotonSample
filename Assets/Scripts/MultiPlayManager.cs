using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using UnityEngine.UI;
using PN = Photon.Pun.PhotonNetwork;
using Random = UnityEngine.Random;
using Photon.Realtime;

namespace MultiPlayCore
{
    /// <summary>
    /// 방 만들기, 방 입장을 위한 매니저
    /// </summary>
    public class MultiPlayManager : MonoBehaviourPunCallbacks
    {
        public static MultiPlayManager instnace { get; private set; }

        [Header("Debug")]
        [SerializeField] bool autoJoin = false;
        [SerializeField] byte autoMaxPlayer = 1;


        [Header("Panel")]
        [SerializeField] private Button machingButton;

        public enum State { None, QuickMatching, QuickMatchDone, GameStart, GameDone }
        public State state;


        public void ConnectClick(InputField nickInput)
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

        }

        public void QuickMachClick()
        {
            if(state == State.None)
            {
                state = State.QuickMatching;

                PN.JoinRandomOrCreateRoom(null, autoMaxPlayer, Photon.Realtime.MatchmakingMode.FillRoom, null, null, 
                    $"room{Random.Range(0,1000)}", new Photon.Realtime.RoomOptions { MaxPlayers = autoMaxPlayer });
            }
            else if(state == State.QuickMatching)
            {
                state = State.None;

                PN.LeaveRoom();
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            PlayerChanged();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PlayerChanged();
        }

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
    }
}

