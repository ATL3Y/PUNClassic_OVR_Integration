using UnityEngine;


// Source: https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/oculusavatarsdk
namespace Com.ATL3Y.Flow
{
    public class NetworkManager : MonoBehaviour
    {
        private void OnEvent ( byte eventcode, object content, int senderid )
        {
            print ( "received event" );
            if ( eventcode == InstantiateVrAvatarEventCode )
            {
                GameObject go = null;
                print ( "senderID: " + senderid + ", " + " player ID: " + PhotonNetwork.player.ID );
                if ( PhotonNetwork.player.ID == senderid )
                {
                    print ( "spawn local avatar" );
                    go = Instantiate ( Resources.Load ( "LocalAvatar" ) ) as GameObject;
                }
                else
                {
                    print ( "spawn remote avatar" );
                    go = Instantiate ( Resources.Load ( "RemoteAvatar" ) ) as GameObject;
                }

                if ( go != null )
                {
                    PhotonView pView = go.GetComponent<PhotonView>();

                    if ( pView != null )
                    {
                        pView.viewID = ( int ) content;
                        print ( "assigned view id of: " + pView.viewID );
                    }
                }
                else
                {
                    print ( "go is null" );
                }
            }
        }

        public void OnEnable ( )
        {
            PhotonNetwork.OnEventCall += OnEvent;
        }

        public void OnDisable ( )
        {
            PhotonNetwork.OnEventCall -= OnEvent;
        }

        private void OnDestroy ( )
        {
            // UnAllocate?
        }

        public readonly byte InstantiateVrAvatarEventCode = 123;

        public void OnJoinedRoom ( )
        {
            int viewId = PhotonNetwork.AllocateViewID();

            PhotonNetwork.RaiseEvent ( InstantiateVrAvatarEventCode, viewId, true, new RaiseEventOptions ( ) { CachingOption = EventCaching.AddToRoomCache, Receivers = ReceiverGroup.All } );
            print ( "sending view ID: " + viewId );
        }
    }
}
