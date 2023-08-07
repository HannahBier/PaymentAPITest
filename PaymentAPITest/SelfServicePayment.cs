using PaymentAPITest.Models;
using PaymentAPITest.APIHandlers;
using System.Net;

namespace PaymentAPITest
{
    [TestClass]
    public class SelfServicePayment
    {
        [ClassInitialize]
        public static async Task InitializeAccount(TestContext context)
        {
            await AccountAPIHandler.RunAsync();
        }

        [TestMethod]
        public async Task PayFullBalance()
        {
            //Get test account
            int accountId = 123;
            Account account = await AccountAPIHandler.GetAccountAsync(accountId);
            decimal initialBalance = account.Balance;
            ICollection<Payment> initialPayments = account.Payments;

            //Make payment
            int paymentId = 111;
            decimal paymentAmount = initialBalance;
            Payment payment = new Payment (paymentId, account.Id, paymentAmount, DateTime.Now);
            HttpResponseMessage paymentResponse = await AccountAPIHandler.MakePaymentAsync(payment);

            //Verify results
                        Assert.IsTrue(paymentResponse.IsSuccessStatusCode, "Failed to POST the new payment");
            account = await AccountAPIHandler.GetAccountAsync(account.Id);
            Assert.AreEqual(0, account.Balance, "Account balance was non-zero after making a full payment");
            Assert.AreEqual((initialPayments.Count + 1), account.Payments.Count, "Incorrect number of payment records found");
            payment = account.Payments.Last();
            Assert.AreEqual(paymentAmount, payment.Amount, "Payment record did not match amount submitted");
        }

        [TestMethod]
        public async Task PayPartialBalance()
        {
            //Get test account
            int accountId = 456;
            Account account = await AccountAPIHandler.GetAccountAsync(accountId);
            decimal initialBalance = account.Balance;
            ICollection<Payment> initialPayments = account.Payments;

            //Make payment
            int paymentId = 222;
            decimal paymentAmount = Math.Round((initialBalance / 4), 2); //Pay a quarter of the current balance
            Payment payment = new Payment(paymentId, account.Id, paymentAmount, DateTime.Now);
            HttpResponseMessage paymentResponse = await AccountAPIHandler.MakePaymentAsync(payment);

            //Verify results
            Assert.IsTrue(paymentResponse.IsSuccessStatusCode, "Failed to POST the new payment");
            account = await AccountAPIHandler.GetAccountAsync(account.Id);
            Assert.AreEqual(initialBalance - paymentAmount, account.Balance, "Account balance was not correctly updated after the payment");
            Assert.AreEqual((initialPayments.Count + 1), account.Payments.Count, "Incorrect number of payment records found");
            payment = account.Payments.Last();
            Assert.AreEqual(paymentAmount, payment.Amount, "Payment record did not match amount submitted");
        }

        [TestMethod]
        public async Task OverpayBalance()
        {
            //Get test account
            int accountId = 789;
            Account account = await AccountAPIHandler.GetAccountAsync(accountId);
            decimal initialBalance = account.Balance;
            ICollection<Payment> initialPayments = account.Payments;

            //Make payment
            int paymentId = 333;
            decimal paymentAmount = initialBalance + 0.01m;
            Payment payment = new Payment(paymentId, account.Id, paymentAmount, DateTime.Now);
            HttpResponseMessage paymentResponse = await AccountAPIHandler.MakePaymentAsync(payment);

            //Verify results
            Assert.AreEqual(HttpStatusCode.BadRequest, paymentResponse.StatusCode, "Did not get expected error code for overpayment");
            account = await AccountAPIHandler.GetAccountAsync(account.Id);
            Assert.AreEqual(initialBalance, account.Balance, "Account balance was updated after an invalid overpayment request");
            Assert.AreEqual(initialPayments.Count, account.Payments.Count, "New payments were added for an invalid overpayment request");
        }

    }
}