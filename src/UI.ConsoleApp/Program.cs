using Azure.Messaging.ServiceBus;
using Azure;
using Azure.Messaging.ServiceBus.Administration;
using System.Text.RegularExpressions;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Introduce la ConnectionString del Service Bus: ");
        string connectionString = Console.ReadLine();

        await using ServiceBusClient client = new(connectionString);
        ServiceBusAdministrationClient adminClient = new(connectionString);

        string pattern = @"[a-fA-F0-9]{8}(-[a-fA-F0-9]{4}){3}-[a-fA-F0-9]{12}";
        Regex regex = new(pattern);

        Console.WriteLine($"Recorriendo colas...");

        AsyncPageable<QueueProperties> queues = adminClient.GetQueuesAsync();

        await foreach (QueueProperties queue in queues)
        {
            if (regex.IsMatch(queue.Name))
            {
                await adminClient.DeleteQueueAsync(queue.Name);
                Console.WriteLine($"Cola eliminada: {queue.Name}");
            }
        }

        Console.WriteLine("Colas eliminadas! Cerrando en 3 segundos...");
        Thread.Sleep(TimeSpan.FromSeconds(3));
    }
}
