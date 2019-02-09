using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace FLOW_PUN
{
    public class Launcher : Photon.PunBehaviour
    {
        private static Launcher instance;
        public static Launcher Instance { get { return instance; } }
        private string gameVersion = "Test_0";

        private void Awake ( )
        {
            instance = this;

            // Game Version is derrived from scene name so you can only play with players joining from this scene.
            gameVersion += "_" + SceneManager.GetActiveScene ( ).name;

            print ( "You are in gameVersion: " + gameVersion );

            // PhotonNetwork.ConnectToRegion ( CloudRegionCode.usw, gameVersion );
            // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;


            // #Critical
            // this makes sure we won't use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = false;

            PhotonNetwork.logLevel = PhotonLogLevel.Full;
        }

        private void Start ( )
        {
            InitLocalGame ( );
            Connect ( );
        }

        // Instantiates local game elements and sets references.
        // This should be in a game controller.
        private void InitLocalGame ( )
        {

        }

        public void Connect ( )
        {
            if ( PhotonNetwork.connected )
            {
                // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom ( );
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server.
                PhotonNetwork.ConnectUsingSettings ( gameVersion );
            }
        }

        public override void OnPhotonRandomJoinFailed ( object [ ] codeAndMsg )
        {
            // base.OnPhotonRandomJoinFailed ( codeAndMsg );
            Debug.Log ( "Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);" );
            // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
            PhotonNetwork.CreateRoom ( null, new RoomOptions ( ) { MaxPlayers = 4 }, null );

        }

        public override void OnConnectedToMaster ( )
        {
            // base.OnConnectedToMaster ( );
            
            Debug.Log ( "Launcher: OnConnectedToMaster() was called by PUN" );
            PhotonNetwork.JoinRandomRoom ( );
        }

        public override void OnDisconnectedFromPhoton ( )
        {
            // base.OnDisconnectedFromPhoton ( );
            Debug.LogWarning ( "Launcher: OnDisconnectedFromPhoton() was called by PUN" );
        }

        public override void OnJoinedRoom ( )
        {
            // base.OnJoinedRoom ( );
            Debug.Log ( "Launcher: OnJoinedRoom() called by PUN. Now this client is in a room." );

            // Set Send Rate.
            //PhotonNetwork.sendRate = 30;
            //PhotonNetwork.sendRateOnSerialize = 30;
        }
    }
}


