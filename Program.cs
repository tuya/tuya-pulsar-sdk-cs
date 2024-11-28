using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;

namespace TuyaPulsar
{
    public class Program
    {
        // Server URLs for different regions
        private const string CN_SERVER_URL = "pulsar+ssl://mqe.tuyacn.com:7285/";
        private const string US_SERVER_URL = "pulsar+ssl://mqe.tuyaus.com:7285/";
        private const string EU_SERVER_URL = "pulsar+ssl://mqe.tuyaeu.com:7285/";
        private const string IND_SERVER_URL = "pulsar+ssl://mqe.tuyain.com:7285/";

        // Environment settings
        private const string MQ_ENV_PROD = "event";
        private const string MQ_ENV_TEST = "event-test";

        // Configuration parameters
        private const string ACCESS_ID = "";      // Your Tuya Access ID
        private const string ACCESS_KEY = "";     // Your Tuya Access Key
        private const string PULSAR_SERVER_URL = CN_SERVER_URL;
        private const string MQ_ENV = MQ_ENV_PROD;

        public static async Task Main()
        {
            // Configure topic and subscription
            string topic = $"persistent://{ACCESS_ID}/out/{MQ_ENV}";
            string subscription = $"{ACCESS_ID}-sub";

            // Create authentication instance
            IAuthentication auth = new MyAuthentication(ACCESS_ID, ACCESS_KEY);

            // Initialize Pulsar client
            await using var client = PulsarClient.Builder()
                .ServiceUrl(new Uri(PULSAR_SERVER_URL))
                .Authentication(auth)
                .Build();

            // Create consumer
            await using var consumer = client.NewConsumer()
                .SubscriptionName(subscription)
                .Topic(topic)
                .SubscriptionType(SubscriptionType.Failover)
                .Create();

            // Process messages
            await foreach (var message in consumer.Messages())
            {
                try
                {
                    string messageId = GetMessageId(message.MessageId);
                    Console.WriteLine($"Received message: {messageId}");

                    string decryptedData = AesUtil.DecryptMessage(message, ACCESS_KEY);
                    Console.WriteLine($"Decrypted message {messageId}: {decryptedData}");

                    await HandleMessage(message, messageId, decryptedData);
                    await consumer.Acknowledge(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                    // Implement your error handling strategy here
                }
            }
        }

        private static string GetMessageId(MessageId messageId)
        {
            return $"{messageId.LedgerId}:{messageId.EntryId}:{messageId.Partition}:{messageId.BatchIndex}";
        }

        private static async Task HandleMessage(IMessage message, string messageId, string decryptedData)
        {
            Console.WriteLine($"Processing message: {messageId}");
            // TODO: Implement your message handling logic here
            await Task.Delay(1); // Placeholder for async operations
            Console.WriteLine($"Finished processing message: {messageId}");
        }
    }
}