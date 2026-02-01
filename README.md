# OTP Authorizer Service

A lightweight, secure, and extensible .NET-based microservice designed to handle One-Time Password (OTP) generation and verification. This service supports multiple delivery channels (SMS, Email), secure storage using Redis, and configurable security policies.

## 1. Information

The **OTP Authorizer** provides a centralized API to manage the lifecycle of temporary authentication codes. 

### Key Features:
*   **Multi-Channel Support:** Built-in support for SMS and Email delivery via decorator patterns.
*   **Secure Storage:** Integration with **Redis** for high-performance, self-expiring OTP storage.
*   **Security First:** 
    *   Optional SHA256 hashing of OTPs before storage (`IOtpProtector`).
    *   API Key authentication required for all endpoints.
    *   Configurable code complexity (length, leading zeros, repeated digit constraints).
*   **Extensible Architecture:** Easily add new channels or storage providers by implementing defined interfaces (`IOtpStore`, `IOtpGenerator`).
*   **API Documentation:** Integrated with **Scalar** and **OpenAPI** for interactive documentation.

---

## 2. Setup Environments

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or newer.
- [Redis](https://redis.io/) (Local instance or Cloud).

### Configuration
Update your `appsettings.json` to configure the Redis connection and OTP behavior:

```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "Otp": {
    "EncryptBeforeStore": true,
    "Channels": {
      "sms": {
        "CodeLength": 6,
        "StartWithZero": true,
        "MaxRepeatedDigits": 2,
        "ExpirationInMinutes": 5
      },
      "email": {
        "CodeLength": 8,
        "StartWithZero": false,
        "MaxRepeatedDigits": 3,
        "ExpirationInMinutes": 10
      }
    }
  }
}
```

### Authentication
The service uses API Key authentication. Ensure you provide a valid key in the `X-Api-Key` header for every request. You can manage authorized keys within the `ApiKeyAuthenticateHandler`.

### Running the Project
1. Clone the repository.
2. Navigate to the project folder.
3. Run the application:
   ```bash
   dotnet run
   ```
4. Access the API documentation (Scalar) at: `https://localhost:{port}/scalar/v1`

---

## 3. Usage

The service exposes two primary endpoints under the `otp` group.

### Authentication Header
All requests must include the following header:
`X-Api-Key: your_secure_api_key`

---

### A. Generate OTP
Generates a new code for a specific client and channel, stores it in Redis, and triggers the delivery decorator.

**Endpoint:** `POST /otp/{channel}/generate?client={client_id}`

**Example Request:**
```bash
curl -X 'POST' \
  'https://localhost:5001/otp/sms/generate?client=user@example.com' \
  -H 'X-Api-Key: your_api_key_here'
```

**Successful Response (200 OK):**
```json
{
  "channel": "sms",
  "client": "user@example.com",
  "code": "048231",
  "length": 6,
  "expireIn": "2023-10-27T10:05:00Z",
  "expirationInMinutes": 5
}
```

---

### B. Verify OTP
Validates the provided code against the stored value for the specific client.

**Endpoint:** `POST /otp/{channel}/verify?client={client_id}&otp={code}`

**Example Request:**
```bash
curl -X 'POST' \
  'https://localhost:5001/otp/sms/verify?client=user@example.com&otp=048231' \
  -H 'X-Api-Key: your_api_key_here'
```

**Responses:**
- `200 OK`: Verification successful. The OTP is consumed and removed from storage.
- `401 Unauthorized`: Verification failed (invalid code, expired, or wrong client).
- `400 Bad Request`: Invalid channel or missing parameters.

---

## 4. Architecture Overview

- **`IOtpStore`**: Handles the persistence logic. The default implementation uses Redis with TTL (Time-To-Live) matching the OTP expiration.
- **`IOtpProtector`**: Provides a layer of security by hashing the OTP (SHA256) before it hits the database.
- **Decorators**: `SmsOtpDecorator` and `EmailOtpDecorator` wrap the base generator logic to integrate with third-party messaging providers (e.g., Twilio, SendGrid).
- **Minimal APIs**: Efficiently routes requests using the latest .NET routing patterns for high performance.
