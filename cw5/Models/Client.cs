using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace cw5.Models;

public partial class Client
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdClient { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Telephone { get; set; } = null!;

    [Required]
    public string Pesel { get; set; } = null!;
    
    [JsonIgnore]
    public virtual ICollection<ClientTrip> ClientTrips { get; set; } = new List<ClientTrip>();
}
