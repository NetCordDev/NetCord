using System.Runtime.InteropServices;

namespace NetCord.Gateway.Voice;

internal class OpusDecoderHandle : SafeHandle
{
    public OpusDecoderHandle() : base((nint)0, true)
    {
    }

    public override bool IsInvalid => handle == (nint)0;

    protected override bool ReleaseHandle()
    {
        Opus.OpusDecoderDestroy(handle);
        return true;
    }
}