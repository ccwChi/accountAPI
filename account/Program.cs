using account;
using account.Data;
using account.Models;
using account.Repository;
using account.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
	option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
	//option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

// �s�W repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();


builder.Services.AddAutoMapper(typeof(MappingConfig));


builder.Services.AddApiVersioning(option =>
{
	option.AssumeDefaultVersionWhenUnspecified = true;
	option.DefaultApiVersion = new ApiVersion(1, 0);
	option.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.DefaultApiVersion = new ApiVersion(1, 0);
	options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddAuthorization();
var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(x =>
{
	x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
	x.RequireHttpsMetadata = false;
	x.SaveToken = true;
	x.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
		ValidateIssuer = false,
		ValidateAudience = false
	};
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description =
		"JWT Authorization header using the Bearer scheme \r\n\r\n" +
		"Enter 'Bearer' [space] and then your token in the text input below. \r\n\r\n" +
		"Example: \"Bearer 12345abcdef\"",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Scheme = "Bearer"
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "bearer",
				Name = "Bearer",
				In = ParameterLocation.Header
			},
			new List<string>()
		}
	});
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Version = "v1",
		Title = "Account_App V1",
		Description = "API to manage Account_App\n\n密碼格式需含有大小寫英文+數字+特殊符號，例如 Abc123*\n\n代入token前面需要Bearer +token",
		Contact = new OpenApiContact
		{
			Name = "Portfolio_web",
			Url = new Uri("https://ccwchi.github.io/portfolio_web/")
		},
		TermsOfService = new Uri("https://example.com/terms"),
		License = new OpenApiLicense
		{
			Name = "Example License",
			Url = new Uri("https://example.com/license")
		}

	});
	options.SwaggerDoc("v2", new OpenApiInfo
	{
		Version = "v2",
		Title = "Account_App V2",
		Description = "API to manage Account_App",
		TermsOfService = new Uri("https://example.com/terms"),
		Contact = new OpenApiContact
		{
			Name = "Dotnetmastery",
			Url = new Uri("https://example.com/terms")
		},
		License = new OpenApiLicense
		{
			Name = "Example License",
			Url = new Uri("https://example.com/license")
		}

	});
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();

if (app.Environment.IsDevelopment())
{
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "AccountAppV1");
		options.SwaggerEndpoint("/swagger/v2/swagger.json", "AccountAppV2");
	});
}
else
{
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "AccountAppV1");
		options.SwaggerEndpoint("/swagger/v2/swagger.json", "AccountAppV2");
		options.RoutePrefix = "";
	});
}


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();



