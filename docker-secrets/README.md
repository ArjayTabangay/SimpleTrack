How to use Docker secrets with this project

Production example (recommended):
1. Create secret files on the host machine (owner root)
   echo "your_db_password" > ./postgres_password.txt
   echo "your_jwt_secret" > ./jwt_key.txt

2. Create Docker secrets
   docker secret create postgres_password ./postgres_password.txt
   docker secret create jwt_key ./jwt_key.txt

3. Reference secrets in a compose file (example snippet):

services:
  api:
    image: your-image
    secrets:
      - jwt_key
    environment:
      - Jwt__Key_FILE=/run/secrets/jwt_key
    deploy:
      replicas: 1

  postgres:
    image: postgres:16-alpine
    secrets:
      - postgres_password
    environment:
      - POSTGRES_PASSWORD_FILE=/run/secrets/postgres_password

secrets:
  postgres_password:
    external: true
  jwt_key:
    external: true

4. In your app, prefer reading secrets from the files (e.g. Jwt__Key_FILE points to /run/secrets/jwt_key). You can modify Program.cs to load secret file values into configuration if needed.

Local Docker Compose tip (for testing):
- You can use the `file` driver in compose v3.8 by referencing an absolute path to a secret file in the `secrets` section. But in practice, using environment variables in CI/CD or container orchestration with secret support (Swarm, Kubernetes) is recommended.

Example Program.cs snippet to load secret file-based settings:

// Load file-based secret if env var with *_FILE is present
var jwtKeyFile = Environment.GetEnvironmentVariable("Jwt__Key_FILE");
if (!string.IsNullOrEmpty(jwtKeyFile) && File.Exists(jwtKeyFile))
{
    var key = File.ReadAllText(jwtKeyFile).Trim();
    builder.Configuration["Jwt:Key"] = key;
}

