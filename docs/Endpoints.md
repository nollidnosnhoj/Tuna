# Endpoints

## Audios

### Get Audio

`GET /audios/{id}`

**Parameters**:

| variable | data-type | parameter-type | required |
| -------- | --------- | -------------- | -------- |
| id       | string    | path           | true     |

### Create Audio

`POST /audios`

*Requires authentication*

**Parameters**:

| variable    | data-type        | parameter-type | required | notes |
| ----------- | ---------------- | -------------- | -------- | ----- |
| uploadId    | string           | body           | true     |       |
| fileName    | string           | body           | true     |       |
| fileSize    | number           | body           | true     |       |
| duration    | number           | body           | true     |       |
| title       | string           | body           | false    |       |
| description | string           | body           | false    |       |
| isPublic    | boolean          | body           | false    |       |
| tags        | array of strings | body           | false    |       |

### Update Audio

`PUT /audios/{id}`

*Requires authentication*

**Parameters:**

| variable    | data-type        | parameter-type | required | notes |
| ----------- | ---------------- | -------------- | -------- | ----- |
| title       | string           | body           | false    |       |
| description | string           | body           | false    |       |
| isPublic    | boolean          | body           | false    |       |
| tags        | array of strings | body           | false    |       |

Note: If you do not provide a variable in the request, it will get ignored during update.

### Remove Audio

`DELETE /audios/{id}`

*Requires authentication*

**Parameters:**

| variable | data-type | parameter-type | required |
| -------- | --------- | -------------- | -------- |
| id       | string    | path           | true     |