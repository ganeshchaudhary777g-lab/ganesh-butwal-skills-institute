# Review and repair notes

This audit covers the application files delivered in this repository. Line references point to the final files in this ZIP. The listed repairs were applied in the source. Npgsql restore later succeeded after allowing NuGet access.

| Severity | Finding | Location | Repair |
|---|---|---|---|
| High | Production SQLite files and local key-ring files disappear with an ephemeral host filesystem. | `Program.cs:55-68`; `Data/AdmissionStore.cs:16-49` | Added PostgreSQL selected by `DATABASE_URL`, with SSL required, pooling capped at eight, and SQLite retained when the variable is absent. Added a database-backed XML repository for Data Protection keys. |
| High | The project did not bootstrap the administrator from the documented environment variables. | `Program.cs:126-128`; `Data/AdmissionStore.cs:299-319` | Added `ADMIN_USERNAME` / `ADMIN_PASSWORD` startup bootstrap. It inserts only when that case-insensitive username is missing and never replaces an existing password. |
| High | Existing SQLite SQL included `AUTOINCREMENT`, `BLOB`, `last_insert_rowid()`, `COLLATE NOCASE`, and `INSERT OR IGNORE`, which do not transfer directly to PostgreSQL. | `Data/AdmissionStore.cs:137-176, 227-239, 579-601` | Added dialect conversion for identity columns, big integer/FK types and byte arrays; changed inserts to `RETURNING` and `ON CONFLICT`; comparisons use `LOWER`. Query values remain parameters. |
| High | Student-created or administrator-supplied strings could become a stored XSS risk if inserted as HTML. | `wwwroot/admin.js:9-11, 37-49, 80-107, 140-181, 198` | The dashboard builds nodes and writes untrusted values using `textContent`; no `innerHTML` interpolation is used. This safe rendering pattern was retained. |
| High | State-changing requests, including public application creation, must reject cross-site submissions. | `Program.cs:164-178, 203, 218, 236, 243, 251, 259, 282-286`; `wwwroot/site.js` | Added antiforgery validation to public application creation. The browser obtains a request token and sends it in `X-CSRF-TOKEN`; admin state-changing routes retain the same validation. |
| High | The admin session cookie must be protected in transit and inaccessible to JavaScript. | `Program.cs:76-96` | Verified `HttpOnly`, `SameSite=Strict`, `SecurePolicy=SameAsRequest`, expiry, and sliding-expiration settings; retained them. Production also enforces HTTPS after forwarded-header processing. |
| High | Payment state must not be inferred from a submitted reference or QR scan. | `Data/AdmissionStore.cs:237-250`; `wwwroot/index.html:49,55-56` | New applications and payments remain `Pending Verification`; the public page states that the institute verifies claims manually. Only an authenticated admin update can change the state. |
| Medium | Nepal mobile input needs server-side format checks independent of browser validation. | `Program.cs:192, 297-308, 329` | Kept the server check for ten-digit Nepal mobile numbers (optionally prefixed with `+977`) for applicants, emergency contacts, and tracking. |
| Medium | The original page did not have an interactive QR enlargement flow or contextual QR display in the application payment step. | `wwwroot/index.html:49,55-56`; `wwwroot/site.js` | Added the supplied image at `wwwroot/assets/esewa-qr.png`, QR dialog, download action, alt text, and a QR panel for eSewa/QR Payment selections with a transaction-reference field. |
| Medium | Mobile field font size and touch-target sizing could be too small, and course/payment cards had no staged reveal. | `wwwroot/site.css`; `wwwroot/admin.css`; `wwwroot/site.js` | Added fluid design tokens, focus glow, reveal observer, reduced-motion handling, 16px mobile controls, 44px targets, safe-area spacing, `100dvh`, and horizontal table scrolling on small screens. |
| Medium | CSS was compressed into dense single-line rules, making responsive and accessibility fixes hard to review. | `wwwroot/site.css`; `wwwroot/admin.css` | Reformatted the existing styles and added design tokens and comments while keeping the existing page layout and palette. |
| Medium | Compressed responses and a friendly fallback for unknown routes were missing. | `Program.cs:131, 149-160, 279` | Enabled ASP.NET response compression, added safe static cache headers and a small HTML 404 fallback. |
| Medium | Security response headers were incomplete. | `Program.cs:137-145` | Added a self-hosted Content Security Policy alongside `nosniff`, frame denial, referrer and permissions policies, and production HSTS. Inline style attributes remain allowed for a small existing script-set style value; scripts and connections remain same-origin. |
| Low | Public page metadata had no social preview fields. | `wwwroot/index.html:5` | Added Open Graph title, description, type, image, and theme-color metadata. The existing logo file remains unchanged in the brand header; `wwwroot/favicon.svg` embeds it as its favicon source. |
| Low | The static QR card had no save action and its label described the placeholder rather than the supplied payment code. | `wwwroot/index.html:55-56` | Added a download link, simple scan steps, and accurate manual-verification language. |
| Info | Admin CSV export already existed and CSV formula-leading values were escaped. | `Program.cs:266-276`; `wwwroot/admin.html:4` | Verified and retained the authenticated dashboard export. |
| Info | Password hashing, login throttling, lockout/rate-limit policies, and the server-side fee calculation were already present. | `Data/AdmissionStore.cs`; `Program.cs:99-108, 201-215, 292-333` | Retained PBKDF2-SHA256 password storage, login rate limiting, server validation, and catalogue-based fee calculation without changing course prices or payment rules. |

## Verification limits

- `dotnet build ButwalAdmission.sln --no-restore` passed on .NET SDK 10.0.401 with 0 warnings and 0 errors after NuGet restore succeeded.
- A temporary SQLite smoke harness passed course seeding, idempotent admin bootstrap/password retention, application submission, status tracking, export query inclusion, and the `Pending Verification` payment state.
- The web host started but the sandbox denied loopback access from the separate test process. Browser-based API flow and the 320–1440 px screenshot sweep could not be completed. Headless Edge also exited with code 13. No fabricated screenshots or Lighthouse scores are included.
- No live Neon/PostgreSQL credentials were available, so PostgreSQL runtime behavior was not exercised. The Docker CLI is unavailable, so `docker build` could not run.

## Final visual polish added in this pass

- Added a low-opacity institute-logo watermark behind transparent page areas.
- Added a restrained animated hero orbit/glow layer, gradient edge highlights on course/payment cards, and smoother button/navigation interactions.
- Added a logo to the administrator header.
- Added clear keyboard focus styling and ensured the new decorative motion stops when `prefers-reduced-motion` is enabled.

## Checks performed for this packaged revision

- `AdmissionWeb.csproj` parses as valid XML.
- Both JavaScript files pass Node.js syntax checks.
- Public and admin HTML have no duplicate IDs; literal JavaScript DOM-ID references resolve to elements in their matching HTML pages.
- Referenced local static assets were checked and found.
- CSS brace counts are balanced.

The current editing environment does not have the .NET SDK or Docker CLI installed, so a fresh `dotnet publish` and `docker build` could not be run here. The included deployment guide still requires a real Render deploy and a configured PostgreSQL database to confirm live startup, database connectivity, form submission, and admin login. No claim of a verified live deployment is made.
