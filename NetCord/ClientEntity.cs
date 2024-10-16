﻿using NetCord.Rest;

namespace NetCord;

public abstract class ClientEntity : Entity
{
    private protected ClientEntity(RestClient client)
    {
        _client = client;
    }

    private protected readonly RestClient _client;
}
