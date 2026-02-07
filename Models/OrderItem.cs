using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlazorApp.Models
{
    public class OrderItem    // Η κλάση αυτή αντιπροσωπεύει ένα αντικείμενο προϊόντος μέσα σε μία παραγγελία. Ορίζεται ως public class ώστε να μπορεί να χρησιμοποιηθεί από services, EF Core, controllers και Razor components.
    {
        public int Id { get; set; }   // Εδώ ορίζεται το πρωτεύον κλειδί της οντότητας, καθώς κάθε item που προστίθεται στη βάση θα έχει ένα μοναδικό Id.

        public int ProductId { get; set; }    // Εδώ αποθηκεύεται το id του προιόντος που αγοράστηκε, καθώς είναι χρήσιμο για να συνδεθεί το OrderItem με την αντίστοιχη εγγραφή στον πίνακα Products.
        public string ProductName { get; set; } = string.Empty;   // Εδώ αποθηκεύεται το όνομα του προιόντος την στιγμή της παραγγελίας, καθώς αρχικοποιείται με string.Empty για να αποφευχθεί το null.

        public decimal Price { get; set; }    // Εδώ αποθηκεύεται η τιμή του προιόντος κατά την στιγμή της παραγγελίας, είναι τύπου decimal, γιατί αφορά χρηματικό ποσό. Στην ουσία εδώ αποθηκεύεται η τιμή της παραγγελίας και όχι η τρέχουσα τιμή του προιόντος για το κατάστημα.
        public int Quantity { get; set; }     // Εδώ αποθηκεύονται πόσα τεμάχια του συγκεκριμένου προιόντος αγόρασε ο πελάτης, καθώς η ιδιότητα αυτή χρησιμοποιείται για υπολογισμό του υποσυνόλου (Subtotal = Price * Quantity) και του συνολικού ποσού της παραγγελίας.

        // Σχέση με Order
        public int OrderId { get; set; }    // Εδώ πρόκειται για το foreign key προς την παραγγελία (Order), καθώς χρησιμοποιείται για την σύνδεση αυτού του item με το γονικό που είναι το (Order). Επιτρέπει στο EF Core να συνδέσει OrderItem με Order και να δημιουργήσει το αντίστοιχο foreign key constraint στη βάση.
        public Order Order { get; set; } = null!;   // Πρόκειται για μία navigation ιδιότητα προς την οντότητα Order, για την πλοήγηση από το order item στην παραγγελία, εξασφαλίζοντας άμεση πρόσβαση στην παραγγελία ενός item.
    }
}
