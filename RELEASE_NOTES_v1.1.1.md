# Release Notes - ABCSchool v1.1.1

**Release Date**: November 6, 2025  
**Git Tag**: `v1.1.1`  
**Repository**: [SistemaEscolarMultiTenanty](https://github.com/ronaldocestrela/SistemaEscolarMultiTenanty)

## Summary

This patch release adds OpenAPI support for the tenant header and implements JWT/token helpers and DTOs to support authentication flows.

## Added

- Swagger/OpenAPI annotations and attribute to document the tenant header and required request header for tenant-aware endpoints.
- `ClaimPrincipalExtensions` to simplify reading claims from the current user.
- `TokenService` implementing token creation and refresh scaffolding.
- `JwtSettings` configuration model used to map JWT options from configuration.
- Token DTOs: `TokenRequest`, `RefreshTokenRequest`, `TokenResponse`.
- Startup and DI updates to register token services and identity helpers.

## Changed

- Updated `appsettings.json` with JWT configuration placeholders (issuer, audience, key, token lifetime).
- Program/Startup updated to wire token services and OpenAPI/Swagger enhancements.

## Notes for Developers

- Ensure `JwtSettings` values are set in production (secrets/secure storage) before issuing tokens.
- The token service scaffolding should be extended to persist refresh tokens if needed.
- OpenAPI tenant header annotation documents the `X-Tenant-Id` (or configured header name) so clients know to include it.

---

**Full Changelog**: [CHANGELOG.md](CHANGELOG.md)
