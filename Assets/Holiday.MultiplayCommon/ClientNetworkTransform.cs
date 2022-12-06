using Unity.Netcode.Components;

namespace Extreal.SampleApp.Holiday.Common
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
