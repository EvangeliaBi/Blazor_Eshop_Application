using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlazorApp.Models
{
    // Η κλάση αυτή αναπαριστά μία εγγραφή στην wishlist(=προσθήκη στην λίστα με τα αγαπημένα προιόντα ενός πελάτη).
    // Κάθε instance αντιστοιχεί σε μία γραμμή του πίνακα WishlistItems στη βάση και χρησιμοποιείται από το EF Core ως entity.
    public class WishlistItem       
    {
        public int Id { get; set; }     // Ιδιότητα που λειτουργεί ως το πρωτεύον κλειδί (PK). Χρησιμοποιείται για μοναδική ταυτοποίηση κάθε wishlist item.
        public int ProductId { get; set; }      // Εδώ αποθηκεύεται το αναγνωριστικό του προιόντος(foreign key), το οποίο είναι το ID του προϊόντος που έχει προστεθεί στη wishlist. Χρησιμοποιείται για συσχέτιση (foreign key) με τον πίνακα Products στη βάση δεδομένων, καθώς αυτό το πεδίο είναι απαραίτητο για να ξέρει η wishlist σε ποιο προϊόν αναφέρεται κάθε εγγραφή.
        public Product Product { get; set; } = null!;     // Σε αυτό το σημείο πρόκειται για μια πλοήγηση (navigation property), που επιτρέπει στο Entity Framework να φορτώσει το πλήρες αντικείμενο Product αντί μόνο το ProductId, επιτρέποντας εύκολη πρόσβαση σε πληροφορίες προϊόντος (item.Product.ProductName).
        public string CustomerUsername { get; set; } = string.Empty;     // Αποθηκεύει το username του χρήστη/πελάτη που έχει το wishlist item. Στην ουσία συσχετίζονται τα wishlist items με χρήστες μέσω username. Με το string.Empty, αποφεύγονται προειδοποιήσεις για nullable string και εξασφαλίζει ότι η ιδιότητα δεν είναι null από προεπιλογή.
    }
}