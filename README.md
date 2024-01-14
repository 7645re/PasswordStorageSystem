# Introduction

This is a test project to familiarize yourself with the basic capabilities of the Cassandra non-relational database.
The subject area of the test project was chosen randomly, the topic "Password storage system".
Project does not pretend to be truly correct solutions in the implementation of certain structures,
it was created solely for informational purposes

# Startup
```
docker run -d --name PasswordStorageSystem -p 9042:9042 cassandra:latest
```

# Chebotko Diagram
<img width="791" alt="image" src="https://github.com/7645re/PasswordStorageSystem/assets/89273037/4e710757-94b4-4808-9843-8b2e7fa16ef0">

# Swagger API
![image](https://github.com/7645re/PasswordStorageSystem/assets/89273037/d2222690-6595-4829-8035-837433a42ab0)


# Technologies used:
 1. ASP.NET CORE
 2. DataStax C# Driver
 3. Cassandra
 4. Docker
 5. Testcontainers
 6. XUnit, NUnit
 7. Github WorkFlow
 8. JWT Auth
 9. Swagger

# Emerging issues:
 1. It is quite difficult to write unit tests for repositories with cassandra
 2. At first, it was difficult to immediately design a database schema to meet my needs
