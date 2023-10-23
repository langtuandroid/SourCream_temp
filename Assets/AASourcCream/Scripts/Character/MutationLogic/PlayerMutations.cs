
using System;
using MessagePack;

[MessagePackObject]
[Serializable]
public class PlayerMutations
{
    // Hack to serialize dictionary
    [Serializable]
    public class DictBodyMutation : UnitySerializedDictionary<BodySlot, MutationData> { }

    [Key(0)]
    public DictBodyMutation mutations;
    [Key(1)]
    public int availablePoints;
    [Key(2)]
    public int usedPoints;
}