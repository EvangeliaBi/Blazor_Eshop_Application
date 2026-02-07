using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlazorApp.Models    // Ο παρακάτω κώδικας βρίσκεται στο συγκεκιμένο project και εντός του φακέλου Models.
{
    public class Product    // Αποτελεί table για την βάση δεδομένων.
    {
        // Παρακάτω τα properties είναι οι στήλες της βάσης δεδομένων.
        public int Id {get; set;}
        public string ProductName {get; set;} = String.Empty;   // Κλήση του στατικού πεδίου Empty, για την αναπαράσταση κενού αλφαριθμητικού.
        public decimal ProductPrice {get; set;}
        public int ProductStock {get; set;} = 0;   // Αρχικοποιείται το απόθεμα με τιμή 0.
        public string? ImageFile {get; set;}
        public string Category { get; set; } = string.Empty;    // Αυτή η ιδιότητα αναπαριστά την κατηγορία του προιόντος, όπου η αρχικοποίηση σε κενό string αποφεύγει το null.
    }
}