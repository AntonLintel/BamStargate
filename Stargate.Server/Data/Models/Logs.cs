using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Stargate.Server.Data.Models
{
    [Table("Logs")]
    public class Logs
    {
        public int Id { get; set; }

        public string Message { get; set; } = string.Empty;

        public string Level {  get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
    }

    public class LogsConfiguration : IEntityTypeConfiguration<Logs>
    {
        public void Configure(EntityTypeBuilder<Logs> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
