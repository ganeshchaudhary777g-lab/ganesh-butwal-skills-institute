# Deployment guide: GitHub, Neon, and Render

This repository is flat: `AdmissionWeb.csproj`, `Program.cs`, `Data`, `Models`, `Properties`, `wwwroot`, `Dockerfile`, `.dockerignore`, `.gitignore`, `ButwalAdmission.sln`, and the Markdown guides are at the repository root.

## 1. Upload the project to GitHub

1. Download and extract `Butwal-Admission-Final.zip` on your computer.
2. Open your GitHub repository and choose **Add file → Upload files**.
3. Upload the *contents* of the extracted folder so that `AdmissionWeb.csproj` and `Dockerfile` appear at the repository root. Do not upload an extra enclosing project folder.
4. Commit the files to the branch connected to Render. Keep `.env` files and database credentials out of GitHub.

## 2. Create a Neon PostgreSQL database

1. Sign in to Neon and create a project. Choose a region that is reasonably close to your Render service.
2. Open the new project and choose **Connect**.
3. Select the branch, database, and role you want the application to use. Copy the connection URL shown in the dialog. Neon shows the pooled URL by default; leave pooling on for this small web service.
4. Keep the URL private. It contains the database username and password. The application enables TLS and uses a maximum pool size of eight connections.

Neon documents the current Connect dialog and pooled connection URL in its [connection details guide](https://neon.com/docs/get-started/connect-neon) and [connection string FAQ](https://neon.com/docs/connect/connection-pooling).

## 3. Set Render environment variables

1. In the Render dashboard, open the existing web service for this repository.
2. Open **Environment** and add these variables:

   - `ADMIN_USERNAME` — choose the first administrator username (3–40 letters, digits, dot, underscore, or hyphen).
   - `ADMIN_PASSWORD` — choose a strong password of at least 14 characters.
   - `DATABASE_URL` — paste the complete Neon PostgreSQL URL, including its `postgresql://` prefix.

3. Save and deploy the environment changes. Do not paste the database URL into source code, README files, or a GitHub issue.
4. Confirm the Render service uses the repository root as its Docker build context and the included `Dockerfile`. The container listens on port `10000`.

Render's dashboard currently provides environment variables under the service's **Environment** page; see [Render environment-variable setup](https://render.com/docs/configure-environment-variables). The repository is configured for a Docker web service; see [Render Docker deploys](https://render.com/docs/docker).

On the first start, the app creates its PostgreSQL tables and the initial administrator if that username does not exist. On later starts it does not replace the existing admin password. ASP.NET Data Protection keys are stored in PostgreSQL so authentication cookies can survive redeploys.

## 4. Verify the deployment and durable data

1. Wait for Render to show the deploy as live, then open the service URL and check that the home page and course list load.
2. Submit a test application with a test phone number. Save its application number. The payment state should be **Pending Verification**.
3. Open `/admin.html`, sign in using the initial admin account, and verify the test application appears. Use **Export CSV** if you need a backup.
4. Check the application status using the test application number and the same mobile number.
5. Restart or redeploy the Render service. Sign in again, then check the application by its number and mobile number. The record should still be present because it is stored in Neon.
6. Delete the test record or keep it only if the institute wants it retained. Do not use real student data for an initial smoke test.

## Local development

If `DATABASE_URL` is absent, the app uses SQLite under `App_Data` and stores Data Protection keys next to that database. For a local first admin, run `dotnet run -- --setup-admin`. Do not commit the local database or key folder.

## Operational note

The application changes its schema automatically and idempotently. This package does not copy old SQLite records into Neon. If you have real records in an existing SQLite database, export and migrate them through a separately reviewed migration before switching production traffic.
