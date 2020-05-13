using SpacedRepetitionSystem.Entities.Entities.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpacedRepetitionSystem.Logic.Controllers.Identity
{
  public sealed class RefreshToken
  {
    public int TokenId { get; set; }
    public string UserId { get; set; }
    public string Token { get; set; }
    public DateTime ExpiryDate { get; set; }

    public User User { get; set; }
  }
}
