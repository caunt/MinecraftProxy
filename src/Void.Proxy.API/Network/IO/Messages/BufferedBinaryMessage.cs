﻿using Void.Proxy.API.Network.IO.Memory;

namespace Void.Proxy.API.Network.IO.Messages;

public class BufferedBinaryMessage(MemoryHolder holder) : IMinecraftMessage
{
    public MemoryHolder Holder => holder;

    public void Dispose()
    {
        holder.Dispose();
    }
}