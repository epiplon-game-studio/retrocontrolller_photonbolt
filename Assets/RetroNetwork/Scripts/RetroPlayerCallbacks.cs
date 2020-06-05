namespace Epiplon.Samples.Network
{
    [BoltGlobalBehaviour("Arena")]
    public class RetroPlayerCallbacks : Bolt.GlobalEventListener
    {
        public override void ControlOfEntityGained(BoltEntity entity)
        {
            NetworkedPlayer networkedPlayer = entity.GetComponent<NetworkedPlayer>();
            networkedPlayer.EnableClientCameras();
        }
    }
}
