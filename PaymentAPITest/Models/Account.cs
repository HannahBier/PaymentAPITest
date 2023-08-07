namespace PaymentAPITest.Models
{
    sealed class Account
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public ICollection<Payment> Payments { get; set; }

        public Account(long id, string firstName, string lastName, decimal balance, ICollection<Payment> payments)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Balance = balance;
            Payments = payments;
        }
    }
}
