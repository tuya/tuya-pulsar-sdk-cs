using DotPulsar;
using DotPulsar.Abstractions;
using DotPulsar.Extensions;
using DotPulsar.Internal;
using System.Security.Cryptography;
using System.Text;


// pulsar server url
const string CN_SERVER_URL = "pulsar+ssl://mqe.tuyacn.com:7285/";
const string US_SERVER_URL = "pulsar+ssl://mqe.tuyaus.com:7285/";
const string EU_SERVER_URL = "pulsar+ssl://mqe.tuyaeu.com:7285/";
const string IND_SERVER_URL = "pulsar+ssl://mqe.tuyain.com:7285/";
// env
const string MQ_ENV_PROD = "event";
const string MQ_ENV_TEST = "event-test";

// accessId, accessKey，serverUrl，MQ_ENV
const string ACCESS_ID = "";
const string ACCESS_KEY = "";
const string PULSAR_SERVER_URL = CN_SERVER_URL;
const string MQ_ENV= MQ_ENV_PROD;

const string topic = "persistent://" + ACCESS_ID + "/out/" + MQ_ENV;
const string subscrition = ACCESS_ID + "-sub";

IAuthentication auth = new MyAuthentication(ACCESS_ID, ACCESS_KEY);

// connecting to pulsar://localhost:6650
await using var client = PulsarClient.Builder()
    .ServiceUrl(new System.Uri(PULSAR_SERVER_URL))
    .Authentication(auth)
    .Build();


// consume messages
await using var consumer = client.NewConsumer()
    .SubscriptionName(subscrition)
    .Topic(topic)
    .SubscriptionType(SubscriptionType.Failover)
    .Create();

await foreach (var message in consumer.Messages())
{
    string messageId = getMessageId(message.MessageId);
    Console.WriteLine($"Received: {messageId} EncryptMessage: {Encoding.UTF8.GetString(message.Data)}");
    string decryptData = AesUtil.DecryptMessage(message, ACCESS_KEY);
    Console.WriteLine($"Received: {messageId} DecryptMessage: {decryptData}");
    handleMessage(message, messageId, decryptData);
    await consumer.AcknowledgeCumulative(message);
}

string getMessageId(MessageId messageId) {
    return $"{messageId.LedgerId}:{messageId.EntryId}:{messageId.Partition}:{messageId.BatchIndex}";
}

void handleMessage(IMessage message, string messageId, string decryptData) {
    Console.WriteLine($"handle start : {messageId}");
    // TODO handle message
    Console.WriteLine($"handle finish : {messageId}");
}
