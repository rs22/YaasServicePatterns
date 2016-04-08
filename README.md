# YaaS Microservice Patterns for ASP.NET Core

This repository contains some classes to simplify the development of 
ASP.NET Core Microservices for [YaaS](https://www.yaas.io).

The obligatory Wishlist example is included, featuring
 - Storage using the YaaS Document Service
 - Scope validation
 - Multi-tenant support

**How to setup the example:**
 1. You will need YaaS client credentials to use the Document service.
    Create a service definition in the Teams section of the [Builder]
    (https://builder.yaas.io/) and subscribe to the Document service.
    To deploy the service and test it using the YaaS API proxy, you 
    also have to create a test app which subscribes to the package that
    contains your service definition.
 2. Fill in your service client credentials in the appsettings.json, or -
    even better - using the user-secret tool (Yaas:ClientID, Yaas:ClientSecret).
    Also provide your service identifier in the appsettings.json.
 3. `npm install && gulp api`
 4. `dnu restore && dnx web`
 5. The API console should be available at [localhost:5000](http://localhost:5000)
