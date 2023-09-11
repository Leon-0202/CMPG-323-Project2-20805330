## Table of contents

- [API Link](api-link)
- [WebAPI Overview](#webapi-overview)
- [Authentication](#authentication) 
- [List of Endpoints](#list-of-endpoints) 
- [Stretch Tasks](#stretch-tasks) 
- [References](#references)

## API Link
The API can be accessed on Azure with the following link:\
https://restfullapi.azurewebsites.net/index.html

## WebAPI Overview
The WebAPI was created as, and makes use of the following core features:  
- Created as an ASP.NET Core Web API, using .NET 6.0 
- Makes use of a SQL Server Database, hosted on Azure
- The DbContext, model, and controller classes were scaffolded using Entity Framework Core
- Uses Swagger support
- Implements authentication to secure its endpoints 

The following minor features were also implemented:
- Data Transfer Objects are used to facilitate input and output of data
- JsonPatchDocuments are used when PATCHing existing records in the database

## Stretch Tasks
The JSON payload inherent to the object structure for model classes Customer, Product, Order, and OrderDetail was found to be overly complicated and unwieldy when trying to input data to send to the server.  Furthermore, when using more complicated actions, such as entering a new Order along with the related Order Details, the JSON payload format often lead to circular referencing and cases where the provided input could not satisfy the referential contraints of the database schema.

In order to alleviate this, Data Transfer Objects (DTO) were implemented.  DTOs were created for all four model classes (Customer, Product, Order, and OrderDetail), omitting the navigation object lists of the original classes wherever possible.  Only class OrderDTO retained the list of OrderDetails, in order for the retalionship between each OrderDetail and its associated Order to be communicated to the server.

By using the simplified DTOs as the JSON payload, it was far more practical to enter all the required information for each controller action, as well as easier to reconcile the data sent to the database with its referential constraints.  It also gave more control over what the output of controller responses looked like, making it possible to restrict output to only the desired properties.

In addition to creating the DTO classes, each controller class's action methods that received a model class (Customer, Product, Order) as an argument, had to be modified to implement the respective DTO instead.  Although not originally planned and only seen as a "nice to have", this could fortunately be completed and tested in time.

## Authentication
Before being able to use any of the endpoints, the user must first log in.  If the user is already registered, they can proceed directly to login; otherwise registration is first required before being able to to so.

* Registration
    * Under Authenticate, click on /api/Authenticate/register, and click on the Try it out button.
    * Enter the required input using the following format:\
  {\
  "username": "string",\
  "email": "user@example.com",\
  "password": "string"\
  }
    * Be sure to remember/record your details for future use, and then click on Execute.
    * You should get a response message saying that the user was created successfully.

* Logging in
    * Under Authenticate, click on /api/Authenticate/login, and click on the Try it out button
    * Enter your username and password using the following format:\
  {\
  "username": "string",\
  "password": "string"\
  }
    * You should get a response code 200, along with a token.
    * Copy the token from the response body.
    * Click on the Authorize button at the top of the screen.
    * Enter the following values: "Bearer {paste in token here, and remove braces}".
    * Click on Authorize and then on Close.

## List of Endpoints
The endpoints can be accessed at the following URL:\
https://restfullapi.azurewebsites.net/index.html

Here follows a list of all the endpoints available in the WebAPI. A brief description of each endpoint is given at the end of the list, and can be navigated to by clicking on the link of each method.

Whenever primary keys (e.g., customerID, productID, OrderID) need to be provided as input, make sure to provide a value that is not already in use in the database.  This can be done by using the GET all methods and verifying by inspection whether a key is already used or not.

- Customers
    - [GET All Customers](#get-all-customers)
    - [POST New Customer](#post-new-customer)
    - [GET Specific Customer](#get-specific-customer)
    - [PUT Existing Customer](#put-existing-customer)
    - [DELETE Existing Customer](#delete-existing-customer)
    - [PATCH Existing Customer](#patch-existing-customer)

- Orders
    - [GET All Orders](#get-all-orders)
    - [POST New Order](#post-new-order)
    - [GET Specific Order](#get-specific-order)
    - [PUT Existing Order](#put-existing-order)
    - [DELETE Existing Order](#delete-existing-order)
    - [PATCH Existing Order](#patch-existing-order)
    - [GET Orders Related to Customer](#get-orders-related-to-customer)

- Products
    - [GET All Products](#get-all-products)
    - [POST New Product](#post-new-product)
    - [GET Specific Product](#get-specific-product)
    - [PUT Existing Product](#put-existing-product)
    - [DELETE Existing Product](#delete-existing-product)
    - [PATCH Existing Product](#patch-existing-product)
    - [GET Products Related to Order](#get-products-related-to-order)

## GET All Customers
Returns a list of the details of all the customers.

## POST New Customer
Adds a new customer.  The input is in the following format:\
  {\
  "customerId": 0,\
  "customerTitle": "string",\
  "customerName": "string",\
  "customerSurname": "string",\
  "cellPhone": "string"\
  }

## GET Specific Customer
Returns the details of a specific customer.  The customer is specified by entering their customerID.

## PUT Existing Customer
Edits an existing customer by replacing the entire record with a new record.  The customer is specified by entering their customerID.  The input is in the following format:\
  {\
  "customerId": 0,\
  "customerTitle": "string",\
  "customerName": "string",\
  "customerSurname": "string",\
  "cellPhone": "string"\
  }

## DELETE Existing Customer
Deletes an existing customer.  The customer is specified by entering their customerID.

## PATCH Existing Customer
Edits an existing customer by only changing the desired values.  The customer is specified by entering their customerID.  The input is in the following format:\
[\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  }\
]

For operationType, enter 2.\
For path, enter a '/', followed by the name of the field that you to edit, e.g., "/CustomerName".\
For op, enter "replace".\
Remove the from line.\
For value, enter the new desired value (be consistent with the field's data type).

If there is more than one value to change, additional lists can be nested as such:\
[\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  },\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  },\
]

Remember the ',' used to seperated each list.

## GET All Orders
Returns a list of the details of all the orders, in addition to every order detail associated with them.

## POST New Order
Adds a new order, and associated order details.  The input is in the following format:\
{\
  "orderId": 0,\
  "orderDate": "2023-08-31",\
  "customerId": 0,\
  "deliveryAddress": "string",\
  "orderDetails": [\
    {\
      "orderDetailsId": 0,\
      "orderId": 0,\
      "productId": 0,\
      "quantity": 0,\
      "discount": 0\
    }\
  ]\
}

For orderDate, only the date component needs to be added - the time component may be omitted.\
If there is more than one order detail to add, additional lists can be nested as such:\
{\
  "orderId": 0,\
  "orderDate": "2023-08-31",\
  "customerId": 0,\
  "deliveryAddress": "string",\
  "orderDetails": [\
    {\
      "orderDetailsId": 0,\
      "orderId": 0,\
      "productId": 0,\
      "quantity": 0,\
      "discount": 0\
    },\
    {\
      "orderDetailsId": 0,\
      "orderId": 0,\
      "productId": 0,\
      "quantity": 0,\
      "discount": 0\
    }\
  ]\
}

Remember the ',' used to seperated each list.

## GET Specific Order
Returns the details of a specific order, and all associated order details.  The order is specified by entering its orderID.

## PUT Existing Order
Edits an existing order, and also its associating orders by replacing the entire record with a new record.  The order is specified by entering their orderID.  The input is in the following format:\
{\
  "orderId": 0,\
  "orderDate": "2023-08-31",\
  "customerId": 0,\
  "deliveryAddress": "string",\
  "orderDetails": [\
    {\
      "orderDetailsId": 0,\
      "orderId": 0,\
      "productId": 0,\
      "quantity": 0,\
      "discount": 0\
    }\
  ]\
}

For orderDate, only the date component needs to be added - the time component may be omitted.\
All associated order details HAVE to be entered when using this action method, otherwise their values will become null.  Remember that additional lists can be nested for each order detail associated with the order.

## DELETE Existing Order
Deletes an existing order, and all its associated order details.  The order is specified by entering its orderID.

## PATCH Existing Order
Edits an existing order by only changing the desired values.  The order is specified by entering its orderID.  The input is in the following format:\
[\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  }\
]

For operationType, enter 2.\
For path, enter a '/', followed by the name of the field that you to edit, e.g., "/OrderDate".\
For op, enter "replace".\
Remove the from line.\
For value, enter the new desired value (be consistent with the field's data type).

If there is more than one value to change, additional lists can be nested as such:\

[\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  },\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  }\
]

Remember the ',' used to seperated each list.

## GET Orders Related to Customer
Returns a list of all the orders (as well as the associated order details) related a specified customer.  The customer is specified by entering their customerID.

## GET All Products
Returns a list of the details of all the products.

## POST New Product
Adds a new product.  The input is in the following format:\
{\
  "productId": 0,\
  "productName": "string",\
  "productDescription": "string",\
  "unitsInStock": 0\
}

## GET Specific Product
Returns the details of a specific product.  The product is specified by entering its productID.

## PUT Existing Product
Edits an existing product, by replacing the entire record with a new record.  The product is specified by entering its productID.  The input is in the following format:\
{\
  "productId": 0,\
  "productName": "string",\
  "productDescription": "string",\
  "unitsInStock": 0\
}

## DELETE Existing Product
Deletes an existing product.  The product is specified by entering its productID.

## PATCH Existing Product
Edits an existing product by only changing the desired values.  The product is specified by entering its productID.  The input is in the following format:\
[\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  }\
]

For operationType, enter 2.\
For path, enter a '/', followed by the name of the field that you to edit, e.g., "/ProductName".\
For op, enter "replace".\
Remove the from line.\
For value, enter the new desired value (be consistent with the field's data type).

If there is more than one value to change, additional lists can be nested as such:\

[\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  },\
  {\
    "operationType": 0,\
    "path": "string",\
    "op": "string",\
    "from": "string",\
    "value": "string"\
  }\
]

Remember the ',' used to seperated each list.

## GET Products Related to Order
Returns a list of all the products related a specified order.  The order is specified by entering its orderID.

## References
Jongalloway (no date) Create a web API with ASP.NET Core controllers - Training. Available at: https://learn.microsoft.com/en-us/training/modules/build-web-api-aspnet-core/. (Accessed: 15 August 2023).

معارف (no date) Learn Swagger ASP NET Core Course Online For Free With Certificate. Available at: https://www.mindluster.com/certificate/5807. (Accessed: 17 August 2023).

Wwlpublish (no date) Describe cloud computing - Training. Available at: https://learn.microsoft.com/en-us/training/modules/describe-cloud-compute/. (Accessed: 19 August 2023).

Wwlpublish (no date) Describe the benefits of using cloud services - Training. Available at: https://learn.microsoft.com/en-us/training/modules/describe-benefits-use-cloud-services/. (Accessed: 19 August 2023).

Wwlpublish (no date) Describe cloud service types - Training. Available at: https://learn.microsoft.com/en-us/training/modules/describe-cloud-service-types/. (Accessed: 19 August 2023).

RicoSuter (2022) ASP.NET Core web API documentation with Swagger / OpenAPI. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/web-api-help-pages-using-swagger?view=aspnetcore-3.1. (Accessed: 21 August 2023).

Sanjay (2021) “Entity Framework Core in ASP.NET Core 3.1 - Getting started | Pro Code Guide,” Pro Code Guide, 10 July. Available at: https://procodeguide.com/programming/entity-framework-core-in-asp-net-core/. (Accessed: 21 August 2023).

Bricelam (2023) EF Core tools reference (.NET CLI) - EF Core. Available at: https://learn.microsoft.com/en-us/ef/core/cli/dotnet. (Accessed: 23 August 2023).

Install SQL Server & SSMS (no date). Available at: https://www.tutorialsteacher.com/sqlserver/install-sql-server. (Accessed: 23 August 2023).

Stephen-Sumner (2023) Abbreviation examples for Azure resources - Cloud Adoption Framework. Available at: https://learn.microsoft.com/en-us/azure/cloud-adoption-framework/ready/azure-best-practices/resource-abbreviations. (Accessed: 23 August 2023).

Alexwolfmsft (2023) Connect to and query Azure SQL Database using .NET and Entity Framework Core - Azure SQL Database. Available at: https://learn.microsoft.com/en-us/azure/azure-sql/database/azure-sql-dotnet-entity-framework-core-quickstart?view=azuresql&tabs=visual-studio%2Cservice-connector%2Cportal. (Accessed: 23 August 2023).

Rolyon (2023) Tutorial: Grant a user access to Azure resources using the Azure portal - Azure RBAC. Available at: https://learn.microsoft.com/en-us/azure/role-based-access-control/quickstart-assign-role-user-portal. (Accessed: 24 August 2023).

Muller, V.A.P. by J. (2022) Join two entities in .NET Core, using lambda and Entity Framework Core - JD Bots. Available at: https://jd-bots.com/2022/01/24/join-two-entities-in-net-core-using-lambda-and-entity-framework-core/. (Accessed: 24 August 2023).

Codemillmatt (2022) Publish an ASP.NET Core web API to Azure API Management with Visual Studio. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-api-management-using-vs?view=aspnetcore-6.0. (Accessed: 24 August 2023).

Rick-Anderson (2022) Articles based on ASP.NET Core projects created with individual user accounts. Available at: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/individual?view=aspnetcore-7.0. (Accessed: 25 August 2023).

Cilwerner (2023) Application types for the Microsoft identity platform - Microsoft Entra. Available at: https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-app-types. (Accessed: 25 August 2023).

WilliamDAssafMSFT (2023) What is a server in Azure SQL Database and Azure Synapse Analytics? - Azure SQL Database. Available at: https://learn.microsoft.com/en-us/azure/azure-sql/database/logical-servers?view=azuresql&tabs=portal. (Accessed: 25 August 2023).

AngelosP (2023) Add a connection to Azure SQL Database - Visual Studio (Windows). Available at: https://learn.microsoft.com/en-us/visualstudio/azure/azure-sql-database-add-connected-service?view=vs-2022&source=recommendations. (Accessed: 26 August 2023).

Rick-Anderson (2023) Dependency injection in ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-7.0. (Accessed: 26 August 2023).

Tdykstra (2023) Safe storage of app secrets in development in ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows. (Accessed: 26 August 2023).

Dotnet-Bot (no date) ControllerBase Class (Microsoft.AspNetCore.MVC). Available at: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase?view=aspnetcore-7.0. (Accessed: 27 August 2023).

Dotnet-Bot (no date) DBContext Class (System.Data.Entity). Available at: https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.dbcontext?view=entity-framework-6.2.0. (Accessed: 27 August 2023).

Zuckerthoben (2023) Get started with Swashbuckle and ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio. (Accessed: 27 August 2023).

Rick-Anderson (2023b) JsonPatch in ASP.NET Core web API. Available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-3.1. (Accessed: 28 August 2023).

auth0.com (no date) JWT.IO - JSON Web Tokens Introduction. Available at: https://jwt.io/introduction/. (Accessed: 28 August 2023).

Dotnet-Bot (no date c) Microsoft.AspNetCore.Identity.EntityFrameworkCore Namespace. Available at: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.identity.entityframeworkcore?view=aspnetcore-1.1. (Accessed: 28 August 2023).

IAmTimCorey (2022) Intro to Web API in .NET 6 - Including Minimal APIs, Swagger, and more. Available at: https://www.youtube.com/watch?v=87oOF9Ve-KA. (Accessed: 28 August 2023).

Rick-Anderson (2022b) Using Web API 2 with Entity Framework 6. Available at: https://learn.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-1. (Accessed: 29 August 2023).

Wadepickett (2023) Tutorial: Create a web API with ASP.NET Core. Available at: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio#over-post. (Accessed: 29 August 2023).

AndriySvyryd (2023) Advanced table mapping - EF Core. Available at: https://learn.microsoft.com/en-us/ef/core/modeling/table-splitting. (Accessed: 29 August 2023).

Entity Framework 6 (no date). Available at: https://www.entityframeworktutorial.net/entityframework6/introduction.aspx. (Accessed: 29 August 2023).
