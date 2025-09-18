# JWT认证测试指南

## 🔍 问题诊断步骤

### 1. **检查JWT Token格式**
确保你的Authorization header格式正确：
```
Authorization: Bearer <your-jwt-token>
```

### 2. **验证JWT Token内容**
使用 [jwt.io](https://jwt.io) 解码你的JWT token，检查：
- `iss` (issuer): 应该是 `https://crdohldywqramzaqitqd.supabase.co`
- `sub` (subject): 应该是用户ID
- `exp` (expiration): 确保token没有过期

### 3. **测试认证流程**

#### 步骤1: 测试基本认证
```bash
# 测试不需要admin权限的端点
curl -H "Authorization: Bearer <your-jwt-token>" \
     https://localhost:5001/api/me/profile
```

#### 步骤2: 测试admin权限端点
```bash
# 测试需要admin权限的端点
curl -H "Authorization: Bearer <your-jwt-token>" \
     https://localhost:5001/api/profiles
```

### 4. **检查用户角色**
确保你的用户在profiles表中的role是 `enterprise`（我设置为admin角色）。

## 🛠️ 临时解决方案

如果你只是想快速测试API，可以临时移除admin权限要求：

### 临时移除admin权限
在 `Controllers/ProfilesController.cs` 中，将：
```csharp
[Authorize(Policy = "AdminPolicy")]
```
改为：
```csharp
[Authorize] // 只需要认证，不需要特定角色
```

## 🔧 调试步骤

### 1. **启用详细日志**
在 `appsettings.Development.json` 中添加：
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

### 2. **检查JWT验证**
在 `Program.cs` 中添加JWT调试信息：
```csharp
options.Events = new JwtBearerEvents
{
    OnAuthenticationFailed = context =>
    {
        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
        return Task.CompletedTask;
    },
    OnTokenValidated = context =>
    {
        Console.WriteLine($"Token validated for user: {context.Principal?.Identity?.Name}");
        return Task.CompletedTask;
    }
};
```

## 📝 常见问题

### 问题1: 401 Unauthorized
- 检查JWT token是否有效
- 检查token是否过期
- 检查Authorization header格式

### 问题2: 403 Forbidden
- 检查用户是否有正确的角色
- 检查profiles表中是否存在该用户记录

### 问题3: JWT验证失败
- 检查Supabase URL配置
- 检查JWT签名是否正确

## 🚀 快速测试

运行应用后，访问 Swagger UI (`/swagger`)：
1. 点击 "Authorize" 按钮
2. 输入 `Bearer <your-jwt-token>`
3. 测试 `/api/me/profile` 端点
4. 测试 `/api/profiles` 端点

如果还有问题，请提供：
- 完整的错误信息
- JWT token内容（解码后的）
- 用户ID和角色信息
