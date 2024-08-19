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
cd src/App
dotnet run
```

## Send Messages

The HTTP requests can be found in `app.http`.

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
const btoa = require("btoa");
const uuid = require("uuid");

pm.collectionVariables.set(
    "valid_data",
    btoa(JSON.stringify({ id: uuid.v4(), name: "example" }))
);

pm.collectionVariables.set(
    "invalid_data",
    btoa(JSON.stringify({ id: 123, name: "example" }))
);
```

### Request Body:

```json
{
    "messages": [
        {
            "data": "{{valid_data}}"
        },
        {
            "data": "{{invalid_data}}"
        }
    ]
}
```
