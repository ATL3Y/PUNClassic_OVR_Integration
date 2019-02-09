using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotonAvatarView : MonoBehaviour
{

    private PhotonView photonView;
    private OvrAvatar ovrAvatar;
    private OvrAvatarRemoteDriver remoteDriver;
    private int localSequence;

    private List<byte[]> packetData;

    public void Start ( )
    {
        photonView = GetComponent<PhotonView> ( );

        if ( photonView.isMine )
        {
            ovrAvatar = GetComponent<OvrAvatar> ( );
            ovrAvatar.RecordPackets = true;
            ovrAvatar.PacketRecorded += OnLocalAvatarPacketRecorded;

            packetData = new List<byte [ ]> ( );
        }
        else
        {
            remoteDriver = GetComponent<OvrAvatarRemoteDriver> ( );
        }
    }

    public void OnDisable ( )
    {
        if ( photonView.isMine )
        {
            ovrAvatar.RecordPackets = false;
            ovrAvatar.PacketRecorded -= OnLocalAvatarPacketRecorded;
        }
    }

    public void OnLocalAvatarPacketRecorded ( object sender, OvrAvatar.PacketEventArgs args )
    {
        if ( notReadyForSerialization )
        {
            return;
        }

        using ( MemoryStream outputStream = new MemoryStream ( ) )
        {
            BinaryWriter writer = new BinaryWriter(outputStream);

            var size = Oculus.Avatar.CAPI.ovrAvatarPacket_GetSize(args.Packet.ovrNativePacket);
            byte[] data = new byte[size];
            Oculus.Avatar.CAPI.ovrAvatarPacket_Write ( args.Packet.ovrNativePacket, size, data );

            writer.Write ( localSequence++ );
            writer.Write ( size );
            writer.Write ( data );

            packetData.Add ( outputStream.ToArray ( ) );
        }
    }

    private void DeserializeAndQueuePacketData ( byte [ ] data )
    {
        if ( notReadyForSerialization )
        {
            return;
        }

        using ( MemoryStream inputStream = new MemoryStream ( data ) )
        {
            BinaryReader reader = new BinaryReader(inputStream);
            int remoteSequence = reader.ReadInt32();

            int size = reader.ReadInt32();
            byte[] sdkData = reader.ReadBytes(size);

            System.IntPtr packet = Oculus.Avatar.CAPI.ovrAvatarPacket_Read((System.UInt32)data.Length, sdkData);
            remoteDriver.QueuePacket ( remoteSequence, new OvrAvatarPacket { ovrNativePacket = packet } );
        }
    }

    public void OnPhotonSerializeView ( PhotonStream stream, PhotonMessageInfo info )
    {
        // Need?
        if ( notReadyForSerialization )
        {
            return;
        }

        if ( stream.isWriting )
        {
            if ( packetData.Count == 0 )
            {
                return;
            }

            stream.SendNext ( packetData.Count );

            foreach ( byte [ ] b in packetData )
            {
                stream.SendNext ( b );
            }

            packetData.Clear ( );
        }

        if ( stream.isReading )
        {
            int num = (int)stream.ReceiveNext();

            for ( int counter = 0; counter < num; ++counter )
            {
                byte[] data = (byte[])stream.ReceiveNext();

                DeserializeAndQueuePacketData ( data );
            }
        }
    }

    private bool notReadyForSerialization
    {
        get
        {
            return ( !PhotonNetwork.inRoom || ( PhotonNetwork.room.PlayerCount < 2 ) ||
                    !Oculus.Platform.Core.IsInitialized ( ) || !ovrAvatar.Initialized );
        }
    }
}
