using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Stargate.Server.Data.Models
{
    [Table("Person")]
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? CurrentRank { get; set; } = string.Empty;

        public string? CurrentDutyTitle { get; set; } = string.Empty;

        public DateTime? CareerStartDate { get; set; }

        public DateTime? CareerEndDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<AstronautDuty>? AstronautDuties { get; set; }

    }

    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            // RULE: unique by name
            builder.HasIndex(x => x.Name).IsUnique();

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd().IsRequired();
            builder.HasMany(z => z.AstronautDuties)
                   .WithOne(z => z.Person)
                   .HasForeignKey(z => z.PersonId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
