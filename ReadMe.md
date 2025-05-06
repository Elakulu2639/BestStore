<<<<<<< HEAD
﻿# BestStore - ASP.NET MVC E-commerce Platform

![ASP.NET Core](https://img.shields.io/badge/.NET-9.0-blue)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019+-blue)

BestStore is a feature-rich e-commerce platform built with ASP.NET Core, designed to manage products, orders, and user interactions efficiently.

## Table of Contents
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [Author](#author)

## Features ✨
- **User Authentication and Authorization**:
  - Secure login and registration using ASP.NET Core Identity.
  - Role-based access control for different user roles (Admin, Customer).
  - Password hashing and secure session management.

- **Product Management**:
  - Comprehensive CRUD operations for products.
  - Image upload with validation (JPEG/PNG).
  - Category and brand management.

- **Order Management**:
  - Place orders and manage order statuses.
  - View order history and details.
  - Generate sales reports.

- **Recommendation System**:
  - Personalized product recommendations based on user purchase history.
  - Frequently bought together products.

- **Responsive Design**:
  - Fully responsive UI using Bootstrap 5.
  - Modern and user-friendly interface.

## Tech Stack 🛠
Frontend  
| ![Bootstrap](https://img.shields.io/badge/Bootstrap-5.0+-purple) | ![Razor](https://img.shields.io/badge/Razor-ASP.NET-blue) | ![Font Awesome](https://img.shields.io/badge/Font_Awesome-6.0+-orange) |
|------------------------------------------------------------------|-----------------------------------------------------------|------------------------------------------------------------------------|

Backend  
| ![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-9.0-blue) | ![EF Core](https://img.shields.io/badge/EF_Core-9.0-red) |
|---------------------------------------------------------------------|----------------------------------------------------------|

Database  
| ![SQL Server](https://img.shields.io/badge/SQL_Server-2019+-blue) |
|-------------------------------------------------------------------|

## Getting Started 🚀

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) (Recommended)
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/)

### Installation
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/ElaKulu2639/BestStore.git
   cd BestStore

2. Configure the database:
     -Create SQL Server database "ProductStoreDb"
     -Update connection string in appsettings.json:
   ```
   
        "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
   }

3. Apply database migrations:
   
     Update-Database

4. Run the application

## Usage 📚

User Authentication:
  -Register or log in to access the platform.
  -Admin users can manage products and orders.
Product Management:
   -Admins can add, edit, and delete products.
   -Users can view product details and place orders.
Order Management:
   -Users can view their order history.
   -Admins can manage order statuses and generate reports.

## Contributing 🤝
If you would like to contribute to BestStore, please follow these steps:
1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Make your changes and commit them (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Create a new Pull Request.


## Author 👨💻
Elias Aynekulu
Email: e9710092@gmail.com
GitHub: ElaKulu2639
