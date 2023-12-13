A Customer Management System to keep track of customers of a certain merchant. 
The customers can be viewed online using Angular, the data is stored in a MSSQL database, and the two are connected via a API written in C#.

This system has three main layers: the UI, the API and the DB:
* The UI: written in Angular, consumes the .NET Web API.
* API: Written in .NET's C#, it makes the connection with the MSSQL DB, where the merchant's and customers informations are being stored. The API calls can be tested with Postman.
* DB: I'm using a Microsoft SQL (MSSQL) database, which on Mac runs via a docker container (from a Azure SQL Edge image).

General Flow of the Customer Management System:

![General_Flow](https://github.com/FrunzaDan/Customer_Management_System/blob/master/Documentation/Diagrams/CMS_General_Flow.PNG)

The System is secured by a JWT, which works the following way:

![Secutiry_JWT](https://github.com/FrunzaDan/Customer_Management_System/blob/master/Documentation/Diagrams/CMS_Security_JWT.PNG)

As an example, let us take the login process. This works as follows:

![Login_Process](https://github.com/FrunzaDan/Customer_Management_System/blob/master/Documentation/Diagrams/CMS_Login_Process.PNG)
