namespace AccountAPI.Settings
{
    public class MongoDataSettings : IMongoDataSettings
    {
        public string AccountCollectionName { get; set; }
        public string AccountHistoryCollectionName { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
