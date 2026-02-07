using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Data;
using MyBlazorApp.Models;

namespace MyBlazorApp.Services
{
    public class CartItem      // Η κλάση αυτή θα αντιπροσωπεύει ένα στοιχείο μέσα στο καλάθι.
{
    public Product Product { get; set; } = null!;     // Περιέχει το αντικείμενο Product που προστέθηκε στο καλάθι. Στην ουσία αποθηκεύεται ολόκληρο το Product, το οποίο προστέθηκε στο καλάθι, προκειμένου να εξασφαλιστεί πρόσβαση σε (ProductName,ProductPrice κλπ.)
    public int Quantity { get; set; } = 1;     // Σε αυτό το σημείο δηλώνεται η ποσότητα του προϊόντος στο καλάθι και την αρχικοποιεί σε 1 ως προεπιλογή. Αυτό σημαίνει ότι όταν προσθέτεις ένα νέο CartItem θα έχει την ποσότητα 1.
}


public class CartService    // Υπηρεσία που διαχειρίζεται την κατάσταση του καλαθιού σε μνήμη (in‑memory), καθώς εγγράφεται στο DI container, κρατά την κατάσταση του καλαθιού καθώς χρησιμοποιείται από Product details page και Cart page και γίνεται registered ως Scoped.
{
    private readonly List<CartItem> _items = new();     // Δημιουργία κι αρχικοποίηση λίστας που αποθηκεύει όλα τα items του καλαθιού, το οποίο στην ουσία αναπαριστά το εσωτερικό state του καλαθιού.
    public IReadOnlyList<CartItem> Items => _items;     // Μέσω της διεπαφής(IReadOnlyList), εκθέτονται τα στοιχεία του καλαθιού, μόνο για ανάγνωση, αποτρέποντας την απευθείας τροποποίηση της λίστας, καθώς γίνεται χρήση των μεθόδων add/remove. Στην ουσία άλλες κλάσεις μπορούν να δούν τα δεδομένα χωρίς όμως να τα τροποποιήσουν.

    public void Add(Product product)    // Μέσω της μεθόδου αυτής προστίθενται ένα προιόν στο καλάθι, εξ ου που δέχεται ως παράμετρο εισόδου το προιόν εκείνο που προστίθενται στο καλάθι.
    {
        var item = _items.FirstOrDefault(x => x.Product.Id == product.Id);     // Εδώ στην ουσία γίνεται αναζήτηση στην _items το πρώτο CartItem όπου το Product.Id ταιριάζει με το product.Id που προσπαθούμε να προσθέσουμε. Αν βρεθεί επιστρέφει το CartItem, αλλιώς επιστρέφει null. Η μέθοδος FirstOrDefault είναι μέρος του LINQ και επιστρέφει default (εδώ null) όταν δεν υπάρχει αποτέλεσμα.
        if (item != null)       // Σε αυτό το σημείο γίνεται έλεγχος, όπου εάν το προιόν δεν είναι null, που σημαίνει ότι βρέθηκε ήδη CartItem για το ίδιο προϊόν, αυξάνει το Quantity κατά 1. Αυτό υλοποιεί τη συμπεριφορά ότι «αν το προϊόν υπάρχει, αυξάνεται αντίστοιχα και η ποσότητα».
            item.Quantity++;    // Αύξηση της ποσότητας κατά 1, για το συγκεκριμένο item.
        else
            _items.Add(new CartItem { Product = product });     // Αν δεν υπάρχει ήδη το προϊόν στο καλάθι, τότε δημιουργείται ένα νέο CartItem με Product = product και προσθέτει το αντικείμενο στη λίστα _items και το Quantity θα είναι 1 λόγω της προεπιλεγμένης τιμής.
    }

    public void Remove(Product product)     // Μέσω της μεθόδου αυτής αφαιρείται ένα προιόν από το καλάθι ή μειώνεται η ποσότητα του συγκεκριμένου προιόντος.
    {
        var item = _items.FirstOrDefault(x => x.Product.Id == product.Id);      // Αναζήτηση του CartItem που αντιστοιχεί στο Id ενός συγκεκριμένου προιόντος.
        if (item != null)       // Εδώ ελέγχεται εάν βρέθηκε το item, πριν γίνει η τροποποίησή του.
        {
            item.Quantity--;        // Εδώ μειώνεται η ποσότητα κατά 1, από την στιγμή που θέλουμε να αφεραίσουμε μία μονάδα κάθε φορά που καλείται η μέθοδος Remove.
            if (item.Quantity <= 0)     // Αν μετά τη μείωση η ποσότητα γίνει 0 ή αρνητική, αφαιρείται εντελώς το CartItem από τη λίστα κι έτσι δεν διατηρούνται εγγραφές με μηδενική ποσότητα.
                _items.Remove(item);
        }
    }

    // Παρακάτω για κάθε cart item υπολογίζεται η (τιμή του προιόντος * την ποσότητα), καθώς επιστρέφεται το συνολικό κόστος του καλαθιού ως decimal. Χρησιμοποιείται η LINQ Sum για να αθροίσει για κάθε CartItem το γινόμενο ProductPrice * Quantity. 
    public decimal Total => _items.Sum(x => x.Product.ProductPrice * x.Quantity);   

    // Η μέθοδος αυτή αδειάζει το καλάθι και (χρησιμοποιείται μετά το checkout ή όταν ο χρήστης θέλει να κάνει εκκαθάριση του καλαθιού του). Η μέθοδος Clear() αφαιρεί όλα τα στοιχεία από την εσωτερική λίστα.
    public void Clear()     
    {
        _items.Clear();
    }

}
}