// Controllers/AuthController.cs in SecureBankWeb

using Microsoft.AspNetCore.Mvc; // Provides core MVC functionalities like Controller, IActionResult
using SecureBankWeb.Models;    // Needed to use LoginRequest, SignUpRequest, AuthResponse DTOs
using System.Net.Http;         // For HttpClientFactory and HTTP calls
using System.Text;             // For Encoding.UTF8
using System.Text.Json;        // For JsonSerializer (for converting C# objects to JSON and vice-versa)
using System.Threading.Tasks;  // For asynchronous operations (Task, await)

namespace SecureBankWeb.Controllers // <--- Ensure this namespace matches your project name!
{
    public class AuthController : Controller // Defines this class as an MVC Controller
    {
        // This field will hold the HttpClientFactory instance, which is injected by ASP.NET Core
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor: ASP.NET Core automatically provides an IHttpClientFactory when AuthController is created
        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // GET Action for Login page
        // When a user navigates to /Auth/Login, this method is called.
        public IActionResult Login()
        {
            return View(); // Tells the controller to render the Login.cshtml view (we'll create this next)
        }

        // POST Action for Login form submission
        // When a user submits the login form to /Auth/Login via POST
        [HttpPost] // Marks this method to respond only to HTTP POST requests
        public async Task<IActionResult> Login(LoginRequest model) // 'model' automatically populated from form
        {
            // 1. Server-side Validation: Checks if the data submitted from the form
            //    conforms to the [Required], [EmailAddress], etc. attributes in LoginRequest.cs
            if (!ModelState.IsValid)
            {
                return View(model); // If validation fails, show the form again with error messages
            }

            // 2. Get HttpClient: Creates a named HttpClient instance configured in Program.cs
            var client = _httpClientFactory.CreateClient("SecureBankApi");

            // 3. Prepare Request Content:
            //    a. JsonSerializer.Serialize(model): Converts the LoginRequest C# object into a JSON string.
            //    b. new StringContent(...): Wraps the JSON string into content that can be sent in an HTTP request.
            //    c. Encoding.UTF8: Specifies character encoding.
            //    d. "application/json": Sets the Content-Type header, telling the API that the body is JSON.
            var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");

            // 4. Send POST Request to API:
            //    Asynchronously sends the POST request to the API's login endpoint.
            //    The complete URL will be ApiSettings:BaseUrl + "auth/login" (e.g., https://localhost:7xxx/auth/login)
            var response = await client.PostAsync("api/Auth/login", content);
            // 5. Handle API Response:
            if (!response.IsSuccessStatusCode) // Check if the API returned a success status code (2xx)
            {
                // If not successful, add a general error message to ModelState
                // Attempt to read the error message from the API's response if available
                var errorResponse = await response.Content.ReadAsStringAsync();
                var authError = JsonSerializer.Deserialize<AuthResponse>(errorResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ModelState.AddModelError(string.Empty, authError?.Message ?? "Login failed. Please check your credentials.");
                return View(model); // Show the form again with the error
            }

            // 6. Process Successful Response:
            //    a. Read the JSON string from the API's response.
            var json = await response.Content.ReadAsStringAsync();
            //    b. JsonSerializer.Deserialize: Converts the JSON string back into an AuthResponse C# object.
            //    c. new JsonSerializerOptions { PropertyNameCaseInsensitive = true }: Allows JSON properties (e.g., "token")
            //       to match C# properties (e.g., Token) regardless of case.
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // 7. Store Token/Username Temporarily:
            //    TempData stores data that is available for the current request and the subsequent request (after a redirect).
            //    This is where we store the JWT token and username received from the API.
            TempData["Token"] = authResponse?.Token;
            TempData["Username"] = authResponse?.Username;

            // 8. Redirect: After successful login, redirect the user to the Welcome action/page.
            return RedirectToAction("Welcome");
        }

        // GET Action for Register page
        // When a user navigates to /Auth/Register, this method is called.
        public IActionResult Register()
        {
            return View(); // Renders the Register.cshtml view (we'll create this next)
        }

        // POST Action for Register form submission
        // When a user submits the register form to /Auth/Register via POST
        [HttpPost]
        public async Task<IActionResult> Register(SignUpRequest model) // 'model' automatically populated from form
        {
            // 1. Server-side Validation: Same as Login, checks validation attributes on SignUpRequest
            if (!ModelState.IsValid)
            {
                return View(model); // If validation fails, show the form again with errors
            }

            // 2. Get HttpClient: Creates the named HttpClient instance.
            var client = _httpClientFactory.CreateClient("SecureBankApi");

            // 3. Prepare API User Object:
            //    This creates an anonymous object. You could directly serialize 'model' too,
            //    but this step might be used if your API's signup DTO has slightly different property names
            //    than your MVC's SignUpRequest. Here, it explicitly maps to what your API expects.
            var apiUser = new
            {
                Username = model.Username,
                PasswordHash = model.Password, // IMPORTANT: Ensure your API's SignUpRequest DTO has a 'Password' property, not 'PasswordHash'
                Email = model.Email,
                MobileNumber = model.MobileNumber,
                AadharNumber = model.AadharNumber,
                Gender = model.Gender,
                AccountType = model.AccountType,
                Branch = model.Branch
            };

            // 4. Prepare Request Content (JSON):
            var content = new StringContent(JsonSerializer.Serialize(apiUser), Encoding.UTF8, "application/json");

            // 5. Send POST Request to API:
            //    Sends the POST request to the API's signup endpoint.
            var response = await client.PostAsync("api/Auth/signup", content);

            // 6. Handle API Response:
            if (!response.IsSuccessStatusCode)
            {
                // If not successful, add a general error message
                var errorResponse = await response.Content.ReadAsStringAsync();
                var authError = JsonSerializer.Deserialize<AuthResponse>(errorResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ModelState.AddModelError(string.Empty, authError?.Message ?? "Registration failed. Please check input or try again.");
                return View(model); // Show the form again with the error
            }

            // 7. Redirect: If registration is successful, redirect the user to the Login page.
            return RedirectToAction("Login");
        }

        // GET Action for Welcome page
        // This is the page users see after successful login.
        public IActionResult Welcome()
        {
            // Retrieve the Username from TempData (which was set after login)
            ViewBag.Username = TempData["Username"]; // ViewBag passes data from controller to view
            return View(); // Renders the Welcome.cshtml view (we'll create this next)
        }
    }
}