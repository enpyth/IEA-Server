Before implementing unit tests, use this handbook to track basic API tests. Keep sensitive tokens out of the file; use `$ACCESS_TOKEN` placeholders.

## Test Case Table

| API | Method | Scenario | Preconditions | Expected | Status | Section |
| --- | --- | --- | --- | --- | --- | --- |
| /api/academicproducts | GET | All roles can read list | Valid token (any role) | 200 OK with items | pass | [AcademicProducts - GET](#academicproducts---get) |
| /api/academicproducts | POST | Expert can create own product | Auth as Expert | 201 Created with product | pass | [AcademicProducts - POST self](#academicproducts---post-expert-creates-own) |
| /api/academicproducts | POST | Non-expert cannot create | Auth as non-Expert | 403 Forbidden | pass | [AcademicProducts - POST forbidden](#academicproducts---post-non-expert-forbidden) |
| /api/academicproducts | POST | Expert creates for another expert | Auth as Expert | 201/403 per policy | todo | [AcademicProducts - POST others](#academicproducts---post-expert-creates-for-others) |


## /api/academicproducts

### AcademicProducts - GET {#academicproducts---get}

- All roles can read (pass)

```bash
curl -X GET \
  "http://localhost:5065/api/AcademicProducts" \
  -H "Authorization: Bearer $ACCESS_TOKEN"
```

Expected: 200 OK, JSON array of products.


### AcademicProducts - POST (expert creates own) {#academicproducts---post-expert-creates-own}

- Experts can create their own product (pass)

```bash
curl -X POST "http://localhost:5065/api/academicproducts" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
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


### AcademicProducts - POST (non-expert forbidden) {#academicproducts---post-non-expert-forbidden}

- Other roles cannot create (pass)

```bash
curl -X POST "http://localhost:5065/api/academicproducts" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
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


### AcademicProducts - POST (expert creates for others) {#academicproducts---post-expert-creates-for-others}

- Experts can create for other experts (todo - depends on policy)

Example (adjust request as needed if API supports target author):

```bash
curl -X POST "http://localhost:5065/api/academicproducts" \
  -H "Authorization: Bearer $ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "achievements": {
      "publications": 3,
      "awards": [],
      "hIndex": 8
    },
    "title": "Collaborative Work",
    "description": "Authored for another expert"
  }'
```

