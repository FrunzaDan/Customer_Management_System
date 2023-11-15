A Customer Management System to keep track of customers of a certain merchant. 
The customers can be viewed online using Angular, the data is stored in a MSSQL database, and the two are connected via a API written in C#.

This system has three main layers: the UI, the API and the DB:
* The UI: written in Angular, consumes the .NET Web API.
* API: Written in .NET's C#, it makes the connection with the MSSQL DB, where the merchant's and customers informations are being stored. The API calls can be tested with Postman.
* DB: I'm using a Microsoft SQL (MSSQL) database, which on Mac runs via a docker container (from a Azure SQL Edge image).
