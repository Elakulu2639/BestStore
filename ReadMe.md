# BestStore - Modern E-commerce Platform

![ASP.NET Core](https://img.shields.io/badge/.NET-9.0-blue)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019+-blue)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.0+-purple)
![jQuery](https://img.shields.io/badge/jQuery-3.6+-green)

BestStore is a sophisticated e-commerce platform built with ASP.NET Core MVC, featuring a modern architecture and comprehensive set of features for both customers and administrators.

## 🌟 Key Features

### Customer Experience
- **Intuitive Shopping Interface**
  - Responsive design using Bootstrap 5
  - Dynamic product catalog with search and filtering
  - Cookie-based shopping cart for persistent sessions
  - Seamless checkout process

- **Smart Recommendations**
  - Personalized product suggestions based on purchase history
  - "Frequently bought together" recommendations
  - Intelligent product associations

- **User Account Management**
  - Secure registration and login
  - Profile management
  - Order history tracking
  - Password reset functionality

### Administrative Features
- **Comprehensive Dashboard**
  - Sales analytics with Chart.js visualizations
  - Order management system
  - User management interface
  - Product inventory control

- **Product Management**
  - CRUD operations for products
  - Image upload and management
  - Category and brand organization
  - Stock tracking

## 🛠 Technical Stack

### Backend
- **Framework**: ASP.NET Core 9.0
- **Language**: C#
- **Database**: SQL Server 2019+
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity

### Frontend
- **UI Framework**: Bootstrap 5
- **JavaScript Libraries**: 
  - jQuery for dynamic interactions
  - Chart.js for analytics
- **Templating**: Razor Views
- **Styling**: CSS3 with Bootstrap components

### Services
- Email service for notifications
- Recommendation engine
- Cart management system
- Database initialization and seeding

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- SQL Server 2019+
- Visual Studio 2022+ (Recommended)
- Git

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/ElaKulu2639/BestStore.git
   cd BestStore
   ```

2. **Database Setup**
   - Create a SQL Server database named "ProductStoreDb"
   - Update the connection string in `appsettings.json`:
   ```json
   "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ProductStoreDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

3. **Apply Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

## 📱 Features in Detail

### Shopping Experience
- Browse products with advanced filtering
- Add items to cart with quantity management
- Secure checkout process
- Order tracking and history

### Admin Dashboard
- Real-time sales analytics
- Order management and status updates
- User management and role assignment
- Product inventory control
- Sales reports and insights

### Security Features
- Role-based access control
- Secure password hashing
- Session management
- CSRF protection
- Input validation

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request. For major changes, please open an issue first to discuss what you would like to change.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 👨‍💻 Author

**Elias Aynekulu**
- Email: e9710092@gmail.com

## 🙏 Acknowledgments

- Bootstrap team for the amazing UI framework
- Chart.js for the visualization capabilities
- ASP.NET Core team for the robust framework
