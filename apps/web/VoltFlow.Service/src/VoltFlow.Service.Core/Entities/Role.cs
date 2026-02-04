namespace VoltFlow.Service.Core.Entities
{
    public class Role
    {
        public int IdRole { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
