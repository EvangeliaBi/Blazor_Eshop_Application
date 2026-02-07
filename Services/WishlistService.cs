using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using MyBlazorApp.Models;

namespace MyBlazorApp.Services
{
    // Η υπηρεσία WishlistService διαχειρίζεται όλα τα wishlist operations για έναν χρήστη. Η κλάση αυτή ορίζεται ως public, διότι θα χρησιμοποιηθεί από άλλα μέρη της εφαρμογής (π.χ. components, controllers).
    public class WishlistService
    {
        public event Action? OnChange;      // Εδώ δημιουργείται ένα event τύπου Action που μπορεί να εγγραφούν components για να λαμβάνουν ειδοποίηση όταν αλλάζει το wishlist. Το ? σημαίνει ότι το event μπορεί να είναι null. Επιτρέπει στα components να εγγραφούν και να ενημερωθούν όταν το wishlist αλλάξει (για άμεσο UI update). Η χρήση Action σημαίνει ότι οι handlers δεν παίρνουν παραμέτρους και δεν επιστρέφουν τιμή.

        private void NotifyChange()     // Μέσω αυτής της μεθόδου καλείται το OnChange με OnChange?.Invoke() μέσα σε try/catch.
        {
            try { OnChange?.Invoke(); }     // Στην ουσία καλείται μετά από επιτυχή εκτέλεση Add/Remove για να ειδοποιήσει εγγεγραμμένα components. Το try/catch αποτρέπει ένα σφάλμα σε έναν handler στην περίπτωση που θα επιχειρηθεί να σπάσει τη ροή του service.
            catch {  }
        }

        private readonly AppDbContext _db;      // Σε αυτό το σημείο αποθηκεύεται το injected DbContext, καθώς το _db εκτελεί τα απαραίτητα queries και τις αντίστοιχες αλλαγές στη βάση.
        
        // Παρακάτω ο λόγος για τον οποίο χρησιμοποιείται το SemaphoreSlim είναι, γιατί το DbContext δεν είναι thread‑safe, η κοινή χρήση του σε concurrent tasks μπορεί να προκαλέσει exceptions. Η σειριοποίηση με SemaphoreSlim συμβαίνει όταν η υπηρεσία μοιράζεται ένα DbContext instance και πρέπει να αποφευχθεί ταυτόχρονη πρόσβαση.
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);    // Σε αυτό το σημείο δημιουργείται ένα semaphore με 1 permit, το οποίο αποτρέπει ταυτόχρονες (concurrent) EF operations πάνω στο ίδιο DbContext. Χρησιμοποιείται για να σειριοποιήσει (serialize) τις προσβάσεις στο _db ώστε να αποφευχθεί ταυτόχρονη χρήση του ίδιου DbContext από πολλαπλά concurrent tasks. Επειδή το DbContext δεν είναι ασφαλές για χρήση από πολλαπλά νήματα ταυτόχρονα, η σειριοποίηση με SemaphoreSlim είναι μια πρακτική για να αποφευχθούν race conditions ή exceptions όταν η υπηρεσία μπορεί να κληθεί ταυτόχρονα. 

        public WishlistService(AppDbContext db)     // Ο κατασκευαστής αυτός παίρνει το AppDbContext μέσω dependency injection και το αποθηκεύει στο _db, καθώς αυτό επιτρέπει στο DI container να παρέχει το context με scoped lifetime. Στην ουσία η υπηρεσία τώρα χρησιμοποιεί το ίδιο DbContext instance που παρέχει ο DI container.
        {
            _db = db;
        }

