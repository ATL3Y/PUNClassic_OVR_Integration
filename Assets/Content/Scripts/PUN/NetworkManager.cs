using UnityEngine;


// Source: https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/oculusavatarsdk
namespace Com.ATL3Y.Flow
{
    public class NetworkManager : MonoBehaviour
    {
        private void OnEvent ( byte eventcode, object content, int senderid )
        {
            if ( eventcode == InstantiateVrAvatarEventCode )
            {
                GameObject go = null;

                if ( PhotonNetwork.player.ID == senderid )
                {
                    go = Instantiate ( Resources.Load ( "LocalAvatar" ) ) as GameObject;
                }
                else
                {
                    go = Instantiate ( Resources.Load ( "RemoteAvatar" ) ) as GameObject;
                }

                if ( go != null )
                {
                    PhotonView pView = go.GetComponent<PhotonView>();

                    if ( pView != null )
                    {
                        pView.viewID = ( int ) content;
                    }
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
        }
    }
}
