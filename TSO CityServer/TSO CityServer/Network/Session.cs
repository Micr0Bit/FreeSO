﻿/*The contents of this file are subject to the Mozilla Public License Version 1.1
(the "License"); you may not use this file except in compliance with the
License. You may obtain a copy of the License at http://www.mozilla.org/MPL/

Software distributed under the License is distributed on an "AS IS" basis,
WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for
the specific language governing rights and limitations under the License.

The Original Code is the TSO CityServer.

The Initial Developer of the Original Code is
Mats 'Afr0' Vederhus. All Rights Reserved.

Contributor(s): ______________________________________.
*/

using System;
using System.Collections.Generic;
using System.Text;
using CityDataModel;
using GonzoNet;

namespace TSO_CityServer.Network
{
    /// <summary>
    /// A session (game) in progress.
    /// </summary>
    public class Session
    {
        private Dictionary<NetworkClient, Character> m_PlayingCharacters = new Dictionary<NetworkClient, Character>();

        /// <summary>
        /// Adds a player to the current session.
        /// </summary>
        /// <param name="Client">The player's client.</param>
        /// <param name="Char">The player's character.</param>
        public void AddPlayer(NetworkClient Client, Character Char)
        {
            lock (m_PlayingCharacters)
            {
                foreach (KeyValuePair<NetworkClient, Character> KVP in m_PlayingCharacters)
                {
                    ClientPacketSenders.SendPlayerJoinSession(KVP.Key, Char);
                    //Send a bunch of these in reverse (to the player joining the session).
                    ClientPacketSenders.SendPlayerJoinSession(Client, KVP.Value);
                }

                m_PlayingCharacters.Add(Client, Char);
            }
        }

        /// <summary>
        /// Removes a player from the current session.
        /// </summary>
        /// <param name="Client">The player's client.</param>
        public void RemovePlayer(NetworkClient Client)
        {
            lock (m_PlayingCharacters)
            {
                m_PlayingCharacters.Remove(Client);

                foreach (KeyValuePair<NetworkClient, Character> KVP in m_PlayingCharacters)
                    ClientPacketSenders.SendPlayerLeftSession(KVP.Key, m_PlayingCharacters[Client]);
            }
        }

        /// <summary>
        /// Gets a player's character from the session.
        /// </summary>
        /// <param name="GUID">The GUID of the character to retrieve.</param>
        /// <returns>A Character instance, null if not found.</returns>
        public Character GetPlayer(string GUID)
        {
            lock (m_PlayingCharacters)
            {
                foreach (KeyValuePair<NetworkClient, Character> KVP in m_PlayingCharacters)
                {
                    if (KVP.Value.GUID.ToString().Equals(GUID, StringComparison.CurrentCultureIgnoreCase))
                        return KVP.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a player's client from the session.
        /// </summary>
        /// <param name="GUID">The GUID of the character.</param>
        /// <returns>A NetworkClient instance, null if not found.</returns>
        public NetworkClient GetPlayersClient(string GUID)
        {
            lock (m_PlayingCharacters)
            {
                foreach (KeyValuePair<NetworkClient, Character> KVP in m_PlayingCharacters)
                {
                    if (KVP.Value.GUID.ToString().Equals(GUID, StringComparison.CurrentCultureIgnoreCase))
                        return KVP.Key;
                }
            }

            return null;
        }
    }
}