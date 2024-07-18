﻿using System.Net.Sockets;

namespace Void.Proxy.API.Network.IO.Streams;

public class SimpleNetworkStream(NetworkStream baseStream) : IMinecraftNetworkStream
{
    private Memory<byte> _nextBuffer = Memory<byte>.Empty;
    public NetworkStream BaseStream => baseStream;

    public void PrependBuffer(Memory<byte> buffer)
    {
        ArgumentOutOfRangeException.ThrowIfZero(buffer.Length, nameof(buffer));
        _nextBuffer = _nextBuffer.Length > 0 ? ((byte[]) [.. _nextBuffer.ToArray(), .. buffer.ToArray()]).AsMemory() : buffer; // TODO do not allocate
    }

    public int Read(Span<byte> span)
    {
        if (_nextBuffer is not { Length: > 0 })
            return baseStream.Read(span);

        var length = Math.Min(_nextBuffer.Length, span.Length);
        _nextBuffer.Span[..length]
            .CopyTo(span);
        _nextBuffer = _nextBuffer[length..];
        return length;
    }

    public async ValueTask<int> ReadAsync(Memory<byte> memory)
    {
        if (_nextBuffer is not { Length: > 0 })
            return await baseStream.ReadAsync(memory);

        var length = Math.Min(_nextBuffer.Length, memory.Length);
        _nextBuffer[..length]
            .CopyTo(memory);
        _nextBuffer = _nextBuffer[length..];
        return length;
    }

    public void ReadExactly(Span<byte> span)
    {
        if (_nextBuffer.Length >= span.Length)
        {
            _nextBuffer.Span[..span.Length]
                .CopyTo(span);
            _nextBuffer = _nextBuffer[span.Length..];
        }
        else
        {
            var length = _nextBuffer.Length;
            _nextBuffer.Span.CopyTo(span[..length]);
            _nextBuffer = Memory<byte>.Empty;
            baseStream.ReadExactly(span[length..]);
        }
    }

    public async ValueTask ReadExactlyAsync(Memory<byte> memory)
    {
        if (_nextBuffer.Length >= memory.Length)
        {
            _nextBuffer[..memory.Length]
                .CopyTo(memory);
            _nextBuffer = _nextBuffer[memory.Length..];
        }
        else
        {
            var length = _nextBuffer.Length;
            _nextBuffer.CopyTo(memory[..length]);
            _nextBuffer = Memory<byte>.Empty;
            await baseStream.ReadExactlyAsync(memory[length..]);
        }
    }

    public void Write(Span<byte> span)
    {
        baseStream.Write(span);
    }

    public async ValueTask WriteAsync(Memory<byte> memory)
    {
        // Console.WriteLine("write " + Convert.ToHexString(memory.Span));
        await baseStream.WriteAsync(memory);
    }

    public void Flush()
    {
        baseStream.Flush();
    }

    public async ValueTask FlushAsync()
    {
        await baseStream.FlushAsync();
    }

    public void Close()
    {
        baseStream.Close();
    }

    public void Dispose()
    {
        baseStream.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await baseStream.DisposeAsync();
    }
}