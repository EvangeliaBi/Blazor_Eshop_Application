using MyBlazorApp.Components;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using MyBlazorApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;      // Πακέτο που εισάγει σταθερές/τύπους για cookie authentication (π.χ. CookieAuthenticationDefaults.AuthenticationScheme) και τις επιλογές cookie.
using System.Security.Claims;                   // Εισάγει τον τύπο Claim και ClaimTypes που χρησιμοποιούνται για να δημιουργηθούν claims (όνομα, ρόλος κ.λπ.) κατά το login.
using Microsoft.AspNetCore.Authentication;      // Παρέχει extension methods όπως SignInAsync/SignOutAsync και τύπους για authentication flows. Δημιουργία cookies, έλεγχος login, sign in & sign out.
using Microsoft.AspNetCore.Identity;        // Εισάγει τύπους του Identity (π.χ. PasswordHasher, PasswordVerificationResult), που χρησιμοποιούνται από το AuthService.
using MyBlazorApp.Models;


// Μέσω της δημιουργίας αντικειμένου builder γίνεται register μία υπηρεσία, όπου διαβάζονται οι ρυθμίσεις για το (appsettings.json), προετοιμάζονται τα services και στήνεται το DI container. Δημιουργεί το WebApplicationBuilder που συγκεντρώνει configuration, services και logging. Είναι το entry point για την καταχώρηση υπηρεσιών (DI) και διαμόρφωσης του pipeline πριν χτιστεί η εφαρμογή.
var builder = WebApplication.CreateBuilder(args);

// Εγγράφει τις υπηρεσίες που απαιτούνται για Razor Components / Blazor Server interactive rendering. Ενεργοποιεί την υποστήριξη για components και interactive server render mode.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Γίνεται register το DbContext.
// Μέσω του property (Services) κάνουμε register το δικό μας AppDbContext της εφαρμογής.
// Μέσω του αντικειμένου (options) γνωστοποιείται στο EntityFramework ποια βάση δεδομένων 
// θα χρησιμοποιηθεί και θα είναι της postgresql με βάση το πού βρίσκεται αυτή η βάση δεδομένων,
// τα credentials της βάσης και η σύνδεση με αυτή την βάση.
// Μέσω της GetConnectionString("DefaultConnection") θα πραγματοποιηθεί η σύνδεση και το
// AppDbContext θα είναι διαθέσιμο παντού.
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Προσθήκη του νέου Service/μίας νέας υπηρεσίας εδω που σχετίζεται με το Authentication για τον Admin.
// Κάνω την υπηρεσία αυτή register για να είναι διαθέσιμη από παντού, και να 
// είναι διαθέσιμη ίσως και για κάποιο client που δημιουργηθεί στην συνέχεια.
// Κάνουμε service για κάποια υπηρεσία που μπορούμε να καλούμε από οπουδήποτε.
// Αυτό το service είναι διαθέσιμο για όλη την εφαρμογή.
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<CartService>();  // Εδώ καταχωρείται (γίνεται register), η υπηρεσία (CartService) ως scoped.

builder.Services.AddScoped<WishlistService>();  // Εδώ γίνεται register η υπηρεσία (WishlistService).

// Ενεργοποίηση του authentication, το οποίο βασίζεται στα cookies και γνωστοποιείται στο ASP.NetCore κάνοντας register αυτή την υπηρεσία στο DI Container. Το CookieAuthenticationDefaults.AuthenticationScheme είναι ένα string, συγκεκριμένα το “Cookies”.
// Σε αυτό το σημείο ο χρήστης δεν μπορεί να επισκεφθεί την σελίδα (/admin/products), χωρίς να έχει κάνει πρώτα login.
// Αν ένας admin κάνει επιτυχημένα το login, θα δημιουργηθεί το Cookie στο Response και θα το στέλνει στον server σε επόμενα requests. Το cookie αυτό θα αποτελεί το session cookie επομένως θα χαθεί by default αν ο χρήστης κλείσει τον browser (όχι το tab που τρέχει η εφαρμογή).
// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
// {
//     // Εδώ καθορίζεται πώς θα λειτουργούν τα cookies μέσω του object options.
//     options.LoginPath = "/admin/login";     // Όταν ο χρήστης προσπαθεί να έχει πρόσβαση στην προστατευμένη σελίδα /admin/products (το UseAuthorization «βλέπει» το @Attribute [Authorize]), το UseAuthentication() αναλαμβάνει και κάνει έλεγχο για το αν ο χρήστης αυτός είναι logged in κι έτσι αν το request τους έχει το κατάλληλο Cookie που δημιουργείται και κρυπτογραφείται από τον server μας κι εφόσον ο χρήστης δεν είναι logged in, ανακατευθύνεται στο /admin/login. Σε αυτή τη φάση θα προσπαθήσουμε να γράψουμε στο response προς το χρήστη ένα valid cookie.
//     options.AccessDeniedPath = "/admin/login";      // Όταν ο χρήστης ΕΙΝΑΙ logged in (έχει δηλαδή ένα valid cookie), αλλά δεν έχει τα κατάλληλα permissions για να έχει πρόσβαση στη σελίδα.

