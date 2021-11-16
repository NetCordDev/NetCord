namespace NetCord
{
    public class UserActivityButton
    {
        private readonly JsonModels.JsonUserActivityButton _jsonEntity;

        public string Label => _jsonEntity.Label;

        public string Url => _jsonEntity.Url;

        internal UserActivityButton(JsonModels.JsonUserActivityButton jsonEntity)
        {
            _jsonEntity = jsonEntity;
        }
    }
}