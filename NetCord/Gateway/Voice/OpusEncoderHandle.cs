using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

internal class OpusEncoderHandle() : SafeHandle(0, true)
{
    public override bool IsInvalid => handle is 0;

    protected override bool ReleaseHandle()
    {
        Opus.OpusEncoderDestroy(handle);
        return true;
    }
}
