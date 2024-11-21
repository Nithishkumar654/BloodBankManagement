namespace BloodBank.Models
{
    public class BloodBankEntry
    {
        public string? Id { get; set; }
        public string? DonorName { get; set; }
        public int? Age { get; set; }
        public string? BloodType { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CollectionDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? Status { get; set; }
    }
}
