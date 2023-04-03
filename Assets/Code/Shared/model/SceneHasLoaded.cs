using shared;

public class SceneHasLoaded : ISerializable
{
    public string scene;

    public SceneHasLoaded() { }
    public SceneHasLoaded(string scene)
    {
        this.scene = scene;
    }

    public override void Deserialize(Packet pPacket)
    {
        scene = pPacket.ReadString();
    }

    public override void Serialize(Packet pPacket)
    {
        pPacket.Write(scene);   
    }
}
