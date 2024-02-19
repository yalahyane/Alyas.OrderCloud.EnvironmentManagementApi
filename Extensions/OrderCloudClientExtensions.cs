using Alyas.OrderCloud.EnvironmentManagementApi.Models;
using OrderCloud.SDK;
using Serilog;

namespace Alyas.OrderCloud.EnvironmentManagementApi.Extensions
{
    public static class IOrderCloudClientExtensions
    {
        public static async Task CloneEnvironment(this IOrderCloudClient sourceClient,
            IOrderCloudClient destinationClient)
        {
            await CloneAdminUserGroups(sourceClient, destinationClient);
            await CloneAdminUsers(sourceClient, destinationClient);
            await CloneAdminAddresses(sourceClient, destinationClient);
            await CloneBuyers(sourceClient, destinationClient);
            await CloneSecurityProfiles(sourceClient, destinationClient);
            await CloneLocales(sourceClient, destinationClient);
            await CloneIntegrationEvents(sourceClient, destinationClient);
            var apiClientMappings = await CloneApiClients(sourceClient, destinationClient);
            await CloneImpersonationConfigs(sourceClient, destinationClient, apiClientMappings);
            await CloneWebhooks(sourceClient, destinationClient, apiClientMappings);
            await CloneParentProducts(sourceClient, destinationClient);
            await CloneProducts(sourceClient, destinationClient);
            await CloneCatalogs(sourceClient, destinationClient);
            await ClonePriceSchedules(sourceClient, destinationClient);
            await CloneSpecs(sourceClient, destinationClient);
            await CloneProductsFacets(sourceClient, destinationClient);
            await ClonePromotions(sourceClient, destinationClient);
            await CloneAssignments(sourceClient, destinationClient);
        }

        public static async Task CleanupEnvironment(this IOrderCloudClient client, EnvCleanupModel model)
        {
            await DeletePromotions(client);
            await DeleteProductsFacets(client);
            await DeleteSpecs(client);
            await DeletePriceSchedules(client);
            await DeleteWebhooks(client);
            await DeleteImpersonationConfigs(client);
            await DeleteApiClients(client, model.ClientId);
            await DeleteIntegrationEvents(client);
            await DeleteLocales(client);
            await DeleteSecurityProfiles(client, model.AdminSecurityProfileId);
            await DeleteBuyers(client);
            await DeleteCatalogs(client);
            await DeleteProducts(client);
            await DeleteParentProducts(client);
            await DeleteAdminAddresses(client);
            await DeleteAdminUsers(client, model.AdminUserId);
            await DeleteAdminUserGroups(client);
        }

        public static async Task CloneCatalog(this IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            await CloneParentProducts(sourceClient, destinationClient);
            await CloneProducts(sourceClient, destinationClient);
            await CloneCatalogs(sourceClient, destinationClient);
            await ClonePriceSchedules(sourceClient, destinationClient);
            await CloneSpecs(sourceClient, destinationClient);
            await CloneProductsFacets(sourceClient, destinationClient);
            await ClonePromotions(sourceClient, destinationClient);
            await CloneCatalogsAssignments(sourceClient, destinationClient);
        }


        public static async Task CleanupCatalog(this IOrderCloudClient client)
        {
            await DeletePromotions(client);
            await DeleteProductsFacets(client);
            await DeleteSpecs(client);
            await DeletePriceSchedules(client);
            await DeleteCatalogs(client);
            await DeleteProducts(client);
            await DeleteParentProducts(client);
        }

