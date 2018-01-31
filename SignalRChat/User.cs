namespace SignalRChat
{
    public class User
    {
        public string Name { get; set; }
        public string ConnectionID { get; set; }

        public User(string name, string connectionID)
        {
            Name = name;
            ConnectionID = connectionID;
        }
    }
}