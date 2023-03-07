using EMP.Service.Implements;
using EMP.Service.Interfaces;
using Microsoft.AspNetCore.Server.IISIntegration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Negotiate.NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddControllers();
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEqmAFortyCompareService, EqmAFortyCompareService>();
builder.Services.AddScoped<IExcelService, ExcelService>();




/*CORS*/
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("allowCors",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    //套用CORS
    app.UseCors("allowCors");
}

app.UseSpaStaticFiles();

/*授權驗證*/
app.UseAuthorization();//Middleware啟用授權功能，要搭配[Authorize]，沒加上[Authorize]還是不會進行授權驗證


//不管有無[Authenrization]，都會進行授權驗證
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization();
});

app.MapControllers();

app.UseSpa(spa => { });
app.Run();

