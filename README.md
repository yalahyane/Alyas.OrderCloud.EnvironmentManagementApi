# Alyas OrderCloud Environment Management API
This API includes 2 Endpoints:
# 1-CloneEnvironment: 
Clones a Source Environment into a Destination Environment.  
Receives Api URL, Client ID and Secret API Keys with FullAccess privilege for both Source and Destination  
# 2-CleanupEnvironment: 
Clones a Source Environment into a Destination Environment.  
Receives Api URL, Client ID and Secret API Keys with FullAccess privilege for Target Environment for Cleanup, As well as Admin User Id attached to the Admin API Client.  
The Admin User Id needs to be ignored during the cleanup for the environment to remain available for recloning.  

# Cloning and Cleanup include the following Entities and all their Assignments:  
-Security Profiles, Impersonation Configs.  
-Admin Users, Admin User Groups, Admin Addresses, API Clients, Integration Events, Locales, Webhooks. 
-Buyers, Users, User Groups, Addresses, Cost Centers, Credit Cards, Spending Accounts.
-Catalogs, Categories, Products, Price Schedules, Specs, Product Facets, Inventory Records.

If you need to clone any entities not listed above, feel free to Clone the Repo and Add them.  


# How to Use

# Option 1:  
Use this public Azure instance:  https://alyas-ordercloud-environmentmanagementapi.azurewebsites.net/swagger.
This runs on a Free Tier at the moment, so it might be slower than running it locally.

# Option 2:
Clone the repo and run the API locally from Visual Studio.  

**P.S:** Depending on the size of your marketplace, the Cloning can take some time. So keep it running and monitor that the environment is being populated from the OrderCloud Portal.