        // Παρακάτω δηλώνεται μία ασύγχρονη μέθοδος που επιστρέφει την λίστα WishlistItem για έναν χρήστη.
        public async Task<List<WishlistItem>> GetForUserAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))    // Σε αυτό το σημείο ελέγχεται εάν το username είναι null ή κενό και τότε εκτελείται το μπλοκ κώδικα του if επιστρέφοντας μία καινούργια άδεια λίστα WishlistItem.
                return new List<WishlistItem>();

            await _semaphore.WaitAsync();       // Εδώ σε αυτό το σημείο αναμένεται η απόκτηση του semaphore πριν συνεχίσει, καθώς σειριοποιείται η πρόσβαση στο _db ώστε να μην τρέχουν παράλληλες EF operations. Αν κάποια άλλη λειτουργία κρατάει ήδη το semaphore, αυτή η κλήση θα περιμένει χωρίς να μπλοκάρει το thread.
            try
            {    
                return await _db.WishlistItems
                                .Include(w => w.Product)    // Εντός του try μπλοκ φορτώνεται το navigation property Product μαζί με κάθε WishlistItem, καθώς με αυτόν τον τρόπο το component έχει άμεσα πρόσβαση σε item.Product.ProductName χωρίς επιπλέον queries. Στην ουσία φορτώνεται το navigation property Product μαζί με κάθε WishlistItem (eager loading), ώστε ο κώδικας να έχει άμεση πρόσβαση σε item.Product χωρίς επιπλέον queries.
                                .Where(w => w.CustomerUsername == username)     // Σε αυτό το σημείο φιλτράρονται τα records για το συγκεκριμένο username.
                                .ToListAsync();     // Κι εδώ υλοποιείται το query ασύγχρονα και επιστρέφει List<WishlistItem>.
            }
            finally
            {
                _semaphore.Release();     // Σε αυτό το σημείο απελευθερώνεται το semaphore, ακόμη και αν προκύψει εξαίρεση.
            }
        }

        // Παρακάτω δηλώνεται μία ασύγχρονη μέθοδος που προσπαθεί να προσθέσει το product στο wishlist του username/ενός πελάτη και επιστρέφει true αν προστέθηκε.
        public async Task<bool> AddAsync(Product product, string username)
        {
            if (product == null || string.IsNullOrWhiteSpace(username))     // Εδώ ελέγχεται η εγκυρότητα των παραμέτρων, καθώς ελέγχεται εάν το προιόν είναι null ή εάν το username αποτελείται από κενούς χαρακτήρες.
                return false;

            await _semaphore.WaitAsync();       // Σε αυτό το σημείο αποκτάται το semaphore για σειριοποίηση.
            try
            {
                var exists = await _db.WishlistItems    // Σε αυτό το σημείο ελέγχει αν υπάρχει ήδη εγγραφή με το ίδιο (ProductId, CustomerUsername), αν δεν υπάρχει κάνει την προσθήκη του προιόντος μέσα στην wishlist αλλιώς δεν προσθέτει κάτι ξανά, αποτρέποντας τις διπλοεγγραφές.
                                      .AnyAsync(w => w.ProductId == product.Id && w.CustomerUsername == username);
                if (exists) return false;      // Ελέγχει αν το προϊόν υπάρχει ήδη στο wishlist. Αν ναι, δεν το προσθέτει.

                // Δημιουργεί το νέο WishlistItem και αποθηκεύει τις αλλαγές στη βάση.
                _db.WishlistItems.Add(new WishlistItem { ProductId = product.Id, CustomerUsername = username });
                await _db.SaveChangesAsync();       // Εδώ πραγματοποιείται η εφαρμογή των αλλαγών στην βάση μέσω του (INSERT).

                // Εδώ καλείται η NotifyChange, προκειμένου να ειδοποιήσει όλα τα components ότι το wishlist άλλαξε, επιστρέφοντας την τιμή true, για την επιτυχή αλλαγή.
                NotifyChange();     // Εδώ ειδοποιούνται εγγεγραμμένα components ότι το wishlist άλλαξε — γίνεται μετά το SaveChangesAsync() ώστε οι listeners να δουν την ενημερωμένη κατάσταση.
                return true;
            }
            catch (DbUpdateException)   // Σε αυτό το σημείο εντός του catch μπλοκ πιάνονται σφάλματα ενημέρωσης DB(πχ.dublicate key), επιστρέφοντας false εάν πέσει σε κάποια εξαίρεση.
            {
                
                return false;
            }
            finally
            {
                _semaphore.Release();   // Σε αυτό το σημείο γίνεται απελευθέρωση του (semaphore).
            }
        }

        // Αφαιρεί προϊόν από το wishlist που αντιστοιχεί σε ένα συγκεκριμένο productId για έναν συγκεκριμένο πελάτη, (επιστρέφοντας true αν αφαιρέθηκε).
        public async Task<bool> RemoveAsync(int productId, string username)
        {
            // Παρακάτω ελέγχεται αν το username είναι null, κενό ή έχει whitespaces τότε η μέθοδος τερματίζεται άμεσα με false. Αποφεύγει άσκοπες DB κλήσεις και πιθανά exceptions.
            if (string.IsNullOrWhiteSpace(username))    // Εδώ ελέγχεται εάν το username είναι null ή κενό επιστρέφοντας false.
                return false;

            await _semaphore.WaitAsync();   // Αποκτά το SemaphoreSlim πριν την πρόσβαση στη βάση. Σκοπός: σειριοποίηση των EF operations ώστε να μην τρέχουν ταυτόχρονα πάνω στο ίδιο DbContext.
            try
            {
                // Εντός του try μπλοκ εκτελείται το ασύγχρονο query που επιστρέφει το πρώτο matching WishlistItem ή null αν δεν υπάρχει. Πιο συγκεκριμένα η επιλογή FirstOrDefaultAsync: φορτώνει ολόκληρο το αντικείμενο WishlistItem, εξασφαλίζοντας ότι η εγγραφή ανήκει στον συγκεκριμένο χρήστη και στο συγκεκριμένο προιον.
                var item = await _db.WishlistItems  
                                    .FirstOrDefaultAsync(w => w.ProductId == productId && w.CustomerUsername == username);
                if (item == null) return false;     // Σε αυτό το σημείο εάν δεν βρεθεί η εγγραφή τότε δεν υπάρχει κάτι για να διαγραφεί κι επιστρέφεται false.

                _db.WishlistItems.Remove(item);     // Εδώ αφαιρείται το item κι αποθηκεύεται στην βάση.
                await _db.SaveChangesAsync();       // Εφαρμόζονται οι αλλαγές στη βάση (εκτελεί DELETE SQL). Είναι το σημείο όπου η διαγραφή γίνεται μόνιμη στην DB, καθώς είναι ασύγχρονο για να μην μπλοκάρει το thread.

                // Ειδοποίηση components μετά την επιτυχή διαγραφή. Παρακάτω καλείται το τοπικό event OnChange ώστε εγγεγραμμένα components να ενημερωθούν (π.χ. να ανανεώσουν το UI). Γίνεται μετά το SaveChangesAsync ώστε οι listeners να δουν την τρέχουσα κατάσταση.
                NotifyChange();     // Ειδοποιεί listeners μετά την επιτυχή διαγραφή.
                return true;    // Εδώ επιστρέφεται true για την επιτυχή διαγραφή.
            }
            finally
            {
                _semaphore.Release();
            }
        }

        // Παρακάτω υλοποιείται έλεγχος αν υπάρχει προϊόν στο wishlist.
        public async Task<bool> IsInWishlistAsync(int productId, string username)
        {
            if (string.IsNullOrWhiteSpace(username))    // Αν το username είναι null, κενό ή μόνο whitespace, η μέθοδος τερματίζει άμεσα με false
                return false;

            await _semaphore.WaitAsync();   // Σειριοποίηση των EF operations ώστε να μην τρέχουν ταυτόχρονα πάνω στο ίδιο DbContext. Η κλήση είναι ασύγχρονη — αν κάποια άλλη εργασία κρατάει ήδη το semaphore, αυτή η κλήση θα περιμένει χωρίς να μπλοκάρει το thread.
            try
            {
                return await _db.WishlistItems      // Σε αυτό το σημείο εκτελείται ένα ασύγχρονο query που επιστρέφει true αν υπάρχει τουλάχιστον ένα record
                                .AnyAsync(w => w.ProductId == productId && w.CustomerUsername == username);
            }
            finally     // Το finally εξασφαλίζει ότι το permit θα επιστραφεί πάντα, ακόμη και αν προκύψει εξαίρεση μέσα στο try.
            {
                _semaphore.Release();
            }
        }
    }
}
