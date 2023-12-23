﻿namespace Void.Proxy.Network.IO.Common;

public class PacketStream(Stream baseStream) : Stream
{
    public override bool CanRead => baseStream.CanRead;

    public override bool CanSeek => baseStream.CanSeek;

    public override bool CanWrite => baseStream.CanWrite;

    public override long Length => baseStream.Length;

    public override long Position { get => baseStream.Position; set => baseStream.Position = value; }

    public override int Read(byte[] buffer, int offset, int count) => baseStream.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);

    public override void SetLength(long value) => baseStream.SetLength(value);

    public override void Flush() => baseStream.Flush();

    public override async Task FlushAsync(CancellationToken cancellationToken) => await baseStream.FlushAsync(cancellationToken);

    public override void Write(byte[] buffer, int offset, int count) => baseStream.Write(buffer, offset, count);

    public override ValueTask<int> ReadAsync(Memory<byte> output, CancellationToken cancellationToken = default) => throw new NotSupportedException("Use ReadPacketAsync");

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> output, CancellationToken cancellationToken = default) => throw new NotSupportedException("Use WritePacketAsync");

    public async ValueTask<MinecraftMessage> ReadPacketAsync(CancellationToken cancellationToken = default)
    {
        var length = await baseStream.ReadVarIntAsync(cancellationToken);
        return await baseStream.ReadMessageAsync(length, cancellationToken);
    }

    public async ValueTask WritePacketAsync(MinecraftMessage message, CancellationToken cancellationToken = default)
    {
        await baseStream.WriteVarIntAsync(message.Length + MinecraftBuffer.GetVarIntSize(message.PacketId), cancellationToken);
        await baseStream.WriteVarIntAsync(message.PacketId, cancellationToken);
        await baseStream.WriteAsync(message.Memory, cancellationToken);
    }
}