using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
// Προσθήκη του πακέτου(Microsoft.AspNetCore.Identity) για μετατροπή των κωδικών σε hashed passwords,
// καθώς οι κωδικοί δεν αποθηκεύονται ποτέ ως plain text, χρησιμοποιώντας το PasswordHasher<T>,
// που προσφέρει το συγκεκριμένο πακέτο.
using Microsoft.AspNetCore.Identity;   
using MyBlazorApp.Data;
using MyBlazorApp.Models;

namespace MyBlazorApp.Services
{
    public class AuthService
    {
        // Πρόκειται για ένα πεδίο (αφού το readonly πηγαίνει μόνο σε πεδία),
        // όπου το AppDbContext μπορώ να το καλώ από οπουδήποτε από την στιγμή
        // που το έχω καταχωρημένο ως service.
        private readonly AppDbContext _context;

        // Παρακάτω δημιουργείται ένα private readonly πεδίο τύπου PasswordHasher<Admin>,
        // οποίο αποτελεί ένα γενικό εργαλείο της ASP.NET Core για ασφαλές hashing password, ενώ
        // ταυτόχρονα λόγω του readonly πεδίου, η αρχικοποίηση αυτού γίνεται μόνο μέσα στον 
        // constructor της κλάσης και δεν μπορεί αν αλλάξει αργότερα.
        // Το πεδίο αυτό χρησιμοποιείται για ασφαλή μετατροπή passwords σε hash μορφή
        // και για επαλήθευση του password που εισάγει ο χρήστης.
        // Το readonly εξασφαλίζει ότι το hasher δεν μπορεί να αντικατασταθεί 
        // μετά τη δημιουργία του AuthService.
        // Δηλώνονται 2 readonly πεδία για hashing και επαλήθευση των passwords, παρέχοντας ασφαλή hash και verify, χρησιμοποιώντας PBKDF2, salt, iterations.
        private readonly PasswordHasher<Admin> _passwordHasher;     // για admin passwords.
        private readonly PasswordHasher<Customer> _customerPasswordHasher;     // για customer passwords.

        // Δημιουργία του κατασκευαστή που δέχεται ως παράμετρο εισόδου ένα αντικείμενο τύπου AppDbContext.
        // Το AppDbContext είναι όλος ο τρόπος επικοινωνίας με την βάση, το ονομάζω
        // context και στην ουσία το αρχικοποιώ. Αυτό είναι το αντικείμενο που συνδέεται 
        // με τη βάση δεδομένων.
        // Το αποθηκεύουμε στο _context για να μπορούμε να κάνουμε queries 
        // (π.χ. να πάρουμε τον admin από τη βάση).
        public AuthService(AppDbContext context)
        {
            _context = context;   // ενημέρωση του πεδίου με την παράμετρο που δέχεται από την είσοδο.
            // Constructor του AuthService:
            // 1) Λαμβάνει το AppDbContext για να επικοινωνεί με τη βάση και καλείται 
            // κάθε φορά που δημιουργείται ένα instance της υπηρεσίας AuthService.
            // 2) Δημιουργεί ένα instance του PasswordHasher<Admin> για ασφαλές hashing 
            // και verification.
            // Το PasswordHasher δεν χρειάζεται περαιτέρω ρύθμιση, PasswordHasher χρησιμοποιεί: 
            // PBKDF2 για hashing, Random salt για κάθε password, Iterations για ασφάλεια.
            // Αυτό σημαίνει ότι η υπηρεσία μπορεί τώρα να ελέγξει τα credentials, δηλαδή εάν ο κωδικός
            // που εισάγει ο admin ταιριάζει με το hash password.
            // χωρίς να αποθηκεύει ποτέ το password σε plain text.
            // Εδώ στην ουσία δημιουργούνται instances των PasswordHasher για admin και customer, όπου αυτά τα αντικείμενα θα χρησιμοποιηθούν για hashing και verification των passwords.
            _passwordHasher = new PasswordHasher<Admin>();
            _customerPasswordHasher = new PasswordHasher<Customer>();
        }

