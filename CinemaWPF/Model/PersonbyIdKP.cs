using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaWPF.Model
{
    public class PersonbyIdKP
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class BirthPlace
        {
            [JsonProperty("value")]
            public string? Value { get; set; }
        }

        public class Fact
        {
            [JsonProperty("value")]
            public string? Value { get; set; }
        }

        public class Movie
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("alternativeName")]
            public string? AlternativeName { get; set; }

            [JsonProperty("rating")]
            public double? Rating { get; set; }

            [JsonProperty("general")]
            public bool? General { get; set; }

            [JsonProperty("description")]
            public string? Description { get; set; }

            [JsonProperty("enProfession")]
            public string? EnProfession { get; set; }
        }

        public class Person
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("enName")]
            public object? EnName { get; set; }

            [JsonProperty("photo")]
            public string? Photo { get; set; }

            [JsonProperty("sex")]
            public string? Sex { get; set; }

            [JsonProperty("growth")]
            public int? Growth { get; set; }

            [JsonProperty("birthday")]
            public DateTime? Birthday { get; set; }

            [JsonProperty("death")]
            public object? Death { get; set; }

            [JsonProperty("age")]
            public int? Age { get; set; }

            [JsonProperty("birthPlace")]
            public List<BirthPlace>? BirthPlace { get; set; }

            [JsonProperty("deathPlace")]
            public List<object>? DeathPlace { get; set; }

            [JsonProperty("spouses")]
            public List<Spouse>? Spouses { get; set; }

            [JsonProperty("countAwards")]
            public int? CountAwards { get; set; }

            [JsonProperty("profession")]
            public List<object>? Profession { get; set; }

            [JsonProperty("facts")]
            public List<Fact>? Facts { get; set; }

            [JsonProperty("movies")]
            public List<Movie>? Movies { get; set; }

            [JsonProperty("createdAt")]
            public DateTime? CreatedAt { get; set; }

            [JsonProperty("updatedAt")]
            public DateTime? UpdatedAt { get; set; }
        }

        public class Spouse
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("name")]
            public object? Name { get; set; }

            [JsonProperty("divorced")]
            public bool? Divorced { get; set; }

            [JsonProperty("divorcedReason")]
            public string? DivorcedReason { get; set; }

            [JsonProperty("children")]
            public int? Children { get; set; }

            [JsonProperty("relation")]
            public string? Relation { get; set; }
        }


    }
}
