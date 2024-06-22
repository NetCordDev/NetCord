using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

internal class OpusDecoderHandle() : SafeHandle(0, true)
{
    public override bool IsInvalid => handle is 0;

    protected override bool ReleaseHandle()
    {
        Opus.OpusDecoderDestroy(handle);
        return true;
    }
}