//     options.Cookie.Expiration = null;   // Μετατρέπει το cookie σε session cookie.
//     options.ExpireTimeSpan = TimeSpan.FromMinutes(20);     // Μετά από 20 λεπτά ο χρήστης γίνεται log out αν παραμέινει ανενεργός για όλο αυτό το διάστημα και θα πρέπει να κάνει ξανά login.
//     // Εάν υπάρχει δραστηριότητα σε κάποιο tab που τρέχει η εφαρμογή από τον χρήστη, τότε πραγματοποιείται ανανέωση του χρονικού ορίου των 20 λεπτών.
//     options.SlidingExpiration = true;
// });


// Παρακάτω η εφαρμογή υποστηρίζει authentication, καθώς το default authentication scheme είναι το cookies. Αυτό σημαίνει ότι όταν το framework χρειαστεί να ελέγξει authentication χωρίς να του δοθεί ρητά scheme, θα χρησιμοποιήσει τον cookie handler. Αυτό έχει ως αποτέλεσμα να καθοριστεί ο βασικός μηχανισμός της εφαρμογής, καθώς επιτρέπει την προσθήκη cookie handler στη συνέχεια.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>       // Σε αυτό το σημείο προστίθεται και παραμετροποιείται το cookie authentication handler. Το options είναι αντικείμενο ρυθμίσεων που επιτρέπει να γίνει ο ορισμός των events, cookie properties και πολιτικές λήξης. Στην ουσία εδώ καθορίζεται πώς θα συμπεριφέρεται το cookie, δηλαδή πώς το σύστημα θα αντιδρά σε περιπτώσεις μη authenticated ή μη εξουσιοδοτημένων αιτημάτων.
    {
        options.Events.OnRedirectToLogin = context =>       // Ορίζει handler για το event που ενεργοποιείται όταν ο χρήστης πρέπει να ανακατευθυνθεί στο login page (π.χ. όταν προσπαθεί να επισκεφθεί σελίδα με [Authorize] χωρίς έγκυρο cookie).
        {
            // Ανάλογα με τη σελίδα, ανακατεύθυνση στον σωστό login.
            if (context.Request.Path.StartsWithSegments("/client"))     // Ελέγχει το path του τρέχοντος αιτήματος για να δει αν ανήκει στην περιοχή /client. Η μέθοδος StartsWithSegments συγκρίνει το αρχικό τμήμα του URI. Χρησιμοποιείται για να αποφανθεί σε ποιο login page θα ανακατευθυνθεί ο χρήστης, ανάλογα με το τμήμα της εφαρμογής που προσπαθεί να επισκεφθεί.
                context.Response.Redirect("/client/login");         // Αν το request είναι για client area, στέλνει HTTP redirect προς /client/login, καθώς η ανακατεύθυνση γίνεται στο response του server και ο browser θα ακολουθήσει το redirect και θα εμφανίσει τη σελίδα login.
            else
                context.Response.Redirect("/admin/login");      // Αν το request δεν είναι για /client, τότε ανακατευθύνει στο admin login, το οποίο επιτρέπει ξεχωριστά login flows για admin και client περιοχές.
            return Task.CompletedTask;      // Εδώ στην ουσία επιστρέφεται το task.
        };

        options.Events.OnRedirectToAccessDenied = context =>        // Εδώ ορίζεται handler για το event που ενεργοποιείται όταν ο χρήστης είναι authenticated αλλά δεν έχει τα απαιτούμενα δικαιώματα (π.χ. role), καθορίζοντας πού θα πάει ο χρήστης όταν έχει cookie αλλά δεν έχει authorization.
        {
            context.Response.Redirect("/client/login");     // Σε περίπτωση access denied, εδώ γίνεται redirect στο /client/login.
            return Task.CompletedTask;
        };

        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);      // Εδώ καθορίζεται η διάρκεια ζωής του authentication cookie, δηλαδή εδώ είναι τα 20 λεπτά. Μετά από αυτό το διάστημα το cookie θεωρείται ληγμένο.
        options.SlidingExpiration = true;       // Κάθε φορά που ο χρήστης κάνει ενεργό αίτημα πριν λήξει το cookie, το expiration μετατοπίζεται προς τα εμπρός, διατηρώντας τον χρήστη logged in όσο υπάρχει δραστηριότητα. Έτσι αποφεύγονται τα συχνά re‑login όταν ο χρήστης είναι ενεργός, ενώ διατηρεί timeout για αδρανείς χρήστες.
    });


