using Unity.Netcode.Components;

namespace Extreal.SampleApp.Holiday.Common.Multiplay
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
