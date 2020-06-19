using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureDemo
{
    public static class OnPaymentReceived
    {
        [FunctionName("OnPaymentReceived")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Queue("order")] IAsyncCollector<Order> orderQueue,ILogger log)
        {
            log.LogInformation("Received Payment");            

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order order = JsonConvert.DeserializeObject<Order>(requestBody);
            await orderQueue.AddAsync(order);

            log.LogInformation($"Order {order.OrderId} recevied from {order.Email} for product {order.ProductId}");

            return order != null
                ? (ActionResult)new OkObjectResult($"Thank you for your purchase")
                : new BadRequestObjectResult("Bad Request");
        }
    }

    public class Order
    {
        public int OrderId { get; set; }

        public string Email { get; set; }

        public int ProductId { get; set; }

    }
}