builder.Services.AddAuthorization(options =>   // Καταχωρεί και ρυθμίζει την υπηρεσία εξουσιοδότησης στο DI container. Το options είναι ο χώρος όπου ορίζονται named policies και άλλοι κανόνες που θα χρησιμοποιήσει το framework για να αποφασίσει αν ένας χρήστης έχει πρόσβαση.
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));      // Εδώ δημιουργείται μια πολιτική με όνομα AdminOnly. Η πολιτική αυτή περιέχει έναν requirement που απαιτεί το claim ρόλου "Admin" στο ClaimsPrincipal του χρήστη. Όταν εφαρμόζεται η πολιτική (π.χ. [Authorize(Policy = "AdminOnly")]), το authorization system ελέγχει αν ο authenticated χρήστης έχει το claim role: Admin και επιτρέπει ή απορρίπτει την πρόσβαση ανάλογα.
    options.AddPolicy("ClientOnly", policy => policy.RequireRole("Client"));    // Δημιουργεί την πολιτική ClientOnly που απαιτεί το role "Client".
});


// Προσθήκη session support που σχετίζεται με το authentication. Προσθέτει in‑memory distributed cache provider που χρησιμοποιείται από το session middleware. 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // Εισαγωγή session για τον χρήστη που όταν κλείνει τον browser σταματά μόνο του.
    options.IdleTimeout = TimeSpan.FromMinutes(30);    // Αν μείνει ανενεργή η σύνδεση του χρήστη για 30 λεπτά τότε κλείνει μόνο του, δηλαδή λήγει το session και ο χρήστης θα πρέπει να κάνει ξανά login.
    // Εισαγωγή των cookies στην εφαρμογή.
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});    


var app = builder.Build();     // Αρχικοποίηση της εφαρμογής blazor. Στην ουσία εδώ χτίζεται το WebApplication από το builder και μετά από αυτό ρυθμίζεται το middleware και τα endpoints.

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())       // Σε αυτό το σημείο ενεργοποιείται ένας global error handler που ανακατευθύνει σε /Error και HSTS για αυστηρή χρήση HTTPS.
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();     // Ανακατευθύνει HTTP αιτήματα σε HTTPS.


app.UseAntiforgery();       // Ενεργοποιεί antiforgery protection, όπου χρειάζεται ενσωμάτωση στα forms/requests που κάνουν POST.

// Με το UseAuthentication και το UseAuthorization γίνεται ο έλεγχος για τα cookies σε κάθε request που θα κάνει ο χρήστης.
app.UseAuthentication();    // Ενεργοποιεί authentication middleware που διαβάζει cookie και δημιουργεί HttpContext.User.
app.UseAuthorization();     // Ενεργοποιεί authorization middleware που ελέγχει policies/roles.

app.UseSession();       // Ενεργοποιεί session middleware για HttpContext.Session.

app.MapStaticAssets();          // Χαρτογραφεί static assets (CSS/JS/images).
app.MapRazorComponents<App>()       // Εδώ χαρτογραφείται το root Razor Components app (App) και ενεργοποιεί interactive server render mode για το Blazor Server.
    .AddInteractiveServerRenderMode();


