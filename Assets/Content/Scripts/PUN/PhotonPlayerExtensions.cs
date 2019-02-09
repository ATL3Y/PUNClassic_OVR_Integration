using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FLOW_PUN
{
    public static class PhotonPlayerExtensions
    {
        public static T GetPlayerProperty<T>(this PhotonPlayer player, string key, T defaultValue = default(T))
        {
            if (player.CustomProperties == null)
            {
                // Debug.LogError("player.CustomProperties == null for player " + player.ID);
                return defaultValue;
            }
                
            else if (!player.CustomProperties.ContainsKey(key))
            {
                // Debug.LogError("!player.CustomProperties.ContainsKey(key) " + key + " for player " + player.ID);
                return defaultValue;
            }
            return (T)player.CustomProperties[key];
        }

        public static void SetPlayerProperty(this PhotonPlayer player, string key, object value)
        {
            // Debug.Log("Player " + player.ID + " property " + key + " was set to " + value);
            var playerProperties = player.CustomProperties;
            playerProperties[key] = value;
            player.SetCustomProperties(playerProperties);
        }
    }
}
