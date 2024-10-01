﻿using Void.Proxy.API.Events;
using Void.Proxy.API.Events.Encryption;

namespace Void.Proxy.Plugins.ExamplePlugin.Services;

public class DebugService : IEventListener
{
    [Subscribe]
    public void OnSearchServerPrivateKey(SearchServerPrivateKey @event)
    {
        if (@event.Server.Name is not "lobby")
            return;

        @event.Result = Convert.FromHexString("30820276020100300d06092a864886f70d0101010500048202603082025c02010002818100eb0ab790f61d4d1ee2b39698223d505b4cbda41143e7027688382c56e7aeedda2eb2c38c5ae7287f45c48ed6a7b4c54191d326c64504f27cc761e04b58192f486b2e6fbef903c470ae7b4583c81632558b50dcdd97759f174e23d090f34d7ad116406b11a5fccda4e402fbcfc36a5783f400872f1e9bb53104ba83633d360bd3020301000102818019d449abf27ff1d3ad12134090b2b03bf848f6d8b6df9213b89083bee123061c6df95327ff6d5bb3f0d4d2e59ff46ba0f307834152a0628d77d3b7b44ff02493d8d0fe8111802e6fd087badd7295d39ee6f819c4c1b890ed3adfc8bad7e4108c3196fc3ddff82872a49a823fc22b03452b274ac0783dc06fed0687791ce07451024100ecc10becd82f0547077fef1dc2232d3c29f4d57ef7e977ffd7bd7b131563db70bbc530e61f407c3f001f875a19ad6d346efac7af909896cd480274833e98dbff024100fe2609d121fec3920d15ce131443b3880c70d513da6f68a527982edde3a382c9d91fcf4a045a56d7f4573b81853d9c5123c0caf3d32d1c0e1758235cad28a02d024025d1ccbadfe9daf8f6bcbc10cfe358a584ba44a48cabb1ec9fa4f8151b54a14847e67f223399d47f27e0e1794622cb10162e5f59af4a80c4781d544966e57a3702406b654b94b256e3f1ddd1af0964f0cef6d8bafd6fac4893e1e67f6a9e9d49454562990c916c91784d3d957731de4a10ba40ef8153d393265dd6038abde8f657e9024100bf1940ff91189f4d15cbbe9d243cdd8b29074189906a98e1fa7b82fbe078e00dab30ea2d82ea6cb99855ace25296d6f75b87b392bfbfaec8a7617d16d78b93df");
    }
}