// Η διαδικασία του login μεταφέρεται στο API /auth/login από την HandleLogin που την είχαμε στο AdminLogin.razor καθώς πλέον θέλουμε να δουλέψουμε με Cookies και όπως εξηγήσαμε σε ένα Razor component αυτό δεν είναι δυνατό γιατί όταν αυτό προβάλλεται έχει ήδη σταλεί το response στον browser του client.
// Παρακάτω δημιουργείται ένα HTTP GET endpoint στο URL /auth/login, το οποίο δέχεται.
// ως παραμέτρους:HttpContext ctx → πληροφορίες για το HTTP request/response.
// AuthService auth → η υπηρεσία για έλεγχο των credentials του admin.
// username και password → τα credentials που στέλνει ο χρήστης.
app.MapGet("/auth/login", async (HttpContext ctx, AuthService auth, string username, string password) =>
{
    // Εδώ ελέγχεται η περίπτωση που ο χρήστης δεν έχει εισάγει ούτε το username ούτε το password (άδεια/κενά πεδία).
    if(string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        // Εάν ο χρήστης δεν έχει συμπληρώσει κάποιο πεδίο κάνει Redirect πίσω στην σελίδα του login, 
        // το οποίο χρησιμοποιείται στο AdminLogin.razor για να εμφανίσει το κατάλληλο μήνυμα στον χρήστη.
        return Results.Redirect("/admin/login?error=empty");
    }

    // Εδώ καλείται η μέθοδος ValidateAdmin της υπηρεσίας AuthService για να ελέγξει για την
    // ορθότητα των credentials.
    var admin = await auth.ValidateAdmin(username, password);
    // Εάν επιστρέψει null, σημαίνει ότι το username ή το password είναι λάθος.
    if(admin == null)
    {
        // Λάθος username ή password => γίνεται Redirect πίσω στο login με 
        // query string error=invalid για να εμφανιστεί το αντίστοιχο μήνυμα λάθους.
        return Results.Redirect("/admin/login?error=invalid");
    }

    // Αφού ο χρήστης συμπληρώσει το username και το password στην φόρμα του AdminLogin.razor, το request αυτό προωθείται στο /auth/login και εφόσον το login του admin γίνει επιτυχημένα δημιουργούμε τα Claims (πληροφορίες σχετικές με το χρήστη).
    // Δημιουργία claims και cookie.
    // Δημιουργία λίστας από claims που περιγράφουν τον χρήστη:
    // ClaimTypes.Name → το όνομα του admin.
    // ClaimTypes.Role → ο ρόλος του χρήστη, εδώ "Admin".
    // Τα claims χρησιμοποιούνται από το ASP.NET Core για authentication και authorization ([Authorize(Roles = "Admin")]).
    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, admin.AdminUserName),
        new Claim(ClaimTypes.Role, "Admin")
    };

    // Μέσω της κλάσης ClaimsIdentity δημιουργούμε με βάση τα claims ένα αντικείμενο αντίστοιχου τύπου που αποτελεί την ταυτότητα του χρήστη (Identity).
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    // Το ClaimsPrincipal περιέχει αυτό το identity και αντιπροσωπεύει τον authenticated χρήστη.
    var principal = new ClaimsPrincipal(identity);

    // Εδώ δημιουργείται το cookie αφού γίνει encryption στο principal (claims, identity κτλ.) και γράφεται το cookie στο HTTP response και στέλνεται στον browser του χρήστη όπου αποθηκεύεται και θα στέλνεται σε κάθε επόμενο request. Εδώ πραγματοποιείται το login του χρήστη δημιουργώντας το authentication cookie που θα χρησιμοποιείται σε κάθε επόμενο request.
    await ctx.SignInAsync(principal);

    // Αν όλα είναι σωστά (credentials έγκυρα), γίνεται redirect στη σελίδα διαχείρισης προϊόντων /admin/products, όπου ο browser του client θα αποστείλει το Cookie.
    return Results.Redirect("/admin/products");
});


