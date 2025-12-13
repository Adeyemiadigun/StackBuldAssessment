using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Application.Models;
using Application.Services;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Payment.Paystack
{
    public class PaystackPaymentGateway : IPaymentGateway
    {
        private readonly HttpClient _http;
        private readonly PaystackOptions _options;

        public PaystackPaymentGateway(
            HttpClient http,
            IOptions<PaystackOptions> options)
        {
            _http = http;
            _options = options.Value;
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _options.SecretKey);
        }

        public async Task<PaymentInitResult> InitiateAsync(
            decimal amount,
            string email,
            string reference,
            CancellationToken ct)
        {
            var payload = new
            {
                email,
                amount = (int)(amount * 100), // Paystack uses kobo
                reference
            };

            var response = await _http.PostAsJsonAsync(
                "transaction/initialize",
                payload,
                ct);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PaystackInitResponse>(ct);

            return new PaymentInitResult(
                true,
                result!.data.authorization_url,
                reference);
        }

        public async Task<PaymentVerifyResult> VerifyAsync(
            string reference,
            CancellationToken ct)
        {
            var response = await _http.GetAsync(
                $"transaction/verify/{reference}",
                ct);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<PaystackVerifyResponse>(ct);

            return new PaymentVerifyResult(

                result.Status,
                result!.Data.Status,
                result.Data.Amount / 100m,
                reference);
        }
    }

}
