A robust .NET Core Web API for managing masjid locations with PostgreSQL and PostGIS support.

📋 Prerequisites
.NET 8.0 SDK

PostgreSQL 12+ with PostGIS extension

Entity Framework Core CLI tools

🛠️ Installation
Clone the repository


git clone <your-repo-url>
cd MasjidLocatorAPI
Configure database connection
Update appsettings.json with your PostgreSQL credentials:

json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=masjid_locator;Username=postgres;Password=your_password"
}
Install dependencies


dotnet restore
Database setup


# Enable PostGIS extension in PostgreSQL
CREATE EXTENSION postgis;

# Apply migrations
dotnet ef database update
Run the API


dotnet run
📊 API Endpoints

Submissions
GET /api/submissions - Get all submissions

GET /api/submissions/{id} - Get submission by ID

GET /api/submissions/user/{userId} - Get user submissions

POST /api/submissions - Create new submission

PUT /api/submissions/{id} - Update submission

DELETE /api/submissions/{id} - Delete submission

Authentication
POST /api/auth/register - User registration

POST /api/auth/login - User login

🗄️ Database Schema
sql
CREATE TABLE submissions (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    location geography(Point, 4326),
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

🔧 Configuration
Environment Variables

export ASPNETCORE_ENVIRONMENT=Development
export DB_HOST=localhost
export DB_PORT=5432
export DB_NAME=masjid_locator
export DB_USER=postgres
export DB_PASSWORD=your_password
export JWT_SECRET=your_jwt_secret
appsettings.json
json
{
  "Jwt": {
    "Key": "your_super_secret_key",
    "Issuer": "masjid_locator_api",
    "Audience": "masjid_locator_app"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
🧪 Testing

# Run tests
dotnet test

# Test API endpoints
curl -X GET "https://localhost:7000/api/submissions"
🐛 Troubleshooting
Common Issues
PostGIS extension missing

sql
CREATE EXTENSION postgis;
Geography type errors

Ensure proper casting: ST_Y(location::geometry)

Connection issues

Verify PostgreSQL is running

Check connection string credentials
