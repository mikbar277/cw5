using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace cw5.Models;

public partial class Trip
{
    [Key]
    public int IdTrip { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public int MaxPeople { get; set; }
    
    [JsonIgnore]
    public virtual ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();

    [JsonIgnore]
    public virtual ICollection<Country> IdCountries { get; set; } = new List<Country>();
}
