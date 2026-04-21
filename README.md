# Contact List App
Application for managing a contact list.

## Technologies Used
- *Backend:* .NET
- *Frontend:* React
- *Database:* PostgreSQL
- *Containerization:* Docker

## Running the App
The recommended way to run the app is using Docker. 

```bash
1. Clone the repository:
   git clone https://github.com/KimPiks/Contact-List-App.git
2. Navigate to the project directory:
   cd Contact-List-App
3. Copy the .env.example file to .env and update the environment variables as needed:
   cp .env.example .env

   **Important:** It is required to fill in the secrets (e.g., `JWT_SECRET_KEY`, `CONTACT_ENCRYPTION_KEY`) in the `.env` file. For `JWT_ISSUER` and `JWT_AUDIENCE`, you can simply use `localhost`.

4. Build and run the Docker containers:
   docker-compose up --build
```

All database migrations will be run automatically when the containers start. The app will be accessible at `http://localhost:8080`.

## Docker Compose
The application ecosystem is fully containerized and orchestrated via Docker Compose (`compose.yaml`). The setup includes the following services:

- **postgres**: The main relational database (`postgres:18`). 
- **migrator**: A short-lived utility container based on the backend API image. It specifically runs Entity Framework Core migrations against the database (`efbundle`) immediately after the Postgres container is healthy.
- **netpc.api**: The main `.NET` backend service. It connects to the Postgres database and waits until the database is healthy *and* the `migrator` finishes updating the schema successfully before starting.
- **proxy**: An `nginx:alpine` container serving as a reverse proxy, mapping API routing over port 8080 and depending on the backend to be healthy.
- **netpc.frontend**: The `React` frontend application. Starts up only after the proxy is available, exposing its UI on port 3000.

## Project Structure
- `backend/`: Contains the .NET backend code.
- `frontend/`: Contains the React frontend code.
- `docker-compose.yml`: Docker configuration for running the app and database.
- `.env.example`: Example environment variables for the application.

## Backend Structure
Backend is structured into the following layers:
- *API*: Contains the controllers and API endpoints.
- *Application*: Contains the business logic and service layer.
- *Domain*: Contains the domain models and entities.
- *Infrastructure*: Contains the database context and repository implementations.

### Domain Layer
Contains the core business entities. It has no dependencies on other layers or external libraries. Key entities include:
- `User`: Inherits from `IdentityUser<Guid>`, representing a registered platform user.
- `Contact`: The central entity storing personal details (name, email, encrypted password, date of birth) and relations to categories.
- `Category` & `Subcategory`: Lookup tables for classifying contacts (e.g., Business with Subcategories like Boss, Client; Private; Other).
- `RefreshToken`: Manages the lifecycle of user authentication sessions.

### Application Layer
Responsible for application logic, use cases, and handling data transfer objects (DTOs). Key components:
- **Services (`ContactService`, `AuthService`)**: Implement the core business rules. 
- **Validation (`PasswordValidator`)**: Contains specific regex-based rules to ensure passwords inside contacts meet complexity requirements.
- **DTOs**: Data Transfer Objects (e.g., `CreateContactDto`, `ContactDto`, `LoginDto`) used to receive and return data safely without exposing database-specific Domain models.
- **Interfaces**: Abstractions (e.g., `IContactRepository`, `IEncryptionService`) defining the repository and service contracts, ensuring loose coupling with the Infrastructure layer.

### Infrastructure Layer
Handles data persistence, external communication, and framework-specific concerns. It includes:
- **`AppDbContext`**: The Entity Framework Core context configuring database schemas, seed data (for Categories/Subcategories), and foreign key constraints.
- **Repositories (`AuthRepository`, `ContactRepository`)**: Concrete implementations of application interfaces.
- **`EncryptionService`**: Implements symmetric AES encryption and decryption logic specifically for contact passwords.
- **`JwtService`**: Handles the generation of JWT access tokens and long-lived refresh tokens.

### API Layer
The entry point of the backend application (`NetPC.Api`). It includes:
- **Controllers (`AuthController`, `ContactsController`)**: Define the HTTP endpoints, handle routing, and return appropriate status codes (like 200 OK, 400 Bad Request). `ContactsController` is protected with the `[Authorize]` attribute to enforce token-based security.
- **`Program.cs`**: Bootstraps the application. It configures Dependency Injection (DI), hooks up Npgsql database connections, registers ASP.NET Core Identity, sets up JWT Bearer authentication options, and configures Swagger/OpenAPI pipelines.

## Security & Encryption
- **Authentication & User Management:** The application uses ASP.NET Core Identity for secure user management and password hashing for platform users. Security and stateless sessions are ensured via JWT (JSON Web Tokens). The authentication flow utilizes a dual-token system: short-lived Access Tokens for authorizing API requests and long-lived Refresh Tokens to securely obtain new access tokens without requiring the user to re-authenticate manually.
- **Contact Passwords:** Passwords stored within contact entries are encrypted symmetrically using the **AES (Advanced Encryption Standard)** algorithm, ensuring that sensitive contact data is protected. The encryption key is securely injected into the application via **environment variables**, preventing it from being hardcoded in the source code.

## Frontend Structure
The frontend is a Single Page Application (SPA) built using React and Vite. Key directories include:
- `src/api/`: Contains the pre-configured API client (`client.js`) for communicating with the .NET backend.
- `src/components/`: Reusable UI components such as forms (`AuthForm.jsx`, `ContactForm.jsx`), cards (`ContactCard.jsx`), full pages (`ContactsPage.jsx`), and specialized inputs (`PasswordField.jsx`).
- `src/utils/`: Helper constants and utility functions (e.g., `categories.js`).

## Libraries & Dependencies

### Backend Libraries
- **Entity Framework Core (with Npgsql)**: For Object-Relational Mapping (ORM) and connecting to the PostgreSQL database.
- **ASP.NET Core Identity**: To manage underlying user schemas and secure user credentials structure.

### Frontend Libraries
- **React**: Core UI library for building the interface layout and managing state.
- **Vite**: A lightning-fast build tool and development server.