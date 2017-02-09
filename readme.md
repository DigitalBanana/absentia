# Absentia

![CI Status](https://thecodefaceuk.visualstudio.com/_apis/public/build/definitions/fcc09e7f-63e2-4376-86b6-c48f16836d51/1/badge )

## Overview
An example of using the Microsoft Graph Subscription and Notification functionality to automatically update Office 365 calendars.

This application has been written to augment the behaviour of a third party absence booking system in order to be able to record different 
leave types while making this clearly visible on employees calendars.  The applicatons uses an entity framework code first database to manage 
users, subscriptionsd and notification data. This will be created if it does not exist based on the value of the AbsentiaDbContext 
Connection string.

### Useful Links:

[Microsoft Graph Notifications example](https://github.com/microsoftgraph/aspnet-webhooks-rest-sample)

[Entity Framwork Code First Migrations](https://msdn.microsoft.com/en-us/library/jj591621(v=vs.113).aspx)



## Getting Started
### Pre-requisites
* VisualStudio 2015 or greater
* Clone this repository
* Download [ngrok]( https://ngrok.com/)
* Create an AAD Application - note Application Id and Application Key - these will be required later
* Create (or purchase) an SSL certificate



## Source

```
-Root
--Source
---Absentia.Console - application that runs in Azure WebJob, manages subscriptions and processes notifications
---Absentia.Domain
---Absentia.Domain.Test
---Absentia.Ioc
---Absentia.Model
---Absentia.Web - web application that receives event notifications from Microsoft Graph
```

## Required Configuration

Application Settings  - set in Absentia.Console
    
    <add key="TenantId" value="YOURTENANT.onmicrosoft.com" />
    <add key="AadTokenEndpoint" value="https://login.microsoftonline.com/{0}" />
    <add key="MicrosoftGraphResource" value="https://graph.microsoft.com" />
    <add key="OutlookResource" value="https://outlook.office.com" />
    <add key="AbsentiaAadApplicationId" value="YOUR_AAD_APPID" />
    <add key="AbsentiaAadApplicationKey" value="YOUR_AAD_APPKEY" />
    <add key="AbsentiaNotificationUrl" value="YOUR_AZURE_API_APP_URL" />
    <add key="CertificateThumbprint" value="YOUR_CERTIFICATE_THUMBPRINT" />

Connection Strings - set in Absentia.Console and Absentia.Web

    <add name="AbsentiaDbContext" connectionString="YOUR_CONNECTION_STRING" providerName="System.Data.SqlClient" />


## Running application locally
In order to set up a Microsoft Graph subscription you will need a valid URL to receive a validation request and the subsequent notification 
payloads. During development [ngrok]( https://ngrok.com/) allows you to receive events to a public url and route these through to your local 
development server. This is easy to setup and is outlined below:

1. Note the randomly allocated portal that VisualStudio has assigned to Absentia.Web (this example assumes port 31416)
2. Navigate to wherever you downloaded ngrok to and  run the following command

    `ngrok http 31416 -host-header=localhost:31416`

3. Note the *Forwarding*  value for https - and enter this as the value for the AbsentiaNotificationUrl setting above
4. Set Absentia.Web and Absentia.Console as startup projects for the solution in Visual Studio 
5. To run locally you will need the .pfx of a certificate that has access to the Azure Active Directory tenant you wish to use
6. Add a user any users in your Azure Active Directory that you wish to add subscriptions for
7. Launch the application in Visual Studio 



