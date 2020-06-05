namespace Epiplon.Samples.Network
{
    [BoltGlobalBehaviour(BoltNetworkModes.Server, "Arena")]
    public class RetroServerCallbacks : Bolt.GlobalEventListener
    {
        private void Awake()
        {
            PlayerObjectRegistry.CreateServerPlayer();
        }

        public override void Connected(BoltConnection connection)
        {
            PlayerObjectRegistry.CreateClientPlayer(connection);
        }

        public override void SceneLoadLocalDone(string scene)
        {
            PlayerObjectRegistry.ServerPlayer.Spawn();
        }

        public override void SceneLoadRemoteDone(BoltConnection connection)
        {
            var playerObject = PlayerObjectRegistry.GetPlayer(connection);
            playerObject.Spawn();
        }
    }
}
