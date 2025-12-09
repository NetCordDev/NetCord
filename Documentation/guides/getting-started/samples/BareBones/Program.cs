using NetCord.Gateway;

// TODO: Complete sample code for bare bones approach
var token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")!;

var client = new GatewayClient(token);

await client.StartAsync();
await Task.Delay(-1);