        // Παρακάτω ο admin μού δίνει ένα Username κι ένα Password και θα τα 
        // χρησιμοποιήσω, όπου μέσω κλήσης της μεθόδου ValidateAdmin, περνάω
        // εντός της μεθόδου τα 2 ορίσματα που μού έδωσε ο χρήστης.
        public async Task<Admin?> ValidateAdmin(string username, string password)
        {
            // Μέσω της σύνδεσης με την βάση δεδομένων κι αφού για το πεδίο _context,
            // καλέσαμε την ιδιότητα Admins του DbSet, μπορούμε να δημιουργήσουμε 
            // queries μέσω έτοιμων συναρτήσεων όπως είναι η FirstOrDefaultAsync,
            // η οποία δέχεται ως όρισμα ένα lamda expression.
            // Αυτή η συνάρτηση παίρνει το πρώτο row μέσα από τον πίνακα Admins,
            // όπου το (a) αναπαριστά το τρέχον αντικείμενο στον πίνακα Admin, στην
            // ουσία είναι το τρέχον row αφού παίρνει όλες τις γραμμές σειριακά.
            // Η μέθοδος έχει δηλωθεί ως async γιατί δέχεται μία λογική όπως 
            // αυτή εδώ (a.AdminUserName == AdminUserName), όπου θα γίνει ο έλεγχος αυτής 
            // της συνθήκης κι αν βγει true η συνθήκη, τότε σού επιστρέφει αυτό το (a), 
            // δηλαδή το τρέχον row του πίνακα. Στην ουσία θα μας επιστρέψει το πρώτο 
            // αντικείμενο που θα δει ότι το username είναι ίδιο στην βάση δεδομένων με αυτό
            // που πέρασε ο χρήστης (ως όρισμα).
            // Το a είναι ένα Lambda expression που αντιπροσωπεύει κάθε γραμμή (Admin) στον πίνακα.
            // Στόχος είναι η εύρεση του χρήστη με το συγκεκριμένο username.

            // Η χρήση του await παρακάτω, συμβαίνει στην περίπτωση όταν έχω μία ασύγχρονη
            // ενέργεια και δεν μπλοκάρεται το process, δηλαδή η ροή του προγράμματος.
            // Λόγω του ότι έχουμε ένα συγκεκριμένο process που δεν θέλουμε να διακοπεί,
            // όση ώρα και να πάρει λόγω χρήσης του (await), δεν προχωράει παρακάτω στις
            // επόμενες εντολές, που σημαίνει ότι μπορούν να γίνουν κάποιες άλλες ενέργειες,
            // είτε να κληθεί μία άλλη συνάρτηση και όταν τελειώσει η ασύγχρονη ενέργεια,
            // τότε θα ειδοποιηθεί ένας EventListener(ο οποίος κοιτάζει ποια ενέργεια έχει τελειώσει),
            // και θα επιστρέψει υλοποιώντας τις επόμενες εντολές κώδικα/τις επόμενες ενέργειες.
            // Από την στιγμή που έχει δηλωθεί το await εντός μίας function τότε
            // αναγκαστικά η συνάρτηση η παραπάνω δηλώνεται ως async, διότι εντός
            // αυτού του function υπάρχουν 1 ή περισσότερες ασύγχρονες ενέργειες,
            // όπου γίνεται ασύγχρονη ενέργεια για να μην μπλοκαριστεί το process.
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminUserName == username);
            if(admin == null) return null;  // Στην περίπτωση που δεν βρεθεί ο admin, τότε επιστρέφεται null. Στην ουσία επιστρέφεται 1 task, το οποίο πρόκειται για ένα είδος αντικειμένου, το οποίο μπορεί να είναι nullable, λόγω του ερωτηματικού(Task<Admin?>).
            var hasher = new PasswordHasher<Admin>();
            // Παρακάτω Χρησιμοποιούμε το PasswordHasher που δημιουργήσαμε στο constructor (_passwordHasher) 
            // για να ελέγξουμε το password. 
            // Μέσω της μεθόδου VerifyHashedPassword: 
            // ο admin → το αντικείμενο χρήστη (χρησιμοποιείται για γενικό typing και context)
            // admin.AdminPasswordHash → το hash του password που είναι αποθηκευμένο στη βάση
            // password → το plain text password που εισήγαγε ο χρήστης.
            // Όλη αυτή η μέθοδος ελέγχει αν το plain password ταιριάζει με το αποθηκευμένο hash password.
            // Χρησιμοποιεί PBKDF2, salt και iterations για ασφαλή σύγκριση, καθώς επιστρέφει 
            // ένα enum PasswordVerificationResult με τιμές:
            // Success → το password ταιριάζει
            // Failed → δεν ταιριάζει
            // SuccessRehashNeeded → ταιριάζει αλλά χρειάζεται επανάληψη της διαδικασίας σε hash μορφή 
            // στην περίπτωση που έχει αλλάξει ο αλγόριθμος.
            var result = _passwordHasher.VerifyHashedPassword(admin, admin.AdminPasswordHash, password);

