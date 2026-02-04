using VoltFlow.Service.Core.Enums;

namespace VoltFlow.Service.Core.Entities
{
    public class Job
    {
        public int IdJob { get; set; }
        public DateTime Date { get; set; }
        public JobStatus Status { get; set; } // Mapowanie na Enum w Application Layer

        // Relacje
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public int AddressId { get; set; }
        public ClientAddress Address { get; set; } = null!;

        public ICollection<TaskEntity> Tasks { get; set; } = new List<TaskEntity>();

        public ICollection<Job> Jobs { get; set; } = new List<Job>();

        public ICollection<EventJob> EventJobs { get; set; } = new List<EventJob>();

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
