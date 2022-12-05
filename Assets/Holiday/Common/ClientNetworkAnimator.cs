using Unity.Netcode.Components;

namespace Extreal.SampleApp.Holiday.Common
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
