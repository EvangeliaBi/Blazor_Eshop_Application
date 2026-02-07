using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBlazorApp.Models;
using Microsoft.EntityFrameworkCore;   // Το πακέτο αυτό θα μας δώσει πρόσβαση σε όλες τις κλάσεις του DbContext.

namespace MyBlazorApp.Data
{
    // Μέσω της κλάσης DbContext έχω πρόσβαση στην βάση δεδομένων 
    // και για αυτό η κλάση που φτιάξαμε κληρονομεί από αυτή την κλάση τα πάντα.
    // Θα πρέπει το DbContext για την δική μας την εφαρμογή να κληρονομεί τα χαρακτηριστικά και τις
    // ιδιότητες αυτής της κλάσης.
    public class AppDbContext : DbContext   
    {
        // Δημιουργία του κατασκευαστή AppDbContext για να ικανοποιηθεί ο γονικός κατασκευαστής
        // (base constructor), λόγω κληρονομικότητας που υπάρχει από την κλάση DbContext. 
        // Στον γονικό κατασκευαστή περνάμε ένα (object) αντικείμενο, το οποίο είναι το (options).
        // Αυτός ο τύπος <AppDbContext> είναι ένας παραμετροποιήσιμος τύπος, όπου το AppDbContextOptions 
        // είναι μία άλλη κλάση, η οποία δουλεύει με πολλούς τύπους και μέσα σε όλους αυτούς τους
        // τύπους θα καθοριστεί με ποιον συγκεκριμένο τύπο θέλει να δουλέψει.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        // Παρακάτω κάθε αντικείμενο, το οποίο είναι τύπου DbSet θα αναπαριστά 
        // τον αντίστοιχο πίνακα στην βάση δεδομένων, όπου θα μπορούν 
        // να χρησιμοποιηθούν για εκτέλεση queries στη Βάση Δεδομένων μέσω της βιβλιοθήκης LINQ.
        // Παρακάτω έχουμε Collections από Products και από Admins. Το κάθε DbSet, θα περιέχει
        // μέσα αντικείμενα τύπου Product ή αντικείμενα τύπου Admin. Για όσα DbSet φτιάξω θα
        // δημιουργηθούν και οι αντίστοιχοι πίνακες στην βάση δεδομένων.
        public DbSet<Product> Products{get; set;}   
        public DbSet<Admin> Admins{get; set;}
        public DbSet<Customer> Customers { get; set; }     // Προσθήκη και της οντότητας customer.
        public DbSet<Order> Orders => Set<Order>();        // Πίνακας Orders με αντικείμενα τύπου Order.
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();     // Πίνακας OrderItems με αντικείμενα τύπου OrderItem
        public DbSet<WishlistItem> WishlistItems { get; set; }     // Πίνακας WishlistItems με αντικείμενα τύπου WishlistItem, που θα χρησιμοποιηθεί από υπηρεσίες όπως η WishlistService.


