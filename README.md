# Tuya Pulsar SDK for C#

This project provides a C# implementation example of a Pulsar consumer client for the Tuya ecosystem. It demonstrates how to connect to Tuya's message queue service and process messages securely.

## Prerequisites

- .NET 9.0 or higher
- Tuya account with access credentials (ACCESS_ID and ACCESS_KEY)

## Installation

### Required NuGet Packages

```xml
<PackageReference Include="DotPulsar" Version="3.4.0" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
```

## Configuration

### Required Parameters

To use this SDK, you need to configure the following parameters:

```csharp
ACCESS_ID = "your_access_id"
ACCESS_KEY = "your_access_key"
PULSAR_SERVER_URL = "your_server_url"
MQ_ENV = "your_environment"  // "event" for production, "event-test" for testing
```

### Available Server URLs

Choose the appropriate server URL based on your region:

```csharp
// China
"pulsar+ssl://mqe.tuyacn.com:7285/"

// United States
"pulsar+ssl://mqe.tuyaus.com:7285/"

// Europe
"pulsar+ssl://mqe.tuyaeu.com:7285/"

// India
"pulsar+ssl://mqe.tuyain.com:7285/"
```

## Usage

The SDK provides a simple consumer implementation that handles message decryption and processing. To use it:

1. Configure your credentials and server URL
2. Initialize the Pulsar client
3. Create a consumer
4. Process incoming messages through the `handleMessage` method

### Basic Example

```csharp
// Initialize client
await using var client = PulsarClient.Builder()
    .ServiceUrl(new Uri(PULSAR_SERVER_URL))
    .Authentication(new MyAuthentication(ACCESS_ID, ACCESS_KEY))
    .Build();

// Create consumer
await using var consumer = client.NewConsumer()
    .SubscriptionName(subscription)
    .Topic(topic)
    .SubscriptionType(SubscriptionType.Failover)
    .Create();
```

## Message Processing

Messages are automatically decrypted using either AES-GCM or AES-ECB depending on the encryption mode specified in the message properties. You can implement your business logic in the `handleMessage` method.

## Security

This SDK implements secure authentication and encryption:
- Token-based authentication using MD5 hashing
- Message encryption using AES-GCM and AES-ECB
- SSL connection to Tuya's servers
