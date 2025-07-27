
# 🧠 Grocery List API – Backend (.NET 8)

This is the backend API for the Grocery List web application. It is built using **ASP.NET Core (.NET 8)** with **Entity Framework Core** and **SQLite** as the database.

---

## 📦 Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQLite (included with EF Core, no installation required)

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/Aleja6458/ConceptDemoWebAPI.git
```

### 2. Setup the Database

Open the **Package Manager Console** in Visual Studio and run the following commands:

```powershell
Add-Migration InitialCreate
Update-Database
```

> These commands will generate the migration files and create the SQLite database (`app.db`) in the root of the project.

---

## ▶️ Run the Application

You can start the API using:

```bash
dotnet run
```

The API will be available at:

```
https://localhost:7208
```

---

## 🔐 Authentication

- User login returns an authentication token.
- Each request must include the token in the `Authorization` header.

Example:

```
Authorization: Bearer your-token-here
```

---

## 📚 Main Endpoints

| Method | Endpoint                    | Description                            |
| ------ | --------------------------- | -------------------------------------- |
| POST   | /api/users/register         | Register a new user                    |
| POST   | /api/users/login            | Login and receive a JWT token          |
| POST   | /api/users/logout           | Logout user by invalidating token      |
| POST   | /api/users/validate-session | Check if session token is still active |
| GET    | /api/users/{id}             | Get user info by ID                    |
| GET    | /api/products/user/{userId} | Get product list for a specific user   |
| POST   | /api/products               | Create a new product                   |
| PUT    | /api/products/{id}          | Update product status                  |
| DELETE | /api/products/{id}          | Delete a product                       |


---

## 📁 Project Structure

```
2Share/
├── Controllers/
├── Models/
├── Data/
├── Migrations/
├── app.db
├── Program.cs
└── appsettings.json
```

---

## ✅ Notes

- Database is created locally using SQLite.
- You can reset the DB by deleting `app.db` and running `Update-Database` again.

---

## 📄 License

This project is for educational and assessment purposes.
