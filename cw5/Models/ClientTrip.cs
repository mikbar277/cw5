using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace cw5.Models;

public partial class ClientTrip
{
    public int IdClient { get; set; }

    public int IdTrip { get; set; }

    public DateTime RegisteredAt { get; set; }

    public DateTime? PaymentDate { get; set; }
    
    [JsonIgnore]
    public virtual Client IdClientNavigation { get; set; } = null!;

    [JsonIgnore]
    public virtual Trip IdTripNavigation { get; set; } = null!;
}
