# API Test Handbook - Curl Commands

This handbook provides comprehensive curl commands to test all endpoints in the API Demo application.

## Table of Contents
1. [Prerequisites](#prerequisites)
2. [Authentication](#authentication)
3. [Base Configuration](#base-configuration)
4. [Profile Management APIs](#profile-management-apis)
5. [Academic Products APIs](#academic-products-apis)
6. [Collaboration Requests APIs](#collaboration-requests-apis)
7. [User Profile APIs (Me)](#user-profile-apis-me)
8. [Error Handling](#error-handling)
9. [Testing Scenarios](#testing-scenarios)

## Prerequisites

- API server running on `http://localhost:5000` (or your configured port)
- Valid JWT token for authentication
- curl command-line tool installed

## Authentication

All endpoints require authentication. You'll need to obtain a JWT token first. For testing purposes, you can:

1. Use a token from your authentication system
2. Or modify the `[Authorize]` attribute temporarily for testing

**Note**: Replace `YOUR_JWT_TOKEN` in all examples with your actual JWT token.

## Base Configuration

Set up your base variables:

```bash
# Base URL
BASE_URL="http://localhost:5000"

# JWT Token (replace with your actual token)
JWT_TOKEN="YOUR_JWT_TOKEN"

# Common headers
HEADERS="-H 'Content-Type: application/json' -H 'Authorization: Bearer $JWT_TOKEN'"
```

## Profile Management APIs

### 1. Get All Profiles
```bash
curl -X GET "$BASE_URL/api/profiles" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 2. Get Profile by ID
```bash
# Replace {profile-id} with actual profile ID
curl -X GET "$BASE_URL/api/profiles/{profile-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 3. Create Profile
```bash
curl -X POST "$BASE_URL/api/profiles" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "role": 0
  }'
```

**Role Values:**
- `0` = Member
- `1` = Enterprise  
- `2` = Expert
- `3` = Admin

### 4. Update Profile Role (Admin only)
```bash
curl -X PUT "$BASE_URL/api/profiles/{profile-id}/role" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "role": 2
  }'
```

### 5. Delete Profile
```bash
curl -X DELETE "$BASE_URL/api/profiles/{profile-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

## Academic Products APIs

### 1. Get All Academic Products
```bash
curl -X GET "$BASE_URL/api/academicproducts" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 2. Get Academic Product by ID
```bash
curl -X GET "$BASE_URL/api/academicproducts/{product-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 3. Get Academic Products by Expert ID
```bash
curl -X GET "$BASE_URL/api/academicproducts/expert/{expert-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 4. Create Academic Product
```bash
curl -X POST "$BASE_URL/api/academicproducts" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "achievements": {
      "publications": 5,
      "awards": ["Best Paper 2023", "Innovation Award"],
      "hIndex": 15
    },
    "title": "Advanced Machine Learning Research",
    "description": "Comprehensive research on deep learning algorithms and their applications in real-world scenarios."
  }'
```

**Note**: The `expertId` is automatically set from the current authenticated user's ID.

### 5. Update Academic Product
```bash
curl -X PUT "$BASE_URL/api/academicproducts/{product-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "achievements": {
      "publications": 7,
      "awards": ["Best Paper 2023", "Innovation Award", "Research Excellence"],
      "hIndex": 18
    },
    "title": "Advanced Machine Learning Research - Updated",
    "description": "Updated comprehensive research on deep learning algorithms with new findings."
  }'
```

### 6. Delete Academic Product
```bash
curl -X DELETE "$BASE_URL/api/academicproducts/{product-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

## Collaboration Requests APIs

### 1. Get All Collaboration Requests
```bash
curl -X GET "$BASE_URL/api/collaborationrequests" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 2. Get Collaboration Request by ID
```bash
curl -X GET "$BASE_URL/api/collaborationrequests/{request-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 3. Get Collaboration Requests by Sender ID
```bash
curl -X GET "$BASE_URL/api/collaborationrequests/sender/{sender-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 4. Get Collaboration Requests by Receiver ID
```bash
curl -X GET "$BASE_URL/api/collaborationrequests/receiver/{receiver-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 5. Create Collaboration Request
```bash
curl -X POST "$BASE_URL/api/collaborationrequests" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "receiverId": "123e4567-e89b-12d3-a456-426614174000",
    "details": "I would like to collaborate on your machine learning research project. I have extensive experience in neural networks and can contribute to the algorithm optimization aspects."
  }'
```

### 6. Review Collaboration Request
```bash
# Approve request
curl -X PUT "$BASE_URL/api/collaborationrequests/{request-id}/review" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": 1,
    "reviewerComment": "Great proposal! Looking forward to working together."
  }'

# Reject request
curl -X PUT "$BASE_URL/api/collaborationrequests/{request-id}/review" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": 2,
    "reviewerComment": "Thank you for your interest, but we are not accepting collaborations at this time."
  }'
```

**Status Values:**
- `0` = Pending
- `1` = Approved
- `2` = Rejected

### 7. Delete Collaboration Request
```bash
curl -X DELETE "$BASE_URL/api/collaborationrequests/{request-id}" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

## User Profile APIs (Me)

### 1. Get Current User Profile
```bash
curl -X GET "$BASE_URL/api/me/profile" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

### 2. Update Current User Profile
```bash
curl -X PUT "$BASE_URL/api/me/profile" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "updated@example.com",
    "role": 2
  }'
```

## Error Handling

### Common HTTP Status Codes

- `200 OK` - Request successful
- `201 Created` - Resource created successfully
- `204 No Content` - Request successful, no content returned
- `400 Bad Request` - Invalid request data
- `401 Unauthorized` - Authentication required
- `403 Forbidden` - Insufficient permissions
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Server error

### Testing Error Scenarios

#### 1. Test Unauthorized Access
```bash
# Remove Authorization header to test 401
curl -X GET "$BASE_URL/api/profiles" \
  -H "Content-Type: application/json"
```

#### 2. Test Invalid Data
```bash
# Test with invalid email format
curl -X POST "$BASE_URL/api/profiles" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "invalid-email",
    "role": 0
  }'
```

#### 3. Test Non-existent Resource
```bash
# Test with non-existent ID
curl -X GET "$BASE_URL/api/profiles/00000000-0000-0000-0000-000000000000" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

## Testing Scenarios

### Scenario 1: Complete User Workflow
```bash
# 1. Create a profile
PROFILE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/profiles" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "role": 2
  }')

# Extract profile ID from response
PROFILE_ID=$(echo $PROFILE_RESPONSE | jq -r '.id')

# 2. Get the created profile
curl -X GET "$BASE_URL/api/profiles/$PROFILE_ID" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"

# 3. Create an academic product for this expert
curl -X POST "$BASE_URL/api/academicproducts" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"achievements\": {
      \"publications\": 3,
      \"awards\": [\"Young Researcher Award\"]
    },
    \"title\": \"Test Research Project\",
    \"description\": \"A test research project for API testing\"
  }"
```

### Scenario 2: Collaboration Workflow
```bash
# 1. Create two profiles (sender and receiver)
SENDER_RESPONSE=$(curl -s -X POST "$BASE_URL/api/profiles" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "sender@example.com",
    "role": 1
  }')

RECEIVER_RESPONSE=$(curl -s -X POST "$BASE_URL/api/profiles" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "receiver@example.com",
    "role": 2
  }')

SENDER_ID=$(echo $SENDER_RESPONSE | jq -r '.id')
RECEIVER_ID=$(echo $RECEIVER_RESPONSE | jq -r '.id')

# 2. Create collaboration request
COLLAB_RESPONSE=$(curl -s -X POST "$BASE_URL/api/collaborationrequests" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d "{
    \"receiverId\": \"$RECEIVER_ID\",
    \"details\": \"I would like to collaborate on your research project.\"
  }")

COLLAB_ID=$(echo $COLLAB_RESPONSE | jq -r '.id')

# 3. Review the collaboration request
curl -X PUT "$BASE_URL/api/collaborationrequests/$COLLAB_ID/review" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "status": 1,
    "reviewerComment": "Approved! Looking forward to collaboration."
  }'
```

### Scenario 3: Bulk Operations Testing
```bash
# Create multiple academic products
for i in {1..5}; do
  curl -X POST "$BASE_URL/api/academicproducts" \
    -H "Authorization: Bearer $JWT_TOKEN" \
    -H "Content-Type: application/json" \
    -d "{
      \"achievements\": {
        \"publications\": $i,
        \"awards\": [\"Award $i\"]
      },
      \"title\": \"Research Project $i\",
      \"description\": \"Description for research project $i\"
    }"
done

# Get all academic products to verify
curl -X GET "$BASE_URL/api/academicproducts" \
  -H "Authorization: Bearer $JWT_TOKEN" \
  -H "Content-Type: application/json"
```

## Tips for Testing

1. **Use Variables**: Store IDs and tokens in variables for reuse
2. **Check Responses**: Always examine response status codes and content
3. **Test Edge Cases**: Try invalid data, empty fields, and boundary conditions
4. **Clean Up**: Delete test data after testing to keep the database clean
5. **Use jq**: Install `jq` for better JSON response parsing
6. **Logging**: Add `-v` flag to curl for verbose output when debugging

## Environment-Specific Testing

### Development Environment
```bash
BASE_URL="http://localhost:5000"
```

### Staging Environment
```bash
BASE_URL="https://your-staging-api.com"
```

### Production Environment
```bash
BASE_URL="https://your-production-api.com"
```

Remember to update the JWT token for each environment!

---

**Note**: This handbook assumes your API is running on the default port. Adjust the `BASE_URL` accordingly for your specific setup.
