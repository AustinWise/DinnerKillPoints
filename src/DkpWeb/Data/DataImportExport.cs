using DkpWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DkpWeb.Data
{
    static class DataImportExport
    {
        public static void ExportData(ApplicationDbContext db, string path)
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,

            };
            string jsonString = JsonSerializer.Serialize(db, options);
            File.WriteAllText(path, jsonString);
        }

        public static void ImportData(ApplicationDbContext db, string path)
        {
            var options = new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
            };

            TempDb imported;
            using (var fs = File.OpenRead(path))
            {
                imported = JsonSerializer.Deserialize<TempDb>(fs, options);
            }

            db.BillSplit.AddRange(imported.BillSplit);
            db.PaymentIdentity.AddRange(imported.PaymentIdentity);
            db.PaymentMethod.AddRange(imported.PaymentMethod);
            db.Person.AddRange(imported.Person);
            db.Transaction.AddRange(imported.Transaction);

            db.UserRoles.AddRange(imported.UserRoles);
            db.Roles.AddRange(imported.Roles);
            db.RoleClaims.AddRange(imported.RoleClaims);
            db.Users.AddRange(imported.Users);
            db.UserClaims.AddRange(imported.UserClaims);
            db.UserLogins.AddRange(imported.UserLogins);
            db.UserTokens.AddRange(imported.UserTokens);

            db.SaveChanges();
        }

        class TempDb
        {
            public List<BillSplit> BillSplit { get; set; }
            public List<PaymentIdentity> PaymentIdentity { get; set; }
            public List<PaymentMethod> PaymentMethod { get; set; }
            public List<Person> Person { get; set; }
            public List<Transaction> Transaction { get; set; }

            public List<IdentityUserRole<string>> UserRoles { get; set; }
            public List<IdentityRole> Roles { get; set; }
            public List<IdentityRoleClaim<string>> RoleClaims { get; set; }
            public List<ApplicationUser> Users { get; set; }
            public List<IdentityUserClaim<string>> UserClaims { get; set; }
            public List<IdentityUserLogin<string>> UserLogins { get; set; }
            public List<IdentityUserToken<string>> UserTokens { get; set; }

        }
    }
}
