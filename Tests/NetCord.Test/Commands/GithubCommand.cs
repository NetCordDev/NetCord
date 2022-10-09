using System.Text.Json;

using NetCord.Rest;
using NetCord.Services.Commands;

namespace NetCord.Test.Commands;

public class GithubCommand : CommandModule<CommandContext>
{
    [Command("github", "gh")]
    public async Task Github([Remainder] string userName)
    {
        List<EmbedFieldProperties> fields = new();
        EmbedProperties embed = new()
        {
            Fields = fields
        };
        if (Context.User is GuildUser guildUser)
        {
            var roles = guildUser.GetRoles(Context.Guild!).Where(r => r.Color != default);
            if (roles.Any())
            {
                var role = roles.MaxBy(r => r.Position)!;
                embed.Color = role.Color;
            }
            else
                embed.Color = new(0, 255, 0);
        }
        else
            embed.Color = new(0, 255, 0);
        HttpClient client = new();
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
        HttpRequestMessage reposMessage = new(HttpMethod.Get, $"https://api.github.com/users/{userName}/repos");
        var reposTask = client.SendAsync(reposMessage);
        HttpRequestMessage userMessage = new(HttpMethod.Get, $"https://api.github.com/users/{userName}");
        var userTask = client.SendAsync(userMessage);
        var repos = await reposTask;
        var user = await userTask;
        if (!repos.IsSuccessStatusCode || !user.IsSuccessStatusCode)
        {
            await ReplyAsync("User not found!");
            return;
        }
        var jsonRepos = JsonDocument.Parse(await repos.Content.ReadAsStreamAsync()).RootElement;
        var jsonUser = JsonDocument.Parse(await user.Content.ReadAsStreamAsync()).RootElement;
        var avatarUrl = jsonUser.GetProperty("avatar_url").GetString();
        embed.Author = new()
        {
            IconUrl = avatarUrl,
            Url = $"https://github.com/{userName}",
            Name = jsonUser.GetProperty("name").GetString() ?? jsonUser.GetProperty("login").GetString()
        };
        embed.Description = jsonUser.GetProperty("bio").GetString();
        embed.Thumbnail = avatarUrl;

        var i = 3;

        fields.Add(new() { Title = "Followers 👀", Description = jsonUser.GetProperty("followers").GetInt32().ToString() });

        fields.Add(new() { Title = "Following 👀", Description = jsonUser.GetProperty("following").GetInt32().ToString() });
        var email = jsonUser.GetProperty("email").GetString();
        if (email != null)
        {
            fields.Add(new() { Title = "Email 📧", Description = $"[{email}](mailto:{email})" });
            i++;
        }
        var location = jsonUser.GetProperty("location").GetString();
        if (location != null)
        {
            fields.Add(new() { Title = "Location <:location:888438681420050484>", Description = location });
            i++;
        }

        var reposList = jsonRepos.EnumerateArray();
        var first = reposList.First();
        var name = first.GetProperty("name").GetString();
        var fullname = first.GetProperty("full_name").GetString();
        fields.Add(new() { Title = "Repos:", Description = $"[**{name}**]({"https://github.com/" + fullname})", Inline = true });
        foreach (var repo in reposList.Skip(1))
        {
            name = repo.GetProperty("name").GetString();
            fullname = repo.GetProperty("full_name").GetString();
            fields.Add(new() { Description = $"[**{name}**]({"https://github.com/" + fullname})", Inline = true });
            if (i == 25)
                break;
        }
        MessageProperties message = new()
        {
            Embeds = new List<EmbedProperties>() { embed }
        };
        await SendAsync(message);
    }

    public class GithubUser
    {

    }
}
