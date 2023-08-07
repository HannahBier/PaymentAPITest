namespace PaymentAPITest.Models
{
    public sealed record Payment(long Id, long AccountId, decimal Amount, DateTime PaymentDate);
}
