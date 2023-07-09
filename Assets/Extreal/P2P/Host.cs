namespace Extreal.P2P.Dev
{
    public class Host
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public Host(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
