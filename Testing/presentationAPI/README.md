Phase F — Presentation API / Authorization Integration Tests

Summary
-------
This test host and its tests verify the production authorization pipeline (policy provider + authorization handler) while using a lightweight, deterministic test environment.

Key outcomes
- Production authorization pipeline kept intact: PermissionPolicyProvider, PermissionAuthorizationHandler, PermissionRequirement, and CustomClaimTypes.Permission are the real implementations used by the tests.
- Tests use an in-memory shared SQLite database for fast relational tests; production SQL Server migrations were not changed.
- No Docker or Testcontainers were added.

Test run summary (final validated run)
- Domain: total 47, passed 47, failed 0
- Application: total 23, passed 23, failed 0
- Infrastructure: total 5, passed 5, failed 0
- Presentation API: total 5, passed 5, failed 0
- Full solution: total 80, passed 80, failed 0

Explicit, literal evidence captured during test runs
(these lines were produced by the test host console and audit logs):

- Permission policy provider executed at policy resolution:
  - [TestDiag] PermissionPolicyProvider.GetPolicyAsync building policy for: Permission:ReadDoctor
  - [TestDiag] TestPolicyInitializer resolved policy: OK; Requirements:2

- Authorization handler executed and evaluated the claim type/value:
  - [TestDiag] PermissionAuthorizationHandler running for requirement: ReadDoctor
  - [TestDiag] Authorization with permission: True
  - [TestDiag] Authorization without permission: False
  - [Audit][AuthZ] User:startupuser Permission:ReadDoctor Success:True Details:Permission check
  - [Audit][AuthZ] User:userwithperm Permission:ReadDoctor Success:True Details:Permission check
  - [Audit][AuthZ] User:usernoperm Permission:ReadDoctor Success:False Details:Permission check

- Request-level diagnostics showing endpoint mapping and responses:
  - [TestReq] GET /test/anon Endpoint: HTTP: GET /test/anon Response:200
  - [TestReq] GET /test/protected Endpoint: HTTP: GET /test/protected Response:401
  - [TestReq] GET /test/protected Endpoint: HTTP: GET /test/protected Response:403
  - [TestReq] GET /test/protected Endpoint: HTTP: GET /test/protected Response:200

What these tests exercise
- The /test/protected endpoint is mapped with RequireAuthorization("Permission:ReadDoctor") and exercises:
  - IAuthorizationPolicyProvider → PermissionPolicyProvider.GetPolicyAsync (production implementation)
  - IAuthorizationHandler → PermissionAuthorizationHandler.HandleRequirementAsync (production implementation)
  - Audit logging invoked by the handler (production IAuditService)

Which tests use test-only JWT generation
- AuthIntegrationTests.CreateUserAndLoginAsync generates deterministic JWTs using the test key configured in the PresentationApiFactory. Those tests validate JwtBearer authentication and the authorization pipeline but do not exercise the production TokenService/AuthService login flow.

Identity login paths intentionally not covered by current integration tests
- The following are outside the current test scope and must be tested separately if full end-to-end Identity validation is required:
  - AuthController login logic
  - AuthService (production service layer)
  - UserManager password validation and EF-backed Identity flows (UserManager.CreateAsync, change password, etc.)
  - MemberRepository user lookup and role retrieval
  - Production TokenService issuing tokens

Notes / rationale
- A test-host stub remains for an invalid login negative case. The test is renamed to Login_InvalidCredentials_Stubbed_Returns_401 and is annotated in the test to indicate it is a test-host stub only. Do not interpret this test as validating the real Identity login flow.
- The test host forces JwtBearer as the default authentication and challenge scheme and disables cookie redirect behavior to ensure deterministic 401/403 results (avoids intermittent 302 -> /Account/Login observed earlier).
- No production authentication/authorization behavior was changed to make tests pass.

How to reproduce the validated run
1. From the solution root run: dotnet test MEDSYstemITI.slnx
2. The presentationAPI project output will include the diagnostic lines listed above.

If you want additional evidence added to the repo (TRX files, full console logs, or a short script that runs the presentationAPI tests and collects logs), I can add that next.
