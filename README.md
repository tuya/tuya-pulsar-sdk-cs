# tuya-pulsar-sdk-cs
# c# pulsar consumer client sdk example
# Support net framework versions
```
    net9.0
    <TargetFramework>net9.0</TargetFramework>
```

# Install requirements

## pulsar client
```
    # current pulsar version ==3.4.0
    # more info: https://github.com/apache/pulsar-dotpulsar or https://pulsar.apache.org/docs/4.0.x/client-libraries-dotnet/
    <PackageReference Include="DotPulsar" Version="3.4.0" />
```
## Json
```
    # Json 
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
```


# Required parameters
```
    ACCESS_ID = ""
    ACCESS_KEY = ""
    PULSAR_SERVER_URL = ""
    MQ_ENV=""
```

# PULSAR_SERVER_URL
```
    //CN
    "pulsar+ssl://mqe.tuyacn.com:7285/";
    //US
    "pulsar+ssl://mqe.tuyaus.com:7285/";
    //EU
    "pulsar+ssl://mqe.tuyaeu.com:7285/";
    //IND
    "pulsar+ssl://mqe.tuyain.com:7285/";
```

# Process the messages you receive，business logic is implemented in this method
```
    handleMessage
```