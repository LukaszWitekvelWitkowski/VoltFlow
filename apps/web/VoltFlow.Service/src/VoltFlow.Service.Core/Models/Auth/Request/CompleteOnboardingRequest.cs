namespace VoltFlow.Service.Core.Models.Auth.Request
{
    public class CompleteOnboardingRequest
    {
        public int ClientId { get; init; }
        public required string City { get; init; }
        public required string Street { get; init; }
        public required string ZipCode { get; init; }
        public required string NumberStreet { get; init; }
        public int TypeAddress { get; init; }
    }
}