    // Η παρακάτω μέθοδος θα εκτελεστεί μόλις το EntityFrameworkCore δημιουργήσει το DB Model. 
    // Το ModelBuilder object είναι ένα tool για το πως θα γίνει το mapping των Models 
    // στους αντίστοιχους πίνακες.
    // Η μέθοδος OnModelCreating κληρονομείται από την γονική κλάση κι άρα αντιγράφεται μέσω
    // του override, καθώς αυτό σημαίνει ότι η συγκεκριμένη μέθοδος έχει δηλωθεί στην γονική 
    // κλάση ως virtual(=δύνανται να αντιγραφεί).  
    // Η μέθοδος OnModelCreating θα εκτελεστεί 1 μόνο φορά (όταν δηλαδή θα εισαχθούν τα δεδομένα), και συγκεκριμένα όταν εισαχθεί το DbContext.
    protected override void OnModelCreating(ModelBuilder modelBuilder)  // Χρησιμοποιείται ένα γενικού τύπου Object για να εισάγουμε δεδομένα ή να διαγράφουμε δεδομένα.
    {
      base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData (    // Το Entity είναι μέθοδος του ModelBuilder, για την εισαγωγή δεδομένων και μέσω της HasData δημιουργούνται τα αντίστοιχα insert.
                // Παρακάτω καλείται ο κατασκευαστής της κλάσης Product, για την δημιουργία ενός
                // αντικειμένου, δηλαδή την δημιουργία ενός προιόντος.
                new Product{Id = 1, ProductName = "Asus Vivobook X1605VA-OLED-SH2182W Laptop 16 Core i7 13620H/16 GB/1 TB/UHD Graphics/Windows 11 Home", ProductPrice = 699, ProductStock = 10, ImageFile = "", Category = "Laptop"},
                new Product{Id = 2, ProductName = "Dell Alienware Aurora 16 Laptop 16 WQXGA Core 7 240H/32 GB/1 TB/RTX 5060 8 GB/Windows 11 Home", ProductPrice = 1299, ProductStock = 5, ImageFile = "", Category = "Laptop"},
                new Product{Id = 3, ProductName = "Apple MacBook Air 13 M4 8-Core/16GB/256GB/10-Core GPU Midnight Laptop", ProductPrice = 1149, ProductStock = 20, ImageFile = "/images/products/mac1.png", Category = "Laptop"},
                new Product{Id = 4, ProductName = "Apple MacBook Pro 16 M4 Max 16-Core/48GB/1TB/40-Core GPU Silver Laptop", ProductPrice = 4839, ProductStock = 10, ImageFile = "/images/products/mac2.png", Category = "Laptop"},
                new Product{Id = 5, ProductName = "Dell XPS 9640 U7-155H/32GB/1TB/W11P RTX 4060 8GB Laptop", ProductPrice = 2699, ProductStock = 30, ImageFile = "/images/products/dell.png", Category = "Laptop"},
                new Product{Id = 6, ProductName = "Green Cell Battery For Hp Elitebook Folio 9470m 9480m / 14,4v 3500mah Hp119", ProductPrice = 48, ProductStock = 20, ImageFile = "/images/products/png1.png", Category = "Batteries"}
            );

            modelBuilder.Entity<Admin>().HasData (
                // Κι εδώ καλείται ο κατασκευαστής της κλάσης Admin, για την δημιουργία ενός
                // αντικειμένου, δηλαδή ενός διαχειριστή καταστήματος.
                // Όταν ο Admin θα καταχωρεί ένα προιόν τότε δεν θα χρειάζεται να συμπληρώνει το 
                // id του προιόντος.
                // Το AdminPasswordHash περιέχει το hash του password, όχι το plain password.
                // Το hash έχει δημιουργηθεί με το PasswordHasher<Admin>.
                // Αυτό σημαίνει ότι κανείς δεν βλέπει το plain password στη βάση.
                // Μέσω της εντολής dotnet ef migrations add UpdateAdminPasswordSeed, δημιουργήθηκε στην συνέχεια,
                // ένα migration, το οποίο παίρνει υπόψην τις αλλαγές που έγιναν στο OnModelCreating 
                // και δημιουργεί ένα αρχείο migration, χωρίς να αλλάζει το schema της βάσης (τύπους ή νέα πεδία),
                // αλλάζοντας μόνο τα password hashes των admins.
                // Τέλος μέσω της εντολής dotnet ef database update, εφαρμόστηκε το νέο migration και η βάση
                // ενημερώθηκε με τα νέα hashed passwords των admins. 
                new Admin {Id = 1, AdminUserName = "TechAdmin1", AdminPasswordHash = "AQAAAAIAAYagAAAAEKw4201BnMtft9Iz+/LKObjQQtYQfKzEAmGsCa/OTkcQsZVTFv3XR4hACZ2Rtei9Og=="},
                new Admin {Id = 2, AdminUserName = "TechAdmin2", AdminPasswordHash = "AQAAAAIAAYagAAAAEHyQVL2GA9mtR7BideC77KIazRdMhPpNYdYREjHm8sPn1ua0oZb7WcZp9RF4JuYk9w=="}
            );


            // Παρακάτω χρησιμοποιείται το modelBuilder, το οποίο είναι το αντικείμενο που παρέχει το EF Core μέσα στην OnModelCreating(ModelBuilder modelBuilder) του DbContext για την διαμόρφωση του σχήματος του μοντέλου (entities, σχέσεις, constraints) και ορισμό δεδομένων.
            // Επιστρέφεται ένας EntityTypeBuilder<Customer> που επιτρέπει την εφαρμογή ρυθμίσεων για την οντότητα Customer.
            modelBuilder.Entity<Customer>().HasData(
                // Παρακάτω δημιουργείται ένα στιγμιότυπο (instance) της κλάσης Customer με τις ιδιότητες που ακολουθούν, με τιμές το Id του κάθε πελάτη που λειτουργεί ως πρωτεύον κλειδί, το όνομα του πελάτη και την hashed μορφη του κωδικού, καθώς η συγκεκριμένη συμβολοσειρά είναι το αποτέλεσμα του μηχανισμού hashing που χρησιμοποιεί το ASP.NET Core Identity (συνήθως PasswordHasher<TUser>), ο οποίος παράγει ένα base64‑encoded blob που περιέχει πληροφορίες όπως έκδοση αλγορίθμου, salt και subkey (το τελικό hash).
                // Εδώ καθορίζονται 2 αρχικοί χρήστες που θα υπάρχουν στην βάση δεδομένων από την αρχή.
                new Customer{Id = 1, Username = "client1", PasswordHash = "AQAAAAIAAYagAAAAENrC26B5B3NUJz1gHkP/L71m+TmRydBEBYpOCST+NypUHYejCsZc/f2nTB+lnGRCKg=="},
                new Customer{Id = 2, Username = "client2", PasswordHash = "AQAAAAIAAYagAAAAEFEXGCpv9CkS5/MUG3ltvJ6rss2+59AhrPgo+ekuUbJiASWTpOdVEal8HdX6lh5kKg=="}
            );
    }
    }
}