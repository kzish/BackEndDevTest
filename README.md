# BackEndDevTest
Afrosoft BackEnd Dev Test


# Auth
1. This is the oAuth authenticating server
2. Run in iis-express with visual studio or deploy to IIS with asp.net core 2.2
3. Copy url where Auth is deployed and paste in "BackEndDeveloperAssesment/Globals.cs" line 11

# BackEndDeveloperAssesment
1. Main API use swagger interface to test the API
2. Run Application and navigate to /swagger to use Swagger interface
3. Begin by calling the "/BankingApi​/RequestToken" method to retrieve the oAuth Token
4. clientID "test_user" and clientSecret "12345"
5. paste oAuth token in header by clicking "Authorise 🔓" button
6. prefix the token with "bearer <token>" and paste into the Authorization prompt, you are now authenticated
7. use "BankAPI" methods to manage bank account
