using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlazorApp.Models
{
    public class Admin  // Αποτελεί table για την βάση δεδομένων.
    {
        // Κάθε property παρακάτω αναπαρίσταται ως στήλη στην βάση δεδομένων.
        public int Id {get; set;}   // Λόγω του Entity Framework, το Id αναγνωρίζεται αυτόματα ότι πρόκειται για πρωτεύον κλειδί στην βάση δεδομένων.

        // Παρακάτω για το Username και το Password για τον χρήστη καλείται το στατικό πεδίο
        // Empty μέσω της κλάσης String, το οποίο αναπαριστά το κενό αλφαριθμητικό.
        public string AdminUserName {get; set;} = String.Empty;

        // Η κλάση (Admin) αποθηκεύει μόνο το hashed password και όχι τον κωδικό με την μορφή 
        // plain text, δηλαδή δεν αποθηκεύει το plain password.      
        public string AdminPasswordHash {get; set;} = String.Empty;
    }
}