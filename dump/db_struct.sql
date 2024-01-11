-- Create database
CREATE DATABASE Store;
GO

-- Use the store database
USE Store;
GO

-- Create roles table
CREATE TABLE roles (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(20)
);
GO

-- Create users table
CREATE TABLE users (
    id INT PRIMARY KEY IDENTITY(1,1),
    role_id INT NOT NULL,
    username NVARCHAR(255) NOT NULL UNIQUE,
    first_name NVARCHAR(255) NOT NULL,
    last_name NVARCHAR(255) NOT NULL,
    birth_date DATETIME NOT NULL,
    gender NVARCHAR(255) NOT NULL CHECK (gender IN('male', 'female')),
    address NVARCHAR(255) NOT NULL,
    email NVARCHAR(255),
    phone_number CHAR(10),
    password NVARCHAR(60),
    FOREIGN KEY (role_id) REFERENCES roles(id)
);
GO
--
CREATE UNIQUE INDEX UQ_phone_number
ON users (phone_number)
WHERE phone_number IS NOT NULL;
--
CREATE UNIQUE INDEX UQ_email
ON users (email)
WHERE email IS NOT NULL;
--
-- Create suppliers table
CREATE TABLE suppliers (
    id INT PRIMARY KEY IDENTITY(1,1),
    supplier_name NVARCHAR(255) NOT NULL,
    address NVARCHAR(255) NOT NULL,
    email NVARCHAR(255),
    phone_number NVARCHAR(12),
    CONSTRAINT UQ_supplier_name UNIQUE (supplier_name),
    CONSTRAINT UQ_supplier_email UNIQUE (email),
    CONSTRAINT UQ_supplier_phone_number UNIQUE (phone_number)
);
GO

-- Create categories table
CREATE TABLE categories (
    id INT PRIMARY KEY IDENTITY(1,1),
    category_name NVARCHAR(255) NOT NULL,
    description NTEXT
);
GO

-- Create products table
CREATE TABLE products (
    id INT PRIMARY KEY IDENTITY(1,1),
    product_name NVARCHAR(255) NOT NULL,
    supplier_id INT,
    category_id INT,
    view_count INT NOT NULL DEFAULT 0,
    price INT NOT NULL,
    quantity INT NOT NULL DEFAULT 0,
    sold_quantity INT NOT NULL DEFAULT 0,
    discount DECIMAL(1,1),
    images NVARCHAR(500),
    description NTEXT,
    CONSTRAINT discount_limit CHECK (discount > 0 AND discount < 1),
    FOREIGN KEY (supplier_id) REFERENCES suppliers(id),
    FOREIGN KEY (category_id) REFERENCES categories(id)
);
GO

-- Create comments table
CREATE TABLE comments (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    comment NVARCHAR(255) NOT NULL,
    comment_date DATE NOT NULL,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (product_id) REFERENCES products(id)
);
GO

-- Create ratings table
CREATE TABLE ratings (
    user_id INT NOT NULL,
    product_id INT NOT NULL,
    rate INT,
    PRIMARY KEY (user_id, product_id),
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (product_id) REFERENCES products(id),
    CONSTRAINT rate CHECK (rate >= 1 AND rate <= 5)
);
GO

-- Create vouchers table
CREATE TABLE vouchers (
    id NVARCHAR(60) PRIMARY KEY,
    voucher_name NVARCHAR(255) NOT NULL,
    voucher_discount DECIMAL(1,1) NOT NULL,
    expiry_date DATE NOT NULL,
    description NTEXT,
    CONSTRAINT voucher_limit CHECK (voucher_discount > 0 AND voucher_discount < 1)
);
GO

-- Create payment_methods table
CREATE TABLE payment_methods (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(255)
);
GO

-- Create orders table
CREATE TABLE orders (
    id INT PRIMARY KEY IDENTITY(1,1),
    user_id INT NOT NULL,
    order_date DATETIME,
    voucher_id NVARCHAR(60),
    paid_method_id INT,
    paid BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (user_id) REFERENCES users(id),
    FOREIGN KEY (voucher_id) REFERENCES vouchers(id),
    FOREIGN KEY (paid_method_id) REFERENCES payment_methods(id)
);
GO

-- Create order_items table
CREATE TABLE order_items (
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    quantity INT NOT NULL,
    price INT NULL,
    discount DECIMAL(1,1) NULL,
    CONSTRAINT PK_order_items PRIMARY KEY (order_id, product_id),
    FOREIGN KEY (order_id) REFERENCES orders(id),
    FOREIGN KEY (product_id) REFERENCES products(id),
    CONSTRAINT atleast_quantity CHECK (quantity > 0),
    CONSTRAINT order_items_discount_limit CHECK (discount > 0 AND discount < 1)
);
GO
