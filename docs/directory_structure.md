YourProject/
│
├── Controllers/             # 控制器层 (接收请求/返回响应)
│   └── ProductsController.cs
│
├── Models/                  # 数据模型 (实体/DTO/ViewModel)
│   ├── Entities/
│   │   └── Product.cs
│   ├── DTOs/
│   │   └── ProductDto.cs
│   └── ViewModels/
│       └── ProductViewModel.cs
│
├── Services/                # 业务逻辑层 (接口 + 实现)
│   ├── Interfaces/
│   │   └── IProductService.cs
│   └── Implementations/
│       └── ProductService.cs
│
├── Repositories/            # 数据访问层 (接口 + 实现)
│   ├── Interfaces/
│   │   └── IProductRepository.cs
│   └── Implementations/
│       └── ProductRepository.cs
│
├── Data/                    # 数据上下文 / 迁移
│   ├── AppDbContext.cs
│   └── Migrations/
│       └── ... EF Core 自动生成的迁移文件
│
├── Middleware/              # 自定义中间件 (可选)
│   └── ErrorHandlingMiddleware.cs
│
├── Filters/                 # 过滤器 (可选)
│   └── ValidateModelFilter.cs
│
├── Program.cs               # 程序入口 (配置 DI, 路由, Swagger)
├── appsettings.json         # 配置文件 (数据库连接字符串, App 配置)
└── YourProject.csproj
