# TULOS API

## Overview

This project provides the backend API for the TULOS e-commerce platform. Built using **ASP.NET Core** and **Entity Framework Core**, it handles user authentication, product management, favorites, and cart functionality. It also supports secure communication between the frontend and backend using token based authentication.

## Features

- **User Authentication**: Register, login, logout, and reset password functionality.
- **Favorites Management**: Add and remove products from a user's favorites list.
- **Cart Management**: Handle adding/removing products in the shopping cart.
- **Token Based Authentication**: Secure authentication using custom tokens for API access.

## Technologies Used

- **ASP.NET Core**
- **Entity Framework Core**
- **SQL Server**
- **ASP.NET Core Identity Custom Token for Authentication**
- **Mailtrap** for email services

## API Endpoints

### Authentication

- `POST /api/Account/register`: Register a new user.
-  `POST /api/Account/confirmEmail`: Activate new user account.
-  `POST /api/Account/checkUser`: Checks if user exists.
- `POST /login`: Authenticate user and return custom token.
- `POST /api/Account/forgotPassword`: Send password reset email.
- `POST /api/Account/resetPassword`: Reset password using the reset code.
- `POST /api/Account/changePassword`: Change user password.
- `POST /api/Account/logout`: Signs out user.

### Favorites
- `GET /api/Favorite`: Retrieve all favorite products.
- `GET /api/Favorite/{email}`: Retrieve a user's favorite products.
- `POST /api/Favorite/addToFavorite`: Add or remove a product from user's favorites.

### Cart

- `GET /api/Cart/{email}`: Retrieve a user's cart.
- `POST /api/Cart/addToCart`: Add a product to the cart.
- `DELETE /api/Cart/removeFromCart`: Remove a product from the cart.
