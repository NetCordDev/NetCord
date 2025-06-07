using NetCord.JsonModels;

namespace NetCord;

public class EntryPointCommandInteractionData(JsonInteractionData jsonModel) : ApplicationCommandInteractionData(jsonModel)
{
}
