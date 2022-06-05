using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

internal class OpusEncoderHandle : SafeHandle
{
    public OpusEncoderHandle() : base((nint)0, true)
    {
    }

    public override bool IsInvalid => handle == (nint)0;

    protected override bool ReleaseHandle()
    {
        Opus.OpusEncoderDestroy(handle);
        return true;
    }
}