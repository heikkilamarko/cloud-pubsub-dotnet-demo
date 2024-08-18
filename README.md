# Cloud Pub/Sub Demo

## Run the Pub/Sub Emulator

### Start Emulator

```bash
docker compose up -d
```

### Create Topics and Subscriptions

The HTTP requests can be found in `pubsub.http`.

## Run the App

```bash
cd src
dotnet run
```

## Send Messages with Postman

### Request URL:

```bash
POST http://0.0.0.0:8085/v1/projects/__PROJECT__/topics/__TOPIC__:publish
```

Examples:

```bash
POST http://0.0.0.0:8085/v1/projects/local/topics/example1:publish
```

```bash
POST http://0.0.0.0:8085/v1/projects/local/topics/example2:publish
```

### Pre-request Script:

```js
pm.collectionVariables.set(
    "data_b64",
    btoa(JSON.stringify({ message: "example" }))
);
```

### Request Body:

```json
{
    "messages": [
        {
            "data": "{{data_b64}}"
        }
    ]
}
```