// Παρακάτω ορίζεται ένα HTTP GET endpoint στο /client/auth/login που δέχεται username και password, επικυρώνει τα credentials μέσω της υπηρεσίας AuthService, δημιουργεί τα απαραίτητα claims για τον πελάτη, κάνει sign‑in με cookie authentication και τελικά ανακατευθύνει τον χρήστη στη σελίδα προϊόντων.
// Η χρήση του GET σημαίνει ότι τα credentials θα περάσουν στο URL ως query string αν ο client τα στείλει έτσι.
app.MapGet("/client/auth/login",
async (HttpContext ctx, AuthService auth, string username, string password) =>      // Σε αυτό το σημείο χαρτογραφείται ένα HTTP GET endpoint στη διαδρομή /client/auth/login. Ο handler είναι ασύγχρονος ως lambda expression που δέχεται: HttpContext ctx για πρόσβαση σε request/response και σε μεθόδους authentication, το AuthService auth που γίνεται inject από το DI container και περιέχει τη λογική επαλήθευσης των credentials, τα string username και string password που δεσμεύονται αυτόματα από το query string του αιτήματος.
{
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))     // Σε αυτό το σημείο ελέγχεται εάν τα πεδία username/password είναι null ή κενά, αοτρέποντας προσπάθειες login με κενά πεδία.
        return Results.Redirect("/client/login?error=empty");       // Εάν το username ή το password (ή και τα 2 πεδία είναι κενά), τότε ο handler επιστρέφει redirect στη σελίδα login με query error=empty, επιτρέποντας στο UI να εμφανίσει το κατάλληλο μήνυμα σφάλματος στον χρήστη.

    var customer = await auth.ValidateCustomer(username, password);     // Σε αυτό το σημείο καλείται η ασύγχρονη μέθοδος ValidateCustomer της AuthService για να βρει τον χρήστη με το δοθέν username και να επαληθεύσει το password (μέσω του PasswordHasher.VerifyHashedPassword). Η μέθοδος επιστρέφει το αντικείμενο customer αν τα credentials είναι σωστά αλλιώς επιστρέφει null αν τα credentials είναι λάθος. Μέσω της χρήσης του await δεν μπλοκάρεται το thread κατά την πρόσβαση στη βάση.

    if (customer == null)           // Σε αυτό το σημείο εάν η επαλήθευση αποτύχει, γίνεται redirect στο login με error=invalid. Αυτό σημαίνει ότι είτε το username δεν υπάρχει είτε ότι το password είναι λάθος.
        return Results.Redirect("/client/login?error=invalid");

    var claims = new List<Claim>    // Εδώ δημιουργείται μία λίστα claims που περιγράφουν τον authenticated χρήστη.
    {
        new Claim(ClaimTypes.Name, customer.Username),     // Σε αυτό το σημείο αποθηκεύεται το όνομα χρήστη ώστε να είναι διαθέσιμο ως User.Identity.Name.
        new Claim(ClaimTypes.Role, "Client")            // Εδώ πραγματοποιείται η προσθήκη του Role(client), ώστε οι πολιτικές/attributes [Authorize(Roles = "Client")] ή policies να λειτουργούν.
    };

    // Παρακάτω δημιουργείται ένα ClaimsIdentity που περιέχει τα claims και δηλώνει το authentication scheme "Cookies". Το identity είναι το αντικείμενο που περιγράφει την ταυτότητα του χρήστη, καθώς το scheme πρέπει να ταιριάζει με το scheme που ρυθμίστηκε στο AddAuthentication(...).AddCookie(...).
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

    var principal = new ClaimsPrincipal(identity);    // Σε αυτό το σημείο πακετάρεται το identity σε ClaimsPrincipal, που είναι το αντικείμενο που το ASP.NET Core τοποθετεί στο HttpContext.User όταν ο χρήστης είναι authenticated.

    // Πιο συγκεκριμένα το cookie πρέπει να είναι HttpOnly και Secure σε production. Η διάρκεια και το sliding expiration καθορίζονται στις ρυθμίσεις του cookie handler, ενώ η χρήση του SignInAsync χωρίς επιπλέον AuthenticationProperties σημαίνει ότι χρησιμοποιούνται οι default ρυθμίσεις, δηλαδή (συγκεκριμένο expire time).
    await ctx.SignInAsync(principal);     // Σε αυτό το σημείο καλείται το authentication subsystem για να δημιουργήσει το authentication cookie. Το principal κρυπτογραφείται και το cookie γράφεται στο HTTP response. Από αυτό το σημείο και μετά ο browser θα στέλνει το cookie σε επόμενα requests και το middleware UseAuthentication θα αναγνωρίζει τον χρήστη.

    return Results.Redirect("/client/products");    // Σε αυτό το σημείο μετά το επιτυχημένο sign‑in, ο handler επιστρέφει redirect στη σελίδα προϊόντων του client. Ο browser θα ακολουθήσει το redirect και θα στείλει το νέο cookie σε μελλοντικά αιτήματα προς προστατευμένες διαδρομές.
});

app.Run();  // Κι αφού έχει αρχικοποιηθεί η εφαρμογή, σε αυτό το σημείο τρέχει η εφαρμογή.
