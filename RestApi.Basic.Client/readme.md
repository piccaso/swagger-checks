# autorest config
> see https://aka.ms/autorest

## requirements
- nodejs v8 (^8.11.3)

## setup 
```sh
npm install
```

## usage
```sh
npm run build
```

## config
``` yaml 
input-file: http://localhost:50230/swagger/v1/swagger.json

csharp:
  namespace: RestApi.Basic.Client.Autorest
  output-folder: Autorest
  # fluent: true
  sync-methods: all
  # add-credentials: true
  # override-client-name: MyClient
```