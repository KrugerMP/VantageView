# VantageView API - Bruno Collection

## Endpoints

| Request | Method | Auth | Description |
|---------|--------|------|-------------|
| Get All Articles | GET | No | List all articles |
| Get Article by Id | GET | No | Get single article by GUID |
| Create Article | POST | Bearer | Create new article (admin) |
| Update Article | PUT | Bearer | Update article (admin) |
| Delete Article | DELETE | Bearer | Delete article (admin) |
| Articles Health | GET | No | Health check for articles service |

## Admin Endpoints (JWT)

Create, Update, and Delete require a JWT Bearer token. To get a token:

1. Run the **Auth** service (`VantageView.Auth`)
2. POST to `http://localhost:7128/api/auth/login` with:
   ```json
   { "username": "admin", "password": "admin" }
   ```
3. Copy the `token` from the response
4. In Bruno, set **authToken** (collection variable) or add header: `Authorization: Bearer <token>`

## Variables

- **articleId**: Set this with a GUID from "Get All Articles" response when testing Get/Update/Delete by Id
- **authToken**: Set with JWT from Auth service login for admin endpoints
