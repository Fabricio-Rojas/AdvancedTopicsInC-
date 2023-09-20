using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankingApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BankingApp.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BankingAppUser class
public class BankingAppUser : IdentityUser
{
    public HashSet<Account> Accounts { get; set; } = new HashSet<Account>();
}

