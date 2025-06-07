// Program.cs in SecureBankWeb

using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- NEW: Configure HttpClient for your API calls ---
builder.Services.AddHttpClient("SecureBankApi", client =>
{
    // Get the API base URL from appsettings.json
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5031/"); // Provide a default or ensure appsettings.json is loaded
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
// --- End NEW HttpClient ---

// --- NEW: Configure Session State for storing data like login tokens ---
builder.Services.AddDistributedMemoryCache(); // This enables session state to store data in the server's memory
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set how long a session can be inactive before expiring (e.g., 30 minutes)
    options.Cookie.HttpOnly = true; // Makes the session cookie inaccessible to client-side scripts for security
    options.Cookie.IsEssential = true; // Marks the session cookie as necessary for the app to function (so it's not blocked by GDPR/cookie consent by default)
});
// --- End NEW Session ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection(); // Standard redirect to HTTPS
app.UseStaticFiles(); // Serves static files like CSS, JS, images

app.UseRouting(); // Enables routing based on URL

app.UseAuthorization(); // Enables authorization checks

// --- NEW: Enable Session Middleware ---
// IMPORTANT: This must be placed AFTER UseRouting and BEFORE MapControllerRoute
app.UseSession();
// --- End NEW Session Middleware ---

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();