            // Μέσω χρήσης του τριαδικού τελεστή, Αν η επαλήθευση είναι επιτυχής, 
            // επιστρέφεται το αντικείμενο admin. Αν η επαλήθευση απέτυχε, επιστρέφεται null.
            // Αυτό σημαίνει ότι ο admin επιστρέφεται μόνο αν το login είναι σωστό.
            // Ενώ το null σημαίνει ότι το login απέτυχε, είτε λόγω λάθους username είτε λόγω λάθους password.
            return result == PasswordVerificationResult.Success ? admin : null;
        }

        // Η μέθοδος («Ελέγχει αν τα credentials του customer είναι έγκυρα») και εκτελεί αύγχρονες ενέργειες, επιτρέποντας την χρήση του await μέσα στην μέθοδο, καθώς δεν μπλοκάρεται το thread του server όσο περιμένουμε απάντηση από τη βάση.
        // Η μέθοδος αυτή παίρνει ως ορίσματα, το username (=που εισάγει ο χρήστης κατά την είσοδό του στο eshop) και τον κωδικό πρόσβασης (ο οποίος είναι plain text κωδικός που πληκτρολογεί για σύγκριση στην συνέχεια με το hashed password).
        // Επικυρώνει αν υπάρχει χρήστης με το δοθέν username και αν το password που παρείχε ταιριάζει με το αποθηκευμένο hashed password. Αν όλα είναι σωστά, επιστρέφει το Customer, αλλιώς null.
        public async Task<Customer?> ValidateCustomer(string username, string password)
        {
        // Εκτελεί ένα ασύγχρονο ερώτημα στη βάση δεδομένων μέσω του Entity Framework Core (_context.Customers) για να βρει την πρώτη εγγραφή Customer που έχει Username ίσο με την παράμετρο username.
        // Η εκτέλεση της μεθόδου (λόγω του await) «παγώνει» ασύγχρονα μέχρι να ολοκληρωθεί το query και το αποτέλεσμα αποθηκεύεται στη μεταβλητή customer.
        // Παρακάτω πραγματοποιείται η σύνδεση με τη βάση δεδομένων, όπου παρέχεται μέσω του Dependency Injection, όπου το _context.Customers (αντιστοιχεί στον πίνακα Customers) καθώς επιστρέφεται το τρέχον αντικείμενο που ταιριάζει. Η χρήση του Lambda expression (c => c.Username == username) => το c αντιπροσωπεύει κάθε εγγραφή (row) του πίνακα Customers, όπου στην ουσία βρίσκει τον customer του οποίου το Username είναι ίσο με αυτό που έδωσε ο χρήστης.
        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Username == username);
        // Αν το προηγούμενο query δεν βρήκε χρήστη (δηλαδή customer είναι null), τότε η μέθοδος επιστρέφει αμέσως null και αυτό έχει ως αποτέλεσμα να παρέχει προστασία από NullReferenceException και γρήγορη έξοδο όταν το username δεν υπάρχει. Αυτό σημαίνει ότι δεν επιχειρείται κανένας έλεγχος password αν δεν υπάρχει χρήστης.
        if (customer == null) return null;      // Εδώ εάν δεν υπάρχει customer με αυτό το username, τότε το login αποτυγχάνει κι επιστρέφεται το null. 
        // Παρακάτω καλείται ο μηχανισμός επαλήθευσης password (_customerPasswordHasher) για να συγκρίνει το plaintext password που παρείχε ο χρήστης με το αποθηκευμένο hashed password customer.PasswordHash.
        var result = _customerPasswordHasher.VerifyHashedPassword(customer, customer.PasswordHash, password);
        // Παρακάτω ελέγχεται το αποτέλεσμα της επαλήθευσης (result). Αν είναι ακριβώς PasswordVerificationResult.Success, τότε επιστρέφει το customer (επιτυχής authentication). Διαφορετικά επιστρέφει null.
        return result == PasswordVerificationResult.Success ? customer : null;      // Η VerifyHashedPassword επιστρέφει μια τιμή του enum PasswordVerificationResult (Failed ή Success).
        }
    }
}