        private static async Task CloneAdminUserGroups(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var adminUserGroups = await sourceClient.AdminUserGroups.ListAsync(page:pageNumber, pageSize:100);
            while (adminUserGroups.Items.Any())
            {
                pageNumber ++;
                foreach (var adminUserGroup in adminUserGroups.Items)
                {
                    try
                    {
                        await destinationClient.AdminUserGroups.CreateAsync(adminUserGroup);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Admin User Group: {adminUserGroup.ID}. Exception: {e}");
                    }
                }
                adminUserGroups = await sourceClient.AdminUserGroups.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteAdminUserGroups(IOrderCloudClient client)
        {
            var firstPass = true;
            var adminUserGroups = await client.AdminUserGroups.ListAsync(pageSize: 100);
            var originalTotal = adminUserGroups.Meta.TotalCount;

            while (adminUserGroups.Items.Any() && (firstPass || originalTotal > adminUserGroups.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = adminUserGroups.Meta.TotalCount;
                foreach (var adminUserGroup in adminUserGroups.Items)
                {
                    try
                    {
                        await client.AdminUserGroups.DeleteAsync(adminUserGroup.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Admin User Group: {adminUserGroup.ID}. Exception: {e}");
                    }
                }
                adminUserGroups = await client.AdminUserGroups.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneAdminUsers(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var adminUsers = await sourceClient.AdminUsers.ListAsync(page: pageNumber, pageSize: 100);
            while (adminUsers.Items.Any())
            {
                pageNumber++;
                foreach (var adminUser in adminUsers.Items)
                {
                    adminUser.CompanyID = null;
                    try
                    {
                        await destinationClient.AdminUsers.CreateAsync(adminUser);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Admin User: {adminUser.ID}. Exception: {e}");
                    }
                }
                adminUsers = await sourceClient.AdminUsers.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteAdminUsers(IOrderCloudClient client, string adminUserId)
        {
            var firstPass = true;
            var adminUsers = await client.AdminUsers.ListAsync(pageSize: 100);
            var originalTotal = adminUsers.Meta.TotalCount;

            while (adminUsers.Items.Any() && (firstPass || originalTotal > adminUsers.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = adminUsers.Meta.TotalCount;
                foreach (var adminUser in adminUsers.Items.Where(x=>!x.ID.Equals(adminUserId, StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        await client.AdminUsers.DeleteAsync(adminUser.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Admin User: {adminUser.ID}. Exception: {e}");
                    }
                }
                adminUsers = await client.AdminUsers.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneAdminAddresses(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var adminAddresses = await sourceClient.AdminAddresses.ListAsync(page: pageNumber, pageSize: 100);
            while (adminAddresses.Items.Any())
            {
                pageNumber++;
                foreach (var adminAddress in adminAddresses.Items)
                {
                    try
                    {
                        await destinationClient.AdminAddresses.CreateAsync(adminAddress);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Admin Address: {adminAddress.ID}. Exception: {e}");
                    }
                }
                adminAddresses = await sourceClient.AdminAddresses.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteAdminAddresses(IOrderCloudClient client)
        {
            var firstPass = true;
            var adminAddresses = await client.AdminAddresses.ListAsync(pageSize: 100);
            var originalTotal = adminAddresses.Meta.TotalCount;

            while (adminAddresses.Items.Any() && (firstPass || originalTotal > adminAddresses.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = adminAddresses.Meta.TotalCount;

                foreach (var adminAddress in adminAddresses.Items)
                {
                    try
                    {
                        await client.AdminAddresses.DeleteAsync(adminAddress.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Admin Address: {adminAddress.ID}. Exception: {e}");
                    }
                }
                adminAddresses = await client.AdminAddresses.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneCatalogs(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var catalogs = await sourceClient.Catalogs.ListAsync(page: pageNumber, pageSize:100);
            while (catalogs.Items.Any())
            {
                pageNumber++;
                foreach (var catalog in catalogs.Items)
                {
                    catalog.CategoryCount = 0;
                    catalog.OwnerID = null;
                    try
                    {
                        await destinationClient.Catalogs.CreateAsync(catalog);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Catalog: {catalog.ID}. Exception: {e}");
                    }

                    await CloneCategories(catalog.ID, sourceClient, destinationClient);
                    await CloneCategoryAssignments(catalog.ID, sourceClient, destinationClient);
                    await CloneCategoryProductAssignments(catalog.ID, sourceClient, destinationClient);
                }
                catalogs = await sourceClient.Catalogs.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteCatalogs(IOrderCloudClient client)
        {
            var firstPass = true;
            var catalogs = await client.Catalogs.ListAsync(pageSize: 100);
            var originalTotal = catalogs.Meta.TotalCount;

            while (catalogs.Items.Any() && (firstPass || originalTotal > catalogs.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = catalogs.Meta.TotalCount;
                foreach (var catalog in catalogs.Items)
                {
                    try
                    {
                        await client.Catalogs.DeleteAsync(catalog.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Catalog: {catalog.ID}. Exception: {e}");
                    }
                }
                catalogs = await client.Catalogs.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneCategories(string catalogId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var categories = await sourceClient.Categories.ListAsync(catalogId, page: pageNumber, pageSize: 100, depth:"5");
            while (categories.Items.Any())
            {
                pageNumber++;
                foreach (var category in categories.Items)
                {
                    try
                    {
                        await destinationClient.Categories.CreateAsync(catalogId, category);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Category: {category.ID}. Exception: {e}");
                    }
                }
                categories = await sourceClient.Categories.ListAsync(catalogId, page: pageNumber, pageSize: 100, depth: "5");
            }
        }

        private static async Task CloneCategoryAssignments(string catalogId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Categories.ListAssignmentsAsync(catalogId, page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.Categories.SaveAssignmentAsync(catalogId, assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Assignment for category: {assignment.CategoryID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Categories.ListAssignmentsAsync(catalogId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneCategoryProductAssignments(string catalogId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Categories.ListProductAssignmentsAsync(catalogId, page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.Categories.SaveProductAssignmentAsync(catalogId, assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Assignment for category and product: {assignment.CategoryID}|{assignment.ProductID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Categories.ListProductAssignmentsAsync(catalogId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneParentProducts(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var products = await sourceClient.Products.ListAsync(page: pageNumber, pageSize: 100, filters: new {IsParent = true});
            while (products.Items.Any())
            {
                pageNumber++;
                foreach (var product in products.Items)
                {
                    product.OwnerID = null;
                    try
                    {
                        await destinationClient.Products.CreateAsync(product);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create parent product: {product.ID}. Exception: {e}");
                    }
                    await CloneInventoryRecords(product.ID, sourceClient, destinationClient);
                    await CloneInventoryRecordsAssignments(product.ID, sourceClient, destinationClient);
                }
                products = await sourceClient.Products.ListAsync(page: pageNumber, pageSize: 100, filters: new { IsParent = true });
            }
        }

        private static async Task CloneProducts(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var products = await sourceClient.Products.ListAsync(page: pageNumber, pageSize: 100, filters: new { IsParent = false });
            while (products.Items.Any())
            {
                pageNumber++;
                foreach (var product in products.Items)
                {
                    product.OwnerID = null;
                    try
                    {
                        await destinationClient.Products.CreateAsync(product);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create product: {product.ID}. Exception: {e}");
                    }
                    await CloneInventoryRecords(product.ID, sourceClient, destinationClient);
                    await CloneInventoryRecordsAssignments(product.ID, sourceClient, destinationClient);
                }
                products = await sourceClient.Products.ListAsync(page: pageNumber, pageSize: 100, filters: new { IsParent = false });
            }
        }

        private static async Task DeleteParentProducts(IOrderCloudClient client)
        {
            var firstPass = true;
            var products = await client.Products.ListAsync(pageSize: 100, filters: new { IsParent = true });
            var originalTotal = products.Meta.TotalCount;

            while (products.Items.Any() && (firstPass || originalTotal > products.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = products.Meta.TotalCount;
                foreach (var product in products.Items)
                {
                    try
                    {
                        await client.Products.DeleteAsync(product.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete product: {product.ID}. Exception: {e}");
                    }
                }
                products = await client.Products.ListAsync(pageSize: 100, filters: new { IsParent = true });
            }
        }

        private static async Task DeleteProducts(IOrderCloudClient client)
        {
            var firstPass = true;
            var products = await client.Products.ListAsync(pageSize: 100, filters: new { IsParent = false });
            var originalTotal = products.Meta.TotalCount;

            while (products.Items.Any() && (firstPass || originalTotal > products.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = products.Meta.TotalCount;
                foreach (var product in products.Items)
                {
                    try
                    {
                        await client.Products.DeleteAsync(product.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete product: {product.ID}. Exception: {e}");
                    }
                }
                products = await client.Products.ListAsync(pageSize: 100, filters: new { IsParent = false });
            }
        }

        private static async Task CloneInventoryRecords(string productId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var productInventories = await sourceClient.InventoryRecords.ListAsync(productId, page: pageNumber, pageSize: 100);
            while (productInventories.Items.Any())
            {
                pageNumber++;
                foreach (var productInventory in productInventories.Items)
                {
                    productInventory.OwnerID = null;
                    try
                    {
                        await destinationClient.InventoryRecords.CreateAsync(productId, productInventory);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create product inventory: {productInventory.ID}. Exception: {e}");
                    }
                }
                productInventories = await sourceClient.InventoryRecords.ListAsync(productId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneInventoryRecordsAssignments(string productId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.InventoryRecords.ListAssignmentsAsync(productId, page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.InventoryRecords.SaveAssignmentAsync(productId, assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create assignment for inventory: {assignment.InventoryRecordID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.InventoryRecords.ListAssignmentsAsync(productId, page: pageNumber, pageSize: 100);
            }
        }
        private static async Task ClonePriceSchedules(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var priceSchedules = await sourceClient.PriceSchedules.ListAsync(page: pageNumber, pageSize: 100);
            while (priceSchedules.Items.Any())
            {
                pageNumber++;
                foreach (var priceSchedule in priceSchedules.Items)
                {
                    try
                    {
                        priceSchedule.OwnerID = null;
                        await destinationClient.PriceSchedules.CreateAsync(priceSchedule);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Price Schedule : {priceSchedule.ID}. Exception: {e}");
                    }
                }
                priceSchedules = await sourceClient.PriceSchedules.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeletePriceSchedules(IOrderCloudClient client)
        {
            var firstPass = true;
            var priceSchedules = await client.PriceSchedules.ListAsync(pageSize: 100);
            var originalTotal = priceSchedules.Meta.TotalCount;

            while (priceSchedules.Items.Any() && (firstPass || originalTotal > priceSchedules.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = priceSchedules.Meta.TotalCount;
                foreach (var priceSchedule in priceSchedules.Items)
                {
                    try
                    {
                        await client.PriceSchedules.DeleteAsync(priceSchedule.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete PriceSchedule: {priceSchedule.ID}. Exception: {e}");
                    }
                }
                priceSchedules = await client.PriceSchedules.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneSpecs(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var specs = await sourceClient.Specs.ListAsync(page: pageNumber, pageSize: 100);
            while (specs.Items.Any())
            {
                pageNumber++;
                foreach (var spec in specs.Items)
                {
                    try
                    {
                        spec.OwnerID = null;
                        await destinationClient.Specs.CreateAsync(spec);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Spec : {spec.ID}. Exception: {e}");
                    }
                }
                specs = await sourceClient.Specs.ListAsync(page: pageNumber, pageSize: 100);
            }
        }
        
        private static async Task DeleteSpecs(IOrderCloudClient client)
        {
            var firstPass = true;
            var specs = await client.Specs.ListAsync(pageSize: 100);
            var originalTotal = specs.Meta.TotalCount;

            while (specs.Items.Any() && (firstPass || originalTotal > specs.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = specs.Meta.TotalCount;
                foreach (var spec in specs.Items)
                {
                    try
                    {
                        await client.Specs.DeleteAsync(spec.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Spec: {spec.ID}. Exception: {e}");
                    }
                }
                specs = await client.Specs.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneProductsFacets(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var productFacets = await sourceClient.ProductFacets.ListAsync(page: pageNumber, pageSize: 100);
            while (productFacets.Items.Any())
            {
                pageNumber++;
                foreach (var productFacet in productFacets.Items)
                {
                    try
                    {
                        await destinationClient.ProductFacets.CreateAsync(productFacet);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Product Facet : {productFacet.ID}. Exception: {e}");
                    }
                }
                productFacets = await sourceClient.ProductFacets.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteProductsFacets(IOrderCloudClient client)
        {
            var firstPass = true;
            var productFacets = await client.ProductFacets.ListAsync(pageSize: 100);
            var originalTotal = productFacets.Meta.TotalCount;

            while (productFacets.Items.Any() && (firstPass || originalTotal > productFacets.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = productFacets.Meta.TotalCount;
                foreach (var productFacet in productFacets.Items)
                {
                    try
                    {
                        await client.ProductFacets.DeleteAsync(productFacet.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Product Facet: {productFacet.ID}. Exception: {e}");
                    }
                }
                productFacets = await client.ProductFacets.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneBuyers(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var buyers = await sourceClient.Buyers.ListAsync(page:pageNumber, pageSize:100);
            while (buyers.Items.Any())
            {
                pageNumber++;
                foreach (var buyer in buyers.Items)
                {
                    if (!string.IsNullOrEmpty(buyer.DefaultCatalogID))
                    {
                        var defaultCatalog = await sourceClient.Catalogs.GetAsync(buyer.DefaultCatalogID);
                        defaultCatalog.CategoryCount = 0;
                        defaultCatalog.OwnerID = null;
                        try
                        {
                            await destinationClient.Catalogs.CreateAsync(defaultCatalog);
                        }
                        catch (Exception e)
                        {
                            Log.Logger.Error($"Failed to create catalog : {defaultCatalog.ID}. Exception: {e}");
                        }
                    }

                    try
                    {
                        await destinationClient.Buyers.CreateAsync(buyer);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create buyer : {buyer.ID}. Exception: {e}");
                    }

                    await CloneUserGroups(buyer.ID, sourceClient, destinationClient);
                    await CloneUsers(buyer.ID, sourceClient, destinationClient);
                    await CloneAddresses(buyer.ID, sourceClient, destinationClient);
                    await CloneCostCenters(buyer.ID, sourceClient, destinationClient);
                    await CloneCreditCards(buyer.ID, sourceClient, destinationClient);
                    await CloneSpendingAccounts(buyer.ID, sourceClient, destinationClient);
                    await CloneUserGroupAssignments(buyer.ID, sourceClient, destinationClient);
                    await CloneAddressAssignments(buyer.ID, sourceClient, destinationClient);
                    await CloneCostCenterAssignments(buyer.ID, sourceClient, destinationClient);
                    await CloneCreditCardAssignments(buyer.ID, sourceClient, destinationClient);
                    await CloneSpendingAccountAssignments(buyer.ID, sourceClient, destinationClient);
                }
                buyers = await sourceClient.Buyers.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteBuyers(IOrderCloudClient client)
        {
            var firstPass = true;
            var buyers = await client.Buyers.ListAsync(pageSize: 100);
            var originalTotal = buyers.Meta.TotalCount;

            while (buyers.Items.Any() && (firstPass || originalTotal > buyers.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = buyers.Meta.TotalCount;

                foreach (var buyer in buyers.Items)
                {
                    try
                    {
                        await client.Buyers.DeleteAsync(buyer.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete buyer : {buyer.ID}. Exception: {e}");
                    }
                }
                buyers = await client.Buyers.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneUserGroups(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var userGroups = await sourceClient.UserGroups.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            while (userGroups.Items.Any())
            {
                pageNumber++;
                foreach (var userGroup in userGroups.Items)
                {
                    try
                    {
                        await destinationClient.UserGroups.CreateAsync(buyerId, userGroup);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create User Group: {userGroup.ID}. Exception: {e}");
                    }
                }
                userGroups = await sourceClient.UserGroups.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneUsers(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var users = await sourceClient.Users.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            while (users.Items.Any())
            {
                pageNumber++;
                foreach (var user in users.Items)
                {
                    try
                    {
                        await destinationClient.Users.CreateAsync(buyerId, user);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create User: {user.ID}. Exception: {e}");
                    }
                }
                users = await sourceClient.Users.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneAddresses(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var addresses = await sourceClient.Addresses.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            while (addresses.Items.Any())
            {
                pageNumber++;
                foreach (var address in addresses.Items)
                {
                    try
                    {
                        await destinationClient.Addresses.CreateAsync(buyerId, address);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Address: {address.ID}. Exception: {e}");
                    }
                }
                addresses = await sourceClient.Addresses.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneCostCenters(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var costCenters = await sourceClient.CostCenters.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            while (costCenters.Items.Any())
            {
                pageNumber++;
                foreach (var costCenter in costCenters.Items)
                {
                    try
                    {
                        await destinationClient.CostCenters.CreateAsync(buyerId, costCenter);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Cost Center: {costCenter.ID}. Exception: {e}");
                    }
                }
                costCenters = await sourceClient.CostCenters.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneCreditCards(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var creditCards = await sourceClient.CreditCards.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            while (creditCards.Items.Any())
            {
                pageNumber++;
                foreach (var creditCard in creditCards.Items)
                {
                    try
                    {
                        await destinationClient.CreditCards.CreateAsync(buyerId, creditCard);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Credit Card: {creditCard.ID}. Exception: {e}");
                    }
                }
                creditCards = await sourceClient.CreditCards.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneSpendingAccounts(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var spendingAccounts = await sourceClient.SpendingAccounts.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            while (spendingAccounts.Items.Any())
            {
                pageNumber++;
                foreach (var spendingAccount in spendingAccounts.Items)
                {
                    try
                    {
                        await destinationClient.SpendingAccounts.CreateAsync(buyerId, spendingAccount);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Spending Account: {spendingAccount.ID}. Exception: {e}");
                    }
                }
                spendingAccounts = await sourceClient.SpendingAccounts.ListAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneUserGroupAssignments(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.UserGroups.ListUserAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.UserGroups.SaveUserAssignmentAsync(buyerId, assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create assignment for User Group: {assignment.UserGroupID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.UserGroups.ListUserAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneAddressAssignments(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Addresses.ListAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.Addresses.SaveAssignmentAsync(buyerId, assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create assignment for address: {assignment.AddressID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Addresses.ListAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneCostCenterAssignments(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.CostCenters.ListAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.CostCenters.SaveAssignmentAsync(buyerId, assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create assignment for Cost Center: {assignment.CostCenterID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.CostCenters.ListAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneCreditCardAssignments(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.CreditCards.ListAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.CreditCards.SaveAssignmentAsync(buyerId, assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create assignment for Credit Card: {assignment.CreditCardID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.CreditCards.ListAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneSpendingAccountAssignments(string buyerId, IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.SpendingAccounts.ListAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {

                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create assignment for Spending Accounts: {assignment.SpendingAccountID}. Exception: {e}");
                    }
                    await destinationClient.SpendingAccounts.SaveAssignmentAsync(buyerId, assignment);
                }
                assignments = await sourceClient.SpendingAccounts.ListAssignmentsAsync(buyerId, page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneSecurityProfiles(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var securityProfiles = await sourceClient.SecurityProfiles.ListAsync(page:pageNumber, pageSize:100);
            while (securityProfiles.Items.Any())
            {
                pageNumber++;
                foreach (var securityProfile in securityProfiles.Items)
                {
                    try
                    {
                        await destinationClient.SecurityProfiles.CreateAsync(securityProfile);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Security Profile: {securityProfile.ID}. Exception: {e}");
                    }
                }
                securityProfiles = await sourceClient.SecurityProfiles.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteSecurityProfiles(IOrderCloudClient client, string adminSecurityProfileId)
        {
            var firstPass = true;
            var securityProfiles = await client.SecurityProfiles.ListAsync(pageSize: 100);
            var originalTotal = securityProfiles.Meta.TotalCount;

            while (securityProfiles.Items.Any() && (firstPass || originalTotal > securityProfiles.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = securityProfiles.Meta.TotalCount;

                foreach (var securityProfile in securityProfiles.Items.Where(x=>!x.ID.Equals(adminSecurityProfileId, StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        await client.SecurityProfiles.DeleteAsync(securityProfile.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Security Profile: {securityProfile.ID}. Exception: {e}");
                    }
                }
                securityProfiles = await client.SecurityProfiles.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneLocales(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var locales = await sourceClient.Locales.ListAsync(page:pageNumber, pageSize:100);
            while (locales.Items.Any())
            {
                pageNumber++;
                foreach (var locale in locales.Items)
                {
                    locale.OwnerID = null;
                    try
                    {
                        locale.OwnerID = null;
                        await destinationClient.Locales.CreateAsync(locale);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Locale: {locale.ID}. Exception: {e}");
                    }
                }
                locales = await sourceClient.Locales.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteLocales(IOrderCloudClient client)
        {
            var firstPass = true;
            var locales = await client.Locales.ListAsync(pageSize: 100);
            var originalTotal = locales.Meta.TotalCount;

            while (locales.Items.Any() && (firstPass || originalTotal > locales.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = locales.Meta.TotalCount;
                foreach (var locale in locales.Items)
                {
                    try
                    {
                        await client.Locales.DeleteAsync(locale.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Locale: {locale.ID}. Exception: {e}");
                    }
                }
                locales = await client.Locales.ListAsync(pageSize: 100);
            }
        }

        private static async Task<Dictionary<string,string>> CloneApiClients(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var apiClientMappings = new Dictionary<string, string>();
            var pageNumber = 1;
            var apiClients = await sourceClient.ApiClients.ListAsync(page:pageNumber, pageSize:100);
            while (apiClients.Items.Any())
            {
                pageNumber++;
                foreach (var apiClient in apiClients.Items)
                {
                    try
                    {
                        var newApiClient = await destinationClient.ApiClients.CreateAsync(apiClient);
                        apiClientMappings.Add(apiClient.ID.ToUpper(), newApiClient.ID.ToUpper());
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Api Client: {apiClient.ID}. Exception: {e}");
                    }
                }
                apiClients = await sourceClient.ApiClients.ListAsync(page:pageNumber, pageSize: 100);
            }

            return apiClientMappings;
        }

        private static async Task DeleteApiClients(IOrderCloudClient client, string adminApiClientId)
        {
            var firstPass = true;
            var apiClients = await client.ApiClients.ListAsync(pageSize: 100);
            var originalTotal = apiClients.Meta.TotalCount;

            while (apiClients.Items.Any() && (firstPass || originalTotal > apiClients.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = apiClients.Meta.TotalCount;
                foreach (var apiClient in apiClients.Items.Where(x=>!x.ID.Equals(adminApiClientId, StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        await client.ApiClients.DeleteAsync(apiClient.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to Delete Api Client: {apiClient.ID}. Exception: {e}");
                    }
                }
                apiClients = await client.ApiClients.ListAsync(pageSize: 100);
            }
        }


        private static async Task CloneImpersonationConfigs(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient, Dictionary<string, string> apiClientMappings)
        {
            var pageNumber = 1;
            var impersonationConfigs = await sourceClient.ImpersonationConfigs.ListAsync(page: pageNumber, pageSize: 100);
            while (impersonationConfigs.Items.Any())
            {
                pageNumber++;
                foreach (var impersonationConfig in impersonationConfigs.Items)
                {
                    impersonationConfig.ClientID = apiClientMappings[impersonationConfig.ClientID.ToUpper()];
                    try
                    {
                        await destinationClient.ImpersonationConfigs.CreateAsync(impersonationConfig);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to Create Impersonation Config: {impersonationConfig.ID}. Exception: {e}");
                    }
                }
                impersonationConfigs = await sourceClient.ImpersonationConfigs.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteImpersonationConfigs(IOrderCloudClient client)
        {
            var firstPass = true;
            var impersonationConfigs = await client.ImpersonationConfigs.ListAsync(pageSize: 100);
            var originalTotal = impersonationConfigs.Meta.TotalCount;

            while (impersonationConfigs.Items.Any() && (firstPass || originalTotal > impersonationConfigs.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = impersonationConfigs.Meta.TotalCount;
                foreach (var impersonationConfig in impersonationConfigs.Items)
                {
                    try
                    {
                        await client.ImpersonationConfigs.DeleteAsync(impersonationConfig.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to Delete Impersonation Config: {impersonationConfig.ID}. Exception: {e}");
                    }
                }
                impersonationConfigs = await client.ImpersonationConfigs.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneIntegrationEvents(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var integrationEvents = await sourceClient.IntegrationEvents.ListAsync(page: pageNumber, pageSize: 100);
            while (integrationEvents.Items.Any())
            {
                pageNumber++;
                foreach (var integrationEvent in integrationEvents.Items)
                {
                    try
                    {
                        await destinationClient.IntegrationEvents.CreateAsync(integrationEvent);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to Create Integration Event Config: {integrationEvent.ID}. Exception: {e}");
                    }
                }
                integrationEvents = await sourceClient.IntegrationEvents.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteIntegrationEvents(IOrderCloudClient client)
        {
            var firstPass = true;
            var integrationEvents = await client.IntegrationEvents.ListAsync(pageSize: 100);
            var originalTotal = integrationEvents.Meta.TotalCount;
            while (integrationEvents.Items.Any() && (firstPass || originalTotal > integrationEvents.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = integrationEvents.Meta.TotalCount;
                foreach (var integrationEvent in integrationEvents.Items)
                {
                    try
                    {
                        await client.IntegrationEvents.DeleteAsync(integrationEvent.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to Delete Integration Event Config: {integrationEvent.ID}. Exception: {e}");
                    }
                }
                integrationEvents = await client.IntegrationEvents.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneWebhooks(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient, Dictionary<string, string> apiClientMappings)
        {
            var pageNumber = 1;
            var webhooks = await sourceClient.Webhooks.ListAsync(page: pageNumber, pageSize: 100);
            while (webhooks.Items.Any())
            {
                pageNumber++;
                foreach (var webhook in webhooks.Items)
                {
                    webhook.ApiClientIDs = webhook.ApiClientIDs.Select(x => apiClientMappings[x.ToUpper()]).ToList();
                    try
                    {
                        await destinationClient.Webhooks.CreateAsync(webhook);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to Create Webhook: {webhook.ID}. Exception: {e}");
                    }
                }
                webhooks = await sourceClient.Webhooks.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeleteWebhooks(IOrderCloudClient client)
        {
            var firstPass = true;
            var webhooks = await client.Webhooks.ListAsync(pageSize: 100);
            var originalTotal = webhooks.Meta.TotalCount;

            while (webhooks.Items.Any() && (firstPass || originalTotal > webhooks.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = webhooks.Meta.TotalCount;
                foreach (var webhook in webhooks.Items)
                {
                    try
                    {
                        await client.Webhooks.DeleteAsync(webhook.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Webhook: {webhook.ID}. Exception: {e}");
                    }
                }
                webhooks = await client.Webhooks.ListAsync(pageSize: 100);
            }
        }

        private static async Task ClonePromotions(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var promotions = await sourceClient.Promotions.ListAsync(page: pageNumber, pageSize: 100);
            while (promotions.Items.Any())
            {
                pageNumber++;
                foreach (var promotion in promotions.Items)
                {
                    try
                    {
                        promotion.OwnerID = null;
                        await destinationClient.Promotions.CreateAsync(promotion);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Promotion: {promotion.ID}. Exception: {e}");
                    }
                }
                promotions = await sourceClient.Promotions.ListAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task DeletePromotions(IOrderCloudClient client)
        {
            var firstPass = true;
            var promotions = await client.Promotions.ListAsync(pageSize: 100);
            var originalTotal = promotions.Meta.TotalCount;

            while (promotions.Items.Any() && (firstPass || originalTotal > promotions.Meta.TotalCount))
            {
                firstPass = false;
                originalTotal = promotions.Meta.TotalCount;
                foreach (var promotion in promotions.Items)
                {
                    try
                    {
                        await client.Promotions.DeleteAsync(promotion.ID);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to delete Promotion: {promotion.ID}. Exception: {e}");
                    }
                }
                promotions = await client.Promotions.ListAsync(pageSize: 100);
            }
        }

        private static async Task CloneAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            await CloneAdminUserGroupAssignments(sourceClient, destinationClient);
            await CloneLocaleAssignments(sourceClient, destinationClient);
            await CloneCatalogAssignments(sourceClient, destinationClient);
            await CloneCatalogProductAssignments(sourceClient, destinationClient);
            await CloneProductAssignments(sourceClient, destinationClient);
            await CloneSpecProductAssignments(sourceClient, destinationClient);
            await CloneSecurityProfileAssignments(sourceClient, destinationClient);
            await CloneApiClientAssignments(sourceClient, destinationClient);
            await ClonePromotionAssignments(sourceClient, destinationClient);
        }

        private static async Task CloneCatalogsAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            await CloneCatalogAssignments(sourceClient, destinationClient);
            await CloneCatalogProductAssignments(sourceClient, destinationClient);
            await CloneProductAssignments(sourceClient, destinationClient);
            await CloneSpecProductAssignments(sourceClient, destinationClient);
            await ClonePromotionAssignments(sourceClient, destinationClient);
        }

        private static async Task CloneAdminUserGroupAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.AdminUserGroups.ListUserAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.AdminUserGroups.SaveUserAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Api Client: {assignment.UserGroupID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.AdminUserGroups.ListUserAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneCatalogAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Catalogs.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.Catalogs.SaveAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Catalog Assignment: {assignment.CatalogID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Catalogs.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneCatalogProductAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Catalogs.ListProductAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.Catalogs.SaveProductAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Catalog Product Assignment: {assignment.CatalogID}|{assignment.ProductID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Catalogs.ListProductAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneProductAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Products.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        assignment.SellerID = null;
                        await destinationClient.Products.SaveAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Product Assignment: {assignment.ProductID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Products.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneSpecProductAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Specs.ListProductAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.Specs.SaveProductAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Spec Product Assignment: {assignment.ProductID}|{assignment.SpecID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Specs.ListProductAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneSecurityProfileAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.SecurityProfiles.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.SecurityProfiles.SaveAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Security Profile Assignment: {assignment.SecurityProfileID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.SecurityProfiles.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneLocaleAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Locales.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.Locales.SaveAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to create Locale Assignment: {assignment.LocaleID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Locales.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task CloneApiClientAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.ApiClients.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.ApiClients.SaveAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to Api Client Assignment: {assignment.ApiClientID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.ApiClients.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }

        private static async Task ClonePromotionAssignments(IOrderCloudClient sourceClient, IOrderCloudClient destinationClient)
        {
            var pageNumber = 1;
            var assignments = await sourceClient.Promotions.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            while (assignments.Items.Any())
            {
                pageNumber++;
                foreach (var assignment in assignments.Items)
                {
                    try
                    {
                        await destinationClient.Promotions.SaveAssignmentAsync(assignment);
                    }
                    catch (Exception e)
                    {
                        Log.Logger.Error($"Failed to Promotion Assignment: {assignment.PromotionID}. Exception: {e}");
                    }
                }
                assignments = await sourceClient.Promotions.ListAssignmentsAsync(page: pageNumber, pageSize: 100);
            }
        }
    }
}
