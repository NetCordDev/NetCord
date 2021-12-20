namespace NetCord;

public partial class RestClient
{
    public class UserModule
    {
        private readonly BotClient _client;

        internal UserModule(BotClient client)
        {
            _client = client;
        }

        public string GetAvatarUrl(DiscordId userId, string avatarHash, ImageFormat? format)
        {
            if (avatarHash != null)
                return $"{Discord.ImageBaseUrl}/avatars/{userId}/{avatarHash}.{(format.HasValue ? InternalHelper.GetImageExtension(format.GetValueOrDefault()) : avatarHash.StartsWith("a_") ? "gif" : "png")}";
            else
                throw new InvalidOperationException("This user has no avatar");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="avatarHash"></param>
        /// <param name="format"></param>
        /// <param name="size">any power of two between 16 and 4096</param>
        /// <returns></returns>
        public string GetAvatarUrl(DiscordId userId, string avatarHash, int size, ImageFormat? format)
        {
            if (avatarHash != null)
                return $"{Discord.ImageBaseUrl}/avatars/{userId}/{avatarHash}.{(format.HasValue ? InternalHelper.GetImageExtension(format.GetValueOrDefault()) : avatarHash.StartsWith("a_") ? "gif" : "png")}?size={size}";
            else
                throw new InvalidOperationException("This user has no avatar");
        }

        public string GetDefaultAvatarUrl(ushort discriminator) => $"{Discord.ImageBaseUrl}/embed/avatars/{discriminator % 5}.png";
    }
}