﻿using Void.Proxy.API.Network.Protocol;
using Void.Proxy.Common.Network.Protocol;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Clientbound;
using Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Packets.Serverbound;

namespace Void.Proxy.Plugins.ProtocolSupport.Java.v1_7_2_to_1_12_2.Registries;

public static class Mappings
{
    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundHandshakeMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundHandshakeMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(HandshakePacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionRequestPacket) },
        { [new PacketMapping(0x02, Plugin.SupportedVersions.First())], typeof(LoginSuccessPacket) },
        { [new PacketMapping(0x03, ProtocolVersion.MINECRAFT_1_8)], typeof(SetCompressionPacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundLoginMappings = new Dictionary<PacketMapping[], Type>
    {
        { [new PacketMapping(0x00, Plugin.SupportedVersions.First())], typeof(LoginStartPacket) },
        { [new PacketMapping(0x01, Plugin.SupportedVersions.First())], typeof(EncryptionResponsePacket) }
    };

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ClientboundPlayMappings = new Dictionary<PacketMapping[], Type>();

    public static readonly IReadOnlyDictionary<PacketMapping[], Type> ServerboundPlayMappings = new Dictionary<PacketMapping[], Type>();

    public static void Fill()
    {
        // will initialize static fields
    }
}