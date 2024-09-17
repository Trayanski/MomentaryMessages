I created a new ASP.NET Core Web Application named "MomentaryMessages".
I build the project with "local iis".
You can log in using admin@admin.net/Test123$ for an admin account or user@user.com/Test123$ for a non-admin account.
The main functionality is isolated in the two buttons on the home page:
	- "Generate a secret link" if Admin;
	- "Tell me a secret!" for a normal user or admin;
The endpoint is: .../api/supersecretapi?userName=XXX
The userName is validated on the admin creation page via javascript (.../SuperSecret/GenerateLink) for the user side validation
	and in the SecretViewLogsService/AddAsync method for the server side validation;
	
The expire links mechanism is configured in the create link admin page under "Assign expiry date (not required)".
The link can be clicked X number of times mechanism is configured in the create link admin page under "Number of views (not required)".
If one of these bonus options is selected, ajax call is made from the admin page
	and a "contract" entry is stored in the database that stores the "ExpiryDate" and/or "RemainingViewsCount".
Only admins can view links with these bonus mechanisms.
