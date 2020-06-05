using UnityEngine;
using vnc;

namespace Epiplon.Samples.Network
{
    public class PlayerObject
    {
        public BoltEntity character;
        public BoltConnection connection;

        public bool IsServer
        {
            get { return connection == null; }
        }

        public bool IsClient
        {
            get { return connection != null; }
        }

        public void Spawn()
        {
            var point = SpawnPoint.Get();

            if (!character)
            {
                character = BoltNetwork.Instantiate(BoltPrefabs.Newtorked_Sample_Controller, point.transform.position, Quaternion.identity);

                if (IsServer)
                {
                    character.TakeControl();
                }
                else
                {
                    character.AssignControl(connection);
                }
            }

            // teleport entity to a random spawn position
            var retroController = character.GetComponent<RetroController>();
            retroController.TeleportTo(point.transform.position, true);
        }
    }
}
