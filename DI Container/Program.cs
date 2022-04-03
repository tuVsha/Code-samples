using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Saritasa.OMDbScrubber.DataAccess;
using Saritasa.OMDbScrubber.Domain.Interfaces;
using Saritasa.OMDbScrubber.Infrastructure;
using Saritasa.OMDbScrubber.Infrastructure.OMDbAutoMapper;
using Saritasa.OMDbScrubber.UseCases.RequestsHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var databaseConnectionString = builder
    .Configuration["ConnectionStrings:AppDatabase"];

builder.Services
    .AddDbContextPool<AppDbContext>(
        options => options.UseSqlServer(databaseConnectionString));

var omdbKey = builder.Configuration["AppSettings:OmdbApiToken"];

var mapper = new Mapper(
    new MapperConfiguration(
        cfg => cfg
            .AddProfile<OmdbMovieMappingProfile>()
    ));

builder.Services
    .AddSingleton<IOmdbService, MovieGetter>(service => new MovieGetter(omdbKey, mapper));

builder.Services
    .AddSingleton<IMovieDbContext, AppDbContext>();

builder.Services
    .AddMediatR
        (typeof(ImdbSearchQueryhHandler).Assembly);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/imdb/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Imdb}/{action=Upload}/{id?}");

app.Run();