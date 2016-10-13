# DemoXamarinAzure

Ejemplo de como utilizar Azure Notification Service e integrarlo con aplicaciones en Xamarin.

Para probar la aplicación se recomienda utilizar Advanced REST para chomr.
Hacer un post a : http://customeroffers.azurewebsites.net/tables/Offer/


Agregar los headers
Content-Type : application/json
ZUMO-API-VERSION : 2.0.0


y el body en un formato como el siguiente

{
  "OfferText": "Test #54564642",
  "OfferStartDate" : "2016-09-15T16:45:20",
   "OfferEndDate" : "2016-09-15T16:45:20"
}


Reemplazar OfferText con lo que se desea agregar

