using Unity.Netcode.Components;

namespace Extreal.SampleApp.Holiday.MultiplayCommon
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}
