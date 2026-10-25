# Changelog

- `.gitignore` — excluded build outputs, local databases, key rings, and environment secrets.
- `.dockerignore` — excluded build outputs, backups, videos, local data, and secrets from Docker context.
- `AdmissionWeb.csproj` — added Npgsql for PostgreSQL persistence.
- `ButwalAdmission.sln` — updated the project reference for the flat repository layout.
- `Data/AdmissionStore.cs` — added provider selection, PostgreSQL schema compatibility, case-insensitive admin bootstrap, bounded pooling, and cross-provider inserts.
- `Data/DatabaseXmlRepository.cs` — added PostgreSQL-backed ASP.NET Data Protection key storage.
- `Data/schema.sql` — documented the SQLite Data Protection key table for local initialization.
- `DEPLOY_GUIDE.md` — added GitHub, Neon, Render, and restart-verification instructions.
- `Dockerfile` — added multi-stage .NET 10 container publishing, non-root runtime, and port 10000 configuration.
- `Program.cs` — added PostgreSQL/key-storage startup, admin bootstrap, CSRF validation, response compression, security headers, cache policy, and 404 fallback.
- `README.md` — updated root-layout build and production database instructions.
- `REVIEW_AND_REPAIR.md` — recorded audit findings, repairs, and verification limits.
- `wwwroot/admin.css` — formatted and restyled the admin dashboard with tokens, focus states, and mobile table scrolling.
- `wwwroot/assets/esewa-qr.jpg` — removed the older duplicate QR asset.
- `wwwroot/assets/esewa-qr.png` — added the supplied eSewa QR image without image processing.
- `wwwroot/favicon.svg` — added a favicon that embeds the unchanged logo asset.
- `wwwroot/index.html` — added QR payment guidance, QR dialog, conditional payment-step QR, and Open Graph metadata.
- `wwwroot/site.css` — formatted and extended the public design system with responsive sizing, glow, reduced-motion, and QR dialog styles.
- `wwwroot/site.js` — added QR dialog controls, conditional QR panel, scroll reveals, and mobile-menu state handling.
- `docs/screenshots/README.md` — documented the unavailable browser/screenshot checks.

`wwwroot/assets/butwal-skills-logo.png` was verified by SHA-256 and left byte-for-byte unchanged.
