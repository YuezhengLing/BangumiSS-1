namespace BangumiSS.Model
{
    using System.Data.Entity;

    public partial class BangumiModel : DbContext
    {
        public BangumiModel()
            : base("name=BangumiModel")
        {
        }

        public virtual DbSet<番组> 番组 { get; set; }
        public virtual DbSet<资源> 资源 { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<番组>()
                .Property(e => e.档期)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<番组>()
                .Property(e => e.星期)
                .IsFixedLength();

            modelBuilder.Entity<番组>()
                .Property(e => e.Bangumi编号)
                .IsFixedLength();

            modelBuilder.Entity<番组>()
                .HasMany(e => e.资源)
                .WithRequired(e => e.番组)
                .WillCascadeOnDelete(false);
        }

        private static BangumiModel model = new BangumiModel();
        public static BangumiModel DbModel
        {
            get
            {
                if (model == null)
                    model = new BangumiModel();
                return model;
            }
        }

        public static void RefreshModel() => model = new BangumiModel();       
    }
}
