using System.Net.Http.Headers;
using PaymentAPITest.Models;


namespace PaymentAPITest.APIHandlers
{
    public class AccountAPIHandler
    {
        public static HttpClient client = new HttpClient();

        internal static async Task RunAsync()
        {
            // Update port # in the following line.
            client.BaseAddress = new Uri("https://localhost:7266/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        internal static async Task<Account> GetAccountAsync(long id)
        {
            Account account = null;
            string path = string.Format("/api/accounts/{0}", id);
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                account = await response.Content.ReadAsAsync<Account>();
            }
            return account;
        }

        internal static async Task<Payment> GetPaymentAsync(long paymentId, long accountId)
        {
            Payment payment = null;
            string path = string.Format("/api/accounts/{0}/payments/{1}", accountId, paymentId);
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                payment = await response.Content.ReadAsAsync<Payment>();
            }
            return payment;
        }

        internal static async Task<HttpResponseMessage> MakePaymentAsync(Payment payment)
        {
            string path = string.Format("/api/accounts/{0}/payments", payment.AccountId);
            HttpResponseMessage response = await client.PostAsJsonAsync(
                path, payment);

            return response;
        }

    }
}
