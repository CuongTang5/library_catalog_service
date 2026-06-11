using CatalogService.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT configuration
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecret = builder.Configuration["Jwt:Secret"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("[JWT] AuthFailed: " + context.Exception);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine("[JWT] Challenge: " + context.Error + " - " + context.ErrorDescription);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("[JWT] Token validated: " + context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(opt =>
        opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    // Tự động đồng bộ sách từ Books sang InventoryBooks nếu InventoryBooks trống
    try
    {
        if (!dbContext.InventoryBooks.Any() && dbContext.Books.Any())
        {
            var books = dbContext.Books.ToList();
            var uniqueInventoryBooks = new List<CatalogService.Models.InventoryBook>();

            foreach (var book in books)
            {
                var isbn = book.Isbn?.Trim();
                var tenSach = book.TenSach?.Trim();
                if (string.IsNullOrWhiteSpace(tenSach)) continue;

                CatalogService.Models.InventoryBook? existing = null;
                if (!string.IsNullOrWhiteSpace(isbn))
                {
                    existing = uniqueInventoryBooks.FirstOrDefault(b => b.Isbn != null && b.Isbn.Trim().Equals(isbn, StringComparison.OrdinalIgnoreCase));
                }
                if (existing == null)
                {
                    existing = uniqueInventoryBooks.FirstOrDefault(b => b.TenSach.Trim().Equals(tenSach, StringComparison.OrdinalIgnoreCase));
                }

                if (existing != null)
                {
                    existing.SoLuongTonKho += book.SoLuong;
                }
                else
                {
                    uniqueInventoryBooks.Add(new CatalogService.Models.InventoryBook
                    {
                        TenSach = book.TenSach ?? string.Empty,
                        TacGia = book.TacGia ?? "Chưa rõ",
                        NhaSanXuat = book.NhaSanXuat ?? "Chưa rõ",
                        TheLoai = book.TheLoai ?? "Chưa phân loại",
                        SoLuongTonKho = book.SoLuong,
                        ImageUrl = book.ImageUrl,
                        MoTa = book.MoTa,
                        Isbn = book.Isbn,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            dbContext.InventoryBooks.AddRange(uniqueInventoryBooks);
            dbContext.SaveChanges();
            Console.WriteLine($"[Sync] Successfully synchronized {uniqueInventoryBooks.Count} unique books to InventoryBooks on startup.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[Sync] Error synchronizing books to InventoryBooks: " + ex.Message);
    }
}

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run("http://0.0.0.0:5185");
