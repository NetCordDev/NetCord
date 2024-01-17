namespace NetCord.Services;

[Flags]
internal enum PermissionsType : byte
{
    Channel = 1 << 0,
    Guild = 1 << 1,
}
