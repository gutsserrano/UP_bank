namespace AccountAPI.Settings
{
    public interface IMongoDataSettings
    {
        public string AccountCollectionName { get; set; }
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
