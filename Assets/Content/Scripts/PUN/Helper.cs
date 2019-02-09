using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLOW_PUN
{
    // Helper provides templates to update player and room custom data. These static functions can be called from anywhere
    public class Helper : MonoBehaviour
    {
        // HACKL3Y: omitting "where T : MonoBehaviour" so this works with Playables and MonoBehaviour components.
        public static T GetCachedComponent<T>(GameObject gameObject, ref T cachedComponent) 
        {
            if (cachedComponent == null)
            {
                cachedComponent = gameObject.GetComponent<T>();
            }
            return cachedComponent;
        }

        // Helper functions from Within to get/set Room properties. 
        // Example usage: SetRoomProperty("StartTime”, 1000); int startTime = GetRoomProperty<int>("StartTime”, -1);
        public static T GetRoomProperty<T>(string key, T defaultValue = default(T))
        {
            if (!PhotonNetwork.inRoom)
            {
                //Debug.LogError("You are trying to get a room property but you are not in a room.");
                return defaultValue;
            }
            // var is implicitly typed, and the compiler infers the rest
            var room = PhotonNetwork.room;
            if (room.CustomProperties == null)
            {
                //Debug.LogError("room.CustomProperties == null");
                return defaultValue;
            }
                
            else if (!room.CustomProperties.ContainsKey(key))
            {
                //Debug.LogError("!room.CustomProperties.ContainsKey(key)");
                return defaultValue;
            }

            return (T)room.CustomProperties[key];
        }

        public static void SetRoomProperty(string key, object value)
        {
            if (!PhotonNetwork.inRoom)
            {
                //Debug.LogError("You are trying to set a room property but you are not in a room.");
                return;
            }

            var room = PhotonNetwork.room;
            var props = room.CustomProperties;
            props[key] = value;
            room.SetCustomProperties(props);
        }
    }
}
