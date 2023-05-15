using BookBrowser.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookBrowser;

public partial class CalibreDbContext : DbContext
{
    public CalibreDbContext()
    {
    }

    public CalibreDbContext(DbContextOptions<CalibreDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Annotation> Annotations { get; set; }

    public virtual DbSet<AnnotationsDirtied> AnnotationsDirtieds { get; set; }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BooksLanguagesLink> BooksLanguagesLinks { get; set; }

    public virtual DbSet<BooksPluginDatum> BooksPluginData { get; set; }

    public virtual DbSet<BooksPublishersLink> BooksPublishersLinks { get; set; }

    public virtual DbSet<BooksRatingsLink> BooksRatingsLinks { get; set; }

    public virtual DbSet<BooksSeriesLink> BooksSeriesLinks { get; set; }

    public virtual DbSet<BooksTagsLink> BooksTagsLinks { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<ConversionOption> ConversionOptions { get; set; }

    public virtual DbSet<CustomColumn> CustomColumns { get; set; }

    public virtual DbSet<LibraryContent> Data { get; set; }

    public virtual DbSet<Feed> Feeds { get; set; }

    public virtual DbSet<Identifier> Identifiers { get; set; }

    public virtual DbSet<Language> Languages { get; set; }

    public virtual DbSet<LastReadPosition> LastReadPositions { get; set; }

    public virtual DbSet<LibraryId> LibraryIds { get; set; }

    public virtual DbSet<MetadataDirtied> MetadataDirtieds { get; set; }

    public virtual DbSet<Preference> Preferences { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Series> Series { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Annotation>(entity =>
        {
            entity.ToTable("annotations");

            entity.HasIndex(e => new { e.Book, e.UserType, e.User,
                e.Format, e.AnnotType, e.AnnotId },
                "IX_annotations_book_user_type_user_format_annot_type_annot_id").IsUnique();

            entity.HasIndex(e => e.Book, "annot_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.AnnotData).HasColumnName("annot_data");
            entity.Property(e => e.AnnotId).HasColumnName("annot_id");
            entity.Property(e => e.AnnotType).HasColumnName("annot_type");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Format).HasColumnName("format");
            entity.Property(e => e.SearchableText)
                .HasDefaultValueSql("\"\"")
                .HasColumnName("searchable_text");
            entity.Property(e => e.Timestamp).HasColumnName("timestamp");
            entity.Property(e => e.User).HasColumnName("user");
            entity.Property(e => e.UserType).HasColumnName("user_type");
        });

        modelBuilder.Entity<AnnotationsDirtied>(entity =>
        {
            entity.ToTable("annotations_dirtied");

            entity.HasIndex(e => e.Book, "IX_annotations_dirtied_book").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
        });

        modelBuilder.Entity<Author>(entity =>
        {
            RelationalEntityTypeBuilderExtensions.ToTable((EntityTypeBuilder) entity, "authors");

            entity.HasIndex(e => e.Name, "IX_authors_name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Link)
                .HasDefaultValueSql("\"\"")
                .HasColumnName("link");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Sort).HasColumnName("sort");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("books");

            entity.HasIndex(e => e.AuthorSort, "authors_idx");

            entity.HasIndex(e => e.Sort, "books_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorSort).HasColumnName("author_sort");
            entity.Property(e => e.Flags)
                .HasDefaultValueSql("1")
                .HasColumnName("flags");
            entity.Property(e => e.HasCover)
                .HasDefaultValueSql("0")
                .HasColumnType("BOOL")
                .HasColumnName("has_cover");
            entity.Property(e => e.Isbn)
                .HasDefaultValueSql("\"\"")
                .HasColumnName("isbn");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("\"2000-01-01 00:00:00+00:00\"")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("last_modified");
            entity.Property(e => e.Lccn)
                .HasDefaultValueSql("\"\"")
                .HasColumnName("lccn");
            entity.Property(e => e.Path)
                .HasDefaultValueSql("\"\"")
                .HasColumnName("path");
            entity.Property(e => e.Pubdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("pubdate");
            entity.Property(e => e.SeriesIndex)
                .HasDefaultValueSql("1.0")
                .HasColumnName("series_index");
            entity.Property(e => e.Sort).HasColumnName("sort");
            entity.Property(e => e.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("TIMESTAMP")
                .HasColumnName("timestamp");
            entity.Property(e => e.Title)
                .HasDefaultValueSql("'Unknown'")
                .HasColumnName("title");
            entity.Property(e => e.Uuid).HasColumnName("uuid");

            entity.HasMany<Author>(f=>f.Authors)
                .WithMany()
                .UsingEntity<BookAuthorLink>();
        });
        
        modelBuilder.Entity<BookAuthorLink>(entity =>
        {
             entity.ToTable("books_authors_link");
        
            entity.HasIndex(e => new {Book = e.BookId, Author = e.AuthorId }, "IX_books_authors_link_book_author").IsUnique();
             entity.HasIndex(e => e.AuthorId, "books_authors_link_aidx");
             entity.HasIndex(e => e.BookId, "books_authors_link_bidx");
        
             entity.Property(e => e.Id)
                 .ValueGeneratedNever()
                 .HasColumnName("id");
             entity.Property(k=>k.AuthorId).HasColumnName("author");
             entity.Property(b=>b.BookId).HasColumnName("book");
             
         });

        modelBuilder.Entity<BooksLanguagesLink>(entity =>
        {
            entity.ToTable("books_languages_link");

            entity.HasIndex(e => new { e.Book, e.LangCode }, "IX_books_languages_link_book_lang_code").IsUnique();

            entity.HasIndex(e => e.LangCode, "books_languages_link_aidx");

            entity.HasIndex(e => e.Book, "books_languages_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.ItemOrder).HasColumnName("item_order");
            entity.Property(e => e.LangCode).HasColumnName("lang_code");
        });

        modelBuilder.Entity<BooksPluginDatum>(entity =>
        {
            entity.ToTable("books_plugin_data");

            entity.HasIndex(e => new { e.Book, e.Name }, "IX_books_plugin_data_book_name").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Val).HasColumnName("val");
        });

        modelBuilder.Entity<BooksPublishersLink>(entity =>
        {
            entity.ToTable("books_publishers_link");

            entity.HasIndex(e => e.Book, "IX_books_publishers_link_book").IsUnique();

            entity.HasIndex(e => e.Publisher, "books_publishers_link_aidx");

            entity.HasIndex(e => e.Book, "books_publishers_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Publisher).HasColumnName("publisher");
        });

        modelBuilder.Entity<BooksRatingsLink>(entity =>
        {
            entity.ToTable("books_ratings_link");

            entity.HasIndex(e => new { e.Book, e.Rating }, "IX_books_ratings_link_book_rating").IsUnique();

            entity.HasIndex(e => e.Rating, "books_ratings_link_aidx");

            entity.HasIndex(e => e.Book, "books_ratings_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Rating).HasColumnName("rating");
        });

        modelBuilder.Entity<BooksSeriesLink>(entity =>
        {
            entity.ToTable("books_series_link");

            entity.HasIndex(e => e.Book, "IX_books_series_link_book").IsUnique();

            entity.HasIndex(e => e.Series, "books_series_link_aidx");

            entity.HasIndex(e => e.Book, "books_series_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Series).HasColumnName("series");
        });

        modelBuilder.Entity<BooksTagsLink>(entity =>
        {
            entity.ToTable("books_tags_link");

            entity.HasIndex(e => new { e.Book, e.Tag }, "IX_books_tags_link_book_tag").IsUnique();

            entity.HasIndex(e => e.Tag, "books_tags_link_aidx");

            entity.HasIndex(e => e.Book, "books_tags_link_bidx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Tag).HasColumnName("tag");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comments");

            entity.HasIndex(e => e.Book, "IX_comments_book").IsUnique();

            entity.HasIndex(e => e.Book, "comments_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Text).HasColumnName("text");
        });

        modelBuilder.Entity<ConversionOption>(entity =>
        {
            entity.ToTable("conversion_options");

            entity.HasIndex(e => new { e.Format, e.Book }, "IX_conversion_options_format_book").IsUnique();

            entity.HasIndex(e => e.Format, "conversion_options_idx_a");

            entity.HasIndex(e => e.Book, "conversion_options_idx_b");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.Format).HasColumnName("format");
        });

        modelBuilder.Entity<CustomColumn>(entity =>
        {
            entity.ToTable("custom_columns");

            entity.HasIndex(e => e.Label, "IX_custom_columns_label").IsUnique();

            entity.HasIndex(e => e.Label, "custom_columns_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Datatype).HasColumnName("datatype");
            entity.Property(e => e.Display)
                .HasDefaultValueSql("\"{}\"")
                .HasColumnName("display");
            entity.Property(e => e.Editable)
                .HasDefaultValueSql("1")
                .HasColumnType("BOOL")
                .HasColumnName("editable");
            entity.Property(e => e.IsMultiple)
                .HasDefaultValueSql("0")
                .HasColumnType("BOOL")
                .HasColumnName("is_multiple");
            entity.Property(e => e.Label).HasColumnName("label");
            entity.Property(e => e.MarkForDelete)
                .HasDefaultValueSql("0")
                .HasColumnType("BOOL")
                .HasColumnName("mark_for_delete");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Normalized)
                .HasColumnType("BOOL")
                .HasColumnName("normalized");
        });

        modelBuilder.Entity<LibraryContent>(entity =>
        {
            entity.ToTable("data");

            entity.HasIndex(e => new { e.Book, e.Format }, "IX_data_book_format").IsUnique();

            entity.HasIndex(e => e.Book, "data_idx");

            entity.HasIndex(e => e.Format, "formats_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Format).HasColumnName("format");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.UncompressedSize).HasColumnName("uncompressed_size");

            entity.HasOne<Book>()
                .WithMany(k => k.LibraryContents)
                .HasForeignKey(k=>k.Book);
        });

        modelBuilder.Entity<Feed>(entity =>
        {
            entity.ToTable("feeds");

            entity.HasIndex(e => e.Title, "IX_feeds_title").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Script).HasColumnName("script");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Identifier>(entity =>
        {
            entity.ToTable("identifiers");

            entity.HasIndex(e => new { e.Book, e.Type }, "IX_identifiers_book_type").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("\"isbn\"")
                .HasColumnName("type");
            entity.Property(e => e.Val).HasColumnName("val");
        });

        modelBuilder.Entity<Language>(entity =>
        {
            entity.ToTable("languages");

            entity.HasIndex(e => e.LangCode, "IX_languages_lang_code").IsUnique();

            entity.HasIndex(e => e.LangCode, "languages_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.LangCode).HasColumnName("lang_code");
            entity.Property(e => e.Link)
                .HasDefaultValueSql("''")
                .HasColumnName("link");
        });

        modelBuilder.Entity<LastReadPosition>(entity =>
        {
            entity.ToTable("last_read_positions");

            entity.HasIndex(e => new { e.User, e.Device, e.Book, e.Format }, "IX_last_read_positions_user_device_book_format").IsUnique();

            entity.HasIndex(e => e.Book, "lrp_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
            entity.Property(e => e.Cfi).HasColumnName("cfi");
            entity.Property(e => e.Device).HasColumnName("device");
            entity.Property(e => e.Epoch).HasColumnName("epoch");
            entity.Property(e => e.Format).HasColumnName("format");
            entity.Property(e => e.PosFrac).HasColumnName("pos_frac");
            entity.Property(e => e.User).HasColumnName("user");
        });

        modelBuilder.Entity<LibraryId>(entity =>
        {
            entity.ToTable("library_id");

            entity.HasIndex(e => e.Uuid, "IX_library_id_uuid").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Uuid).HasColumnName("uuid");
        });

        modelBuilder.Entity<MetadataDirtied>(entity =>
        {
            entity.ToTable("metadata_dirtied");

            entity.HasIndex(e => e.Book, "IX_metadata_dirtied_book").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Book).HasColumnName("book");
        });

        modelBuilder.Entity<Preference>(entity =>
        {
            entity.ToTable("preferences");

            entity.HasIndex(e => e.Key, "IX_preferences_key").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Key).HasColumnName("key");
            entity.Property(e => e.Val).HasColumnName("val");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.ToTable("publishers");

            entity.HasIndex(e => e.Name, "IX_publishers_name").IsUnique();

            entity.HasIndex(e => e.Name, "publishers_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Link)
                .HasDefaultValueSql("''")
                .HasColumnName("link");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Sort).HasColumnName("sort");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.ToTable("ratings");

            entity.HasIndex(e => e.Rating1, "IX_ratings_rating").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Link)
                .HasDefaultValueSql("''")
                .HasColumnName("link");
            entity.Property(e => e.Rating1).HasColumnName("rating");
        });

        modelBuilder.Entity<Series>(entity =>
        {
            entity.ToTable("series");

            entity.HasIndex(e => e.Name, "IX_series_name").IsUnique();

            entity.HasIndex(e => e.Name, "series_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Link)
                .HasDefaultValueSql("''")
                .HasColumnName("link");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Sort).HasColumnName("sort");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tags");

            entity.HasIndex(e => e.Name, "IX_tags_name").IsUnique();

            entity.HasIndex(e => e.Name, "tags_idx");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Link)
                .HasDefaultValueSql("''")
                .HasColumnName("link");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
