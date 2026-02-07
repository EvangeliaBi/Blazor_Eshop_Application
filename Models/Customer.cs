using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlazorApp.Models
{
    // Παρακάτω η κλάση Customer θα αντιπροσωπεύει έναν πελάτη στην εφαρμογή, π.χ. για login, ταυτοποίηση ή διαχείριση παραγγελιών.
    public class Customer
    {
        public int Id { get; set; }     // Εδώ δηλώνεται ένα public property τύπου int που αντιπροσωπεύει το μοναδικό αναγνωριστικό (Primary Key) του πελάτη. Στη βάση δεδομένων, αυτή η ιδιότητα θα χρησιμοποιηθεί ως Primary Key για να ξεχωρίζουμε κάθε πελάτη μοναδικά.
        public string Username { get; set; } = string.Empty;       // Δηλώνεται το όνομα χρήστη του πελάτη για login, καθώς ορίζεται η αρχική τιμή της ως κενή συμβολοσειρά, ώστε να μην είναι null. Το Username χρησιμοποιείται για ταυτοποίηση του πελάτη κατά την είσοδο στην εφαρμογή.
        public string PasswordHash { get; set; } = string.Empty;    // Αυτή η ιδιότητα αποθηκεύει το hash του κωδικού πρόσβασης του πελάτη, όχι το plain text password, καθώς ορίζεται ως αρχική τιμή μία κενή συμβολοσειρά, για να αποφευχθεί το null. Χρησιμοποιείται για ασφαλή αποθήκευση των κωδικών.
    }
}