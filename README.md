# NotificationService

A robust .NET 8 application for managing and delivering notifications via email and push notifications, built with Clean Architecture. It includes advanced features such as dynamic handler resolution, customizable SMTP configurations, SignalR for real-time communication, Razor templates for HTML email rendering, Quartz.NET for scheduling, and resource-based localization. The service also offers advanced logging via background services, dynamic query building, custom validation attributes, and standardized API responses. It includes integration and unit tests using NUnit and is designed with scalability and performance in mind.

---

## Features

### **Notification Management**
- **Push Notifications**: Real-time notifications powered by **SignalR**.
- **Email Notifications**: Configurable SMTP settings for personalized email delivery.
- **Background Services**:
  - Emails are queued and sent asynchronously to ensure performance and scalability.
  - Logs are processed in the background to maintain system responsiveness.

### **Scheduling with Quartz**
- Automated scheduling for notification sending using the **Quartz.NET** library.
- Supports both immediate and delayed job execution for emails and push notifications.

### **Custom Validation and Localization**
- **Resource-Based Localization**:
  - Fully localized error messages and validation attributes.
  - Dynamic culture resolution using cookies, query parameters, and browser headers.
- **Custom Validation Attributes**:
  - Includes rules like range, required, email validation, and custom domain-specific checks.
  
### **Template Rendering**
- Razor templates are dynamically rendered into HTML using **RazorLight** for:
  - Email bodies.
  - PDF generation with **iText7** for reporting.

### **Dynamic Handler and Provider Resolution**
- **Factory Design Pattern**:
  - Notification handlers and providers (e.g., email, push notifications) are resolved dynamically based on context.
  - Report generation handlers for different notification types.

### **Clean Architecture**
- Separation of concerns:
  - **API Layer**: Handles HTTP requests and responses.
  - **Application Layer**: Contains business logic and use cases.
  - **Domain Layer**: Defines core entities and rules.
  - **Infrastructure Layer**: Manages database access, external services, and background tasks.
- **Shared Library**: Common utilities, helpers, and extensions shared across layers.

### **Advanced Logging**
- **Custom Logger**:
  - Logs are managed through a queue-based **LogBackgroundService** to avoid blocking application performance.
  - Fine-grained control over logging levels for both application and third-party libraries.

### **Robust API Design**
- Bearer token authentication using JWT.
- Standardized API responses for all HTTP status codes, including `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, and `500 Internal Server Error`.
- Comprehensive error handling with custom middleware for exception management.

### **Dynamic Query Building**
- Custom query profiles allow dynamic filtering and predicate building for database queries.

---

## Technologies Used

### **Core Frameworks and Libraries**
- **.NET 8**
- **Entity Framework Core**: ORM for database interactions.
- **SignalR**: Real-time communication for push notifications.
- **Quartz.NET**: Scheduling library for task automation.
- **iText7**: PDF generation.
- **RazorLight**: Runtime Razor view rendering.

### **Validation and Localization**
- Resource-based localization using `.resx` files.
- Custom attributes for validation (e.g., email validation, required fields).

### **Logging and Monitoring**
- Custom logging system with queued background processing.

### **Testing**
- Integration and unit tests using **NUnit**.

---

## Project Structure

### **API Layer**
- Handles HTTP requests and responses.
- Includes controllers for managing notifications (push and email) and SMTP configurations.

### **Application Layer**
- Business logic for notification processing, scheduling, and validation.
- Contains dynamic mapping profiles for mapping DTOs to entities.

### **Domain Layer**
- Core entities (e.g., `Notification`, `EmailNotification`).
- Business rules and shared interfaces.

### **Infrastructure Layer**
- Handles data persistence, background services, and third-party integrations (e.g., SignalR, SMTP).
- Implements providers for notifications and jobs.

---

## How to Run

### Prerequisites
- Install **.NET 8 SDK**.
- Configure the database connection string in `appsettings.json`.

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/ConradArq/NotificationService.git

---

## License
This project is licensed under the [MIT License](LICENSE).