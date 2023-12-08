﻿using MinecraftProxy.Network.Protocol.Forwarding;
using MinecraftProxy.Network.Protocol.Packets;
using MinecraftProxy.Network.Protocol.Packets.Clientbound;
using MinecraftProxy.Network.Protocol.Packets.Serverbound;
using MinecraftProxy.Network.Protocol.Registry;
using MinecraftProxy.Network.Protocol.States.Custom;

namespace MinecraftProxy.Network.Protocol.States.Common;

public class LoginState(Player player) : ProtocolState, IPlayableState
{
    protected override StateRegistry Registry { get; } = Registries.LoginStateRegistry;
    
    private byte[]? verifyToken;

    public async Task<bool> HandleAsync(LoginStartPacket packet)
    {
        player.SetGameProfile(new()
        {
            Id = packet.Guid,
            Name = packet.Username
        });

        var encryptionRequestPacket = GenerateEncryptionRequest();
        verifyToken = encryptionRequestPacket.VerifyToken;

        await player.SendPacketAsync(PacketDirection.Clientbound, encryptionRequestPacket);
        return true; // cancelling as we will send login packet to server later with online game profile
    }

    public Task<bool> HandleAsync(EncryptionRequestPacket packet)
    {
        throw new Exception("Backend server is in online-mode");
    }

    public async Task<bool> HandleAsync(EncryptionResponsePacket packet)
    {
        if (verifyToken is null)
            throw new Exception("Encryption verify token is not set yet");

        if (!Proxy.RSA.Decrypt(packet.VerifyToken, false).SequenceEqual(verifyToken))
            throw new Exception("Unable to verify encryption token");

        var secret = Proxy.RSA.Decrypt(packet.SharedSecret, false);
        player.EnableEncryption(PacketDirection.Clientbound, secret);

        var compressionPacket = new SetCompressionPacket { Threshold = Proxy.CompressionThreshold };
        await player.SendPacketAsync(PacketDirection.Clientbound, compressionPacket);
        player.EnableCompression(PacketDirection.Clientbound, Proxy.CompressionThreshold);

        await player.RequestGameProfileAsync(secret);
        await player.SendPacketAsync(PacketDirection.Serverbound, GenerateLoginStartPacket());

        return true;
    }

    public async Task<bool> HandleAsync(SetCompressionPacket packet)
    {
        if (packet.Threshold > 0)
            player.EnableCompression(PacketDirection.Serverbound, packet.Threshold);

        return true; // we should complete encryption before sending compression packet
    }

    public async Task<bool> HandleAsync(LoginSuccessPacket packet)
    {
        if (player.GameProfile is null)
            throw new Exception("Game profile not loaded yet");

        if (player.CurrentServer is null)
            throw new Exception("Server not chosen yet");

        if (player.CurrentServer.Forwarding is not NoneForwarding)
        {
            if (packet.Guid != player.GameProfile.Id)
                throw new Exception($"Server sent wrong player UUID: {packet.Guid}, online is: {player.GameProfile.Id}");
        }
        else
        {
            // fallback to offline GameProfile
            player.GameProfile.Id = packet.Guid;
            player.GameProfile.Name = packet.Username;
            player.GameProfile.Properties = packet.Properties;
        }

        if (player.ProtocolVersion < ProtocolVersion.MINECRAFT_1_20_2)
            player.SwitchState(4);

        return false;
    }

    public Task<bool> HandleAsync(LoginAcknowledgedPacket packet)
    {
        player.SwitchState(3);
        return Task.FromResult(false);
    }

    public async Task<bool> HandleAsync(LoginPluginRequest packet)
    {
        if (player.CurrentServer is null)
            throw new Exception("Server not chosen yet");

        if (player.CurrentServer.Forwarding is not ModernForwarding forwarding)
            return false;

        if (!packet.Identifier.Equals("velocity:player_info"))
            return false;

        var data = forwarding.GenerateForwardingData(packet.Data, player);
        await player.SendPacketAsync(PacketDirection.Serverbound, new LoginPluginResponse
        {
            MessageId = packet.MessageId,
            Successful = true,
            Data = data
        });

        return true;
    }

    public Task<bool> HandleAsync(LoginPluginResponse packet)
    {
        return Task.FromResult(false);
    }

    public EncryptionRequestPacket GenerateEncryptionRequest()
    {
        var verify = new byte[4];
        Random.Shared.NextBytes(verify);

        return new()
        {
            PublicKey = Proxy.RSA.ExportSubjectPublicKeyInfo(),
            VerifyToken = verify
        };
    }

    public LoginStartPacket GenerateLoginStartPacket()
    {
        if (player.GameProfile is null)
            throw new Exception("Can't proceed login as we do not have online GameProfile");

        return new()
        {
            Guid = player.GameProfile.Id,
            Username = player.GameProfile.Name
        };
    }

    public Task<bool> HandleAsync(DisconnectPacket packet)
    {
        return Task.FromResult(false);
    }
}