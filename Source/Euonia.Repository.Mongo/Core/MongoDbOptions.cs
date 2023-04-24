namespace Nerosoft.Euonia.Repository.Mongo;

public class MongoDbOptions
{
    public string ConnectionString { get; private set; }

    public string Database { get; private set; }

    public MongoDbOptions UseConnection(string connection)
    {
        ConnectionString = connection;
        return this;
    }

    public MongoDbOptions UseDatabase(string database)
    {
        Database = database;
        return this;
    }
}