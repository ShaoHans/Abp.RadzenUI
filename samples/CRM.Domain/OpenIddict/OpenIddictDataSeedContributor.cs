﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using OpenIddict.Abstractions;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace CRM.OpenIddict;

/* Creates initial data that is needed to property run the application
 * and make client-to-server communication possible.
 */
public class OpenIddictDataSeedContributor(
    IConfiguration configuration,
    IOpenIddictApplicationRepository openIddictApplicationRepository,
    IAbpApplicationManager applicationManager,
    IOpenIddictScopeRepository openIddictScopeRepository,
    IOpenIddictScopeManager scopeManager,
    IPermissionDataSeeder permissionDataSeeder,
    IStringLocalizer<OpenIddictResponse> l
) : IDataSeedContributor, ITransientDependency
{
    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        await CreateScopesAsync();
        await CreateApplicationsAsync();
    }

    private async Task CreateScopesAsync()
    {
        if (await openIddictScopeRepository.FindByNameAsync("CRM") == null)
        {
            await scopeManager.CreateAsync(
                new OpenIddictScopeDescriptor
                {
                    Name = "CRM",
                    DisplayName = "CRM API",
                    Resources = { "CRM" }
                }
            );
        }
    }

    private async Task CreateApplicationsAsync()
    {
        var commonScopes = new List<string>
        {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles,
            "CRM"
        };

        var configurationSection = configuration.GetSection("OpenIddict:Applications");

        //Console Test / Angular Client
        var consoleAndAngularClientId = configurationSection["CRM_App:ClientId"];
        if (!consoleAndAngularClientId.IsNullOrWhiteSpace())
        {
            var consoleAndAngularClientRootUrl = configurationSection["CRM_App:RootUrl"]
                ?.TrimEnd('/');
            await CreateApplicationAsync(
                name: consoleAndAngularClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Console Test / Angular Application",
                secret: null,
                grantTypes:
                [
                    OpenIddictConstants.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.GrantTypes.Password,
                    OpenIddictConstants.GrantTypes.ClientCredentials,
                    OpenIddictConstants.GrantTypes.RefreshToken,
                    "LinkLogin",
                    "Impersonation"
                ],
                scopes: commonScopes,
                redirectUri: consoleAndAngularClientRootUrl,
                postLogoutRedirectUri: consoleAndAngularClientRootUrl,
                clientUri: consoleAndAngularClientRootUrl,
                logoUri: "/images/clients/angular.svg"
            );
        }

        // Swagger Client
        var swaggerClientId = configurationSection["CRM_Swagger:ClientId"];
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var swaggerRootUrl = configurationSection["CRM_Swagger:RootUrl"]?.TrimEnd('/');

            await CreateApplicationAsync(
                name: swaggerClientId!,
                type: OpenIddictConstants.ClientTypes.Public,
                consentType: OpenIddictConstants.ConsentTypes.Implicit,
                displayName: "Swagger Application",
                secret: null,
                grantTypes: [OpenIddictConstants.GrantTypes.AuthorizationCode,],
                scopes: commonScopes,
                redirectUri: $"{swaggerRootUrl}/swagger/oauth2-redirect.html",
                clientUri: swaggerRootUrl,
                logoUri: "/images/clients/swagger.svg"
            );
        }
    }

    private async Task CreateApplicationAsync(
        [NotNull] string name,
        [NotNull] string type,
        [NotNull] string consentType,
        string displayName,
        string? secret,
        List<string> grantTypes,
        List<string> scopes,
        string? redirectUri = null,
        string? postLogoutRedirectUri = null,
        List<string>? permissions = null,
        string? clientUri = null,
        string? logoUri = null
    )
    {
        if (
            !string.IsNullOrEmpty(secret)
            && string.Equals(
                type,
                OpenIddictConstants.ClientTypes.Public,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BusinessException(l["NoClientSecretCanBeSetForPublicApplications"]);
        }

        if (
            string.IsNullOrEmpty(secret)
            && string.Equals(
                type,
                OpenIddictConstants.ClientTypes.Confidential,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            throw new BusinessException(l["TheClientSecretIsRequiredForConfidentialApplications"]);
        }

        var client = await openIddictApplicationRepository.FindByClientIdAsync(name);

        var application = new AbpApplicationDescriptor
        {
            ClientId = name,
            ClientType = type,
            ClientSecret = secret,
            ConsentType = consentType,
            DisplayName = displayName,
            ClientUri = clientUri,
            LogoUri = logoUri,
        };

        Check.NotNullOrEmpty(grantTypes, nameof(grantTypes));
        Check.NotNullOrEmpty(scopes, nameof(scopes));

        if (
            new[]
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode,
                OpenIddictConstants.GrantTypes.Implicit
            }.All(grantTypes.Contains)
        )
        {
            application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);

            if (
                string.Equals(
                    type,
                    OpenIddictConstants.ClientTypes.Public,
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken
                );
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.ResponseTypes.CodeToken
                );
            }
        }

        if (!redirectUri.IsNullOrWhiteSpace() || !postLogoutRedirectUri.IsNullOrWhiteSpace())
        {
            application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.EndSession);
        }

        var buildInGrantTypes = new[]
        {
            OpenIddictConstants.GrantTypes.Implicit,
            OpenIddictConstants.GrantTypes.Password,
            OpenIddictConstants.GrantTypes.AuthorizationCode,
            OpenIddictConstants.GrantTypes.ClientCredentials,
            OpenIddictConstants.GrantTypes.DeviceCode,
            OpenIddictConstants.GrantTypes.RefreshToken
        };

        foreach (var grantType in grantTypes)
        {
            if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode)
            {
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode
                );
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
            }

            if (
                grantType == OpenIddictConstants.GrantTypes.AuthorizationCode
                || grantType == OpenIddictConstants.GrantTypes.Implicit
            )
            {
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.Endpoints.Authorization
                );
            }

            if (
                grantType == OpenIddictConstants.GrantTypes.AuthorizationCode
                || grantType == OpenIddictConstants.GrantTypes.ClientCredentials
                || grantType == OpenIddictConstants.GrantTypes.Password
                || grantType == OpenIddictConstants.GrantTypes.RefreshToken
                || grantType == OpenIddictConstants.GrantTypes.DeviceCode
            )
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.Endpoints.Introspection
                );
            }

            if (grantType == OpenIddictConstants.GrantTypes.ClientCredentials)
            {
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
                );
            }

            if (grantType == OpenIddictConstants.GrantTypes.Implicit)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
            }

            if (grantType == OpenIddictConstants.GrantTypes.Password)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
            }

            if (grantType == OpenIddictConstants.GrantTypes.RefreshToken)
            {
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken
                );
            }

            if (grantType == OpenIddictConstants.GrantTypes.DeviceCode)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.Endpoints.DeviceAuthorization
                );
            }

            if (grantType == OpenIddictConstants.GrantTypes.Implicit)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
                if (
                    string.Equals(
                        type,
                        OpenIddictConstants.ClientTypes.Public,
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken
                    );
                    application.Permissions.Add(
                        OpenIddictConstants.Permissions.ResponseTypes.Token
                    );
                }
            }

            if (!buildInGrantTypes.Contains(grantType))
            {
                application.Permissions.Add(
                    OpenIddictConstants.Permissions.Prefixes.GrantType + grantType
                );
            }
        }

        var buildInScopes = new[]
        {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };

        foreach (var scope in scopes)
        {
            if (buildInScopes.Contains(scope))
            {
                application.Permissions.Add(scope);
            }
            else
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
            }
        }

        if (redirectUri != null)
        {
            if (!redirectUri.IsNullOrEmpty())
            {
                if (
                    !Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri)
                    || !uri.IsWellFormedOriginalString()
                )
                {
                    throw new BusinessException(l["InvalidRedirectUri", redirectUri]);
                }

                if (application.RedirectUris.All(x => x != uri))
                {
                    application.RedirectUris.Add(uri);
                }
            }
        }

        if (postLogoutRedirectUri != null)
        {
            if (!postLogoutRedirectUri.IsNullOrEmpty())
            {
                if (
                    !Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri)
                    || !uri.IsWellFormedOriginalString()
                )
                {
                    throw new BusinessException(
                        l["InvalidPostLogoutRedirectUri", postLogoutRedirectUri]
                    );
                }

                if (application.PostLogoutRedirectUris.All(x => x != uri))
                {
                    application.PostLogoutRedirectUris.Add(uri);
                }
            }
        }

        if (permissions != null)
        {
            await permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                name,
                permissions,
                null
            );
        }

        if (client == null)
        {
            await applicationManager.CreateAsync(application);
            return;
        }

        if (!HasSameRedirectUris(client, application))
        {
            client.RedirectUris = JsonSerializer.Serialize(
                application.RedirectUris.Select(q => q.ToString().RemovePostFix("/"))
            );
            client.PostLogoutRedirectUris = JsonSerializer.Serialize(
                application.PostLogoutRedirectUris.Select(q => q.ToString().RemovePostFix("/"))
            );

            await applicationManager.UpdateAsync(client.ToModel());
        }

        if (!HasSameScopes(client, application))
        {
            client.Permissions = JsonSerializer.Serialize(
                application.Permissions.Select(q => q.ToString())
            );
            await applicationManager.UpdateAsync(client.ToModel());
        }
    }

    private static bool HasSameRedirectUris(
        OpenIddictApplication existingClient,
        AbpApplicationDescriptor application
    )
    {
        return existingClient.RedirectUris
            == JsonSerializer.Serialize(
                application.RedirectUris.Select(q => q.ToString().RemovePostFix("/"))
            );
    }

    private static bool HasSameScopes(
        OpenIddictApplication existingClient,
        AbpApplicationDescriptor application
    )
    {
        return existingClient.Permissions
            == JsonSerializer.Serialize(
                application.Permissions.Select(q => q.ToString().TrimEnd('/'))
            );
    }